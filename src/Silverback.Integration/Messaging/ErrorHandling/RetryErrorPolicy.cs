﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Silverback.Messaging.Messages;

namespace Silverback.Messaging.ErrorHandling
{
    /// <summary>
    ///     This policy retries the handler method multiple times in case of exception.
    ///     An optional delay can be specified.
    /// </summary>
    /// TODO: Exponential backoff variant
    public class RetryErrorPolicy : ErrorPolicyBase
    {
        private readonly TimeSpan _initialDelay;
        private readonly TimeSpan _delayIncrement;
        private readonly ILogger _logger;
        private readonly MessageLogger _messageLogger;

        public RetryErrorPolicy(
            IServiceProvider serviceProvider,
            ILogger<RetryErrorPolicy> logger,
            MessageLogger messageLogger,
            TimeSpan? initialDelay = null,
            TimeSpan? delayIncrement = null)
            : base(serviceProvider, logger, messageLogger)
        {
            _initialDelay = initialDelay ?? TimeSpan.Zero;
            _delayIncrement = delayIncrement ?? TimeSpan.Zero;
            _logger = logger;
            _messageLogger = messageLogger;
        }

        protected override ErrorAction ApplyPolicy(
            IReadOnlyCollection<IRawInboundEnvelope> envelopes,
            Exception exception)
        {
            ApplyDelay(envelopes);

            _messageLogger.LogInformation(_logger, "The message(s) will be processed again.", envelopes);

            return ErrorAction.Retry;
        }

        private void ApplyDelay(IReadOnlyCollection<IRawInboundEnvelope> envelopes)
        {
            var delay = (int)_initialDelay.TotalMilliseconds +
                        envelopes.First().Headers.GetValueOrDefault<int>(DefaultMessageHeaders.FailedAttempts) *
                        (int)_delayIncrement.TotalMilliseconds;

            if (delay <= 0)
                return;

            _messageLogger.LogTrace(_logger, $"Waiting {delay} milliseconds before retrying to process the message(s).",
                envelopes);
            Thread.Sleep(delay);
        }
    }
}