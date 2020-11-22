// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using Silverback.Messaging.Serialization;
using Silverback.Util;

namespace Silverback.Messaging.Configuration
{
    /// <summary>
    ///     Adds the <c>SerializeAsJson</c> method to the <see cref="ProducerEndpoint" />.
    /// </summary>
    public static class ProducerEndpointSerializeAsJsonExtensions
    {
        /// <summary>
        ///     Sets the serializer to an instance of <see cref="JsonMessageSerializer" /> to serialize the produced
        ///     messages in JSON format and forward the message type as header. This serializer is ideal when the
        ///     producer and the consumer are both using Silverback.
        /// </summary>
        /// <param name="endpoint">
        ///     The <see cref="ProducerEndpoint" />.
        /// </param>
        /// <param name="configAction">
        ///     The (optional) additional configuration.
        /// </param>
        /// <returns>
        ///     The <see cref="ProducerEndpoint" /> so that additional calls can be chained.
        /// </returns>
        public static ProducerEndpoint SerializeAsJson(
            this ProducerEndpoint endpoint,
            Action<JsonMessageSerializer>? configAction = null)
        {
            Check.NotNull(endpoint, nameof(endpoint));

            var serializer = new JsonMessageSerializer();
            configAction?.Invoke(serializer);
            endpoint.Serializer = serializer;

            return endpoint;
        }

        /// <summary>
        ///     Sets the serializer to an instance of <see cref="JsonMessageSerializer{TMessage}" /> to serialize the
        ///     produced messages of type <typeparamref name="TMessage" /> in JSON format.
        /// </summary>
        /// <typeparam name="TMessage">
        ///     The type of the messages to be serialized.
        /// </typeparam>
        /// <param name="endpoint">
        ///     The <see cref="ProducerEndpoint" />.
        /// </param>
        /// <param name="configAction">
        ///     The (optional) additional configuration.
        /// </param>
        /// <returns>
        ///     The <see cref="ProducerEndpoint" /> so that additional calls can be chained.
        /// </returns>
        public static ProducerEndpoint SerializeAsJson<TMessage>(
            this ProducerEndpoint endpoint,
            Action<JsonMessageSerializer<TMessage>>? configAction = null)
        {
            Check.NotNull(endpoint, nameof(endpoint));

            var serializer = new JsonMessageSerializer<TMessage>();
            configAction?.Invoke(serializer);
            endpoint.Serializer = serializer;

            return endpoint;
        }
    }
}
