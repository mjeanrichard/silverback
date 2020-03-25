﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Silverback.Messaging.Messages;
using Silverback.Util;

namespace Silverback.Messaging.Broker
{
    public class RabbitProducer : Producer<RabbitBroker, RabbitProducerEndpoint>, IDisposable
    {
        internal const string RoutingKeyHeaderKey = "x-rabbit-routing-key";

        private readonly IRabbitConnectionFactory _connectionFactory;
        private readonly ILogger<Producer> _logger;
        private readonly BlockingCollection<QueuedMessage> _queue = new BlockingCollection<QueuedMessage>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private IModel _channel;

        public RabbitProducer(
            RabbitBroker broker,
            RabbitProducerEndpoint endpoint,
            MessageIdProvider messageIdProvider,
            IEnumerable<IProducerBehavior> behaviors,
            IRabbitConnectionFactory connectionFactory,
            ILogger<Producer> logger,
            MessageLogger messageLogger)
            : base(broker, endpoint, messageIdProvider, behaviors, logger, messageLogger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;

            Task.Run(() => ProcessQueue(_cancellationTokenSource.Token));
        }

        /// <inheritdoc cref="Producer" />
        protected override IOffset Produce(IRawOutboundEnvelope envelope) =>
            AsyncHelper.RunSynchronously(() => ProduceAsync(envelope));

        /// <inheritdoc cref="Producer" />
        protected override Task<IOffset> ProduceAsync(IRawOutboundEnvelope envelope)
        {
            var queuedMessage = new QueuedMessage(envelope);

            _queue.Add(queuedMessage);

            return queuedMessage.TaskCompletionSource.Task;
        }

        private void ProcessQueue(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var queuedMessage = _queue.Take(cancellationToken);

                    try
                    {
                        PublishToChannel(queuedMessage.Envelope);

                        queuedMessage.TaskCompletionSource.SetResult(null);
                    }
                    catch (Exception ex)
                    {
                        queuedMessage.TaskCompletionSource.SetException(ex);
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogTrace(ex, "Producer queue processing was cancelled.");
            }
        }

        private void PublishToChannel(IRawOutboundEnvelope envelope)
        {
            _channel ??= _connectionFactory.GetChannel(Endpoint);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true; // TODO: Make it configurable?
            properties.Headers = envelope.Headers.ToDictionary(header => header.Key, header => (object) header.Value);

            switch (Endpoint)
            {
                case RabbitQueueProducerEndpoint queueEndpoint:
                    _channel.BasicPublish(
                        "",
                        queueEndpoint.Name,
                        properties,
                        envelope.RawMessage);
                    break;
                case RabbitExchangeProducerEndpoint exchangeEndpoint:
                    _channel.BasicPublish(
                        exchangeEndpoint.Name,
                        GetRoutingKey(envelope.Headers),
                        properties,
                        envelope.RawMessage);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Endpoint.ConfirmationTimeout.HasValue)
                _channel.WaitForConfirmsOrDie(Endpoint.ConfirmationTimeout.Value);
        }

        private string GetRoutingKey(IEnumerable<MessageHeader> headers) =>
            headers?.FirstOrDefault(header => header.Key == RoutingKeyHeaderKey)?.Value ?? "";

        private void Flush()
        {
            _queue.CompleteAdding();

            while (!_queue.IsCompleted)
                Task.Delay(100).Wait();
        }

        public void Dispose()
        {
            Flush();

            _cancellationTokenSource.Cancel();

            _channel?.Dispose();
            _channel = null;
        }

        private class QueuedMessage
        {
            public QueuedMessage(IRawOutboundEnvelope envelope)
            {
                Envelope = envelope;
                TaskCompletionSource = new TaskCompletionSource<IOffset>();
            }

            public IRawOutboundEnvelope Envelope { get; }
            public TaskCompletionSource<IOffset> TaskCompletionSource { get; }
        }
    }
}