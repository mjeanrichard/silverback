﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Silverback.Messaging.Messages;

namespace Silverback.Messaging.Outbound.TransactionalOutbox.Repositories.Model
{
    /// <summary>
    ///     Encapsulates the information related to a message stored in the outbox.
    /// </summary>
    public class OutboxStoredMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OutboxStoredMessage" /> class.
        /// </summary>
        /// <param name="messageType">
        ///     The type of the message.
        /// </param>
        /// <param name="content">
        ///     The message raw binary content (body).
        /// </param>
        /// <param name="headers">
        ///     The message headers.
        /// </param>
        /// <param name="endpointName">
        ///     The name of the target endpoint.
        /// </param>
        public OutboxStoredMessage(
            Type? messageType,
            byte[]? content,
            IEnumerable<MessageHeader>? headers,
            string endpointName)
        {
            MessageType = messageType;
            Content = content;
            Headers = headers?.ToList();
            EndpointName = endpointName;
        }

        /// <summary>
        ///     Gets the message raw binary content (body).
        /// </summary>
        [SuppressMessage("", "CA1819", Justification = Justifications.CanExposeByteArray)]
        public byte[]? Content { get; }

        /// <summary>
        ///     Gets the message headers.
        /// </summary>
        public IReadOnlyCollection<MessageHeader>? Headers { get; }

        /// <summary>
        ///     Gets the name of the target endpoint.
        /// </summary>
        public string EndpointName { get; }

        /// <summary>
        ///     Gets or sets the type of the message.
        /// </summary>
        public Type? MessageType { get; set; }
    }
}
