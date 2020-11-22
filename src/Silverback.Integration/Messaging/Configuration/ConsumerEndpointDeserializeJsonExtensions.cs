// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using Silverback.Messaging.Serialization;
using Silverback.Util;

namespace Silverback.Messaging.Configuration
{
    /// <summary>
    ///     Adds the <c>DeserializeJson</c> method to the <see cref="ConsumerEndpoint" />.
    /// </summary>
    public static class ConsumerEndpointDeserializeJsonExtensions
    {
        /// <summary>
        ///     Sets the serializer to an instance of <see cref="JsonMessageSerializer" /> to deserialize the consumed
        ///     JSON. This serializer relies on the message type header to determine the type of the message to be
        ///     deserialized.
        /// </summary>
        /// <param name="endpoint">
        ///     The <see cref="ConsumerEndpoint" />.
        /// </param>
        /// <param name="configAction">
        ///     The (optional) additional configuration.
        /// </param>
        /// <returns>
        ///     The <see cref="ConsumerEndpoint" /> so that additional calls can be chained.
        /// </returns>
        public static ConsumerEndpoint DeserializeJson(
            this ConsumerEndpoint endpoint,
            Action<JsonMessageSerializer>? configAction = null)
        {
            Check.NotNull(endpoint, nameof(endpoint));

            var serializer = new JsonMessageSerializer();
            configAction?.Invoke(serializer);
            endpoint.Serializer = serializer;

            return endpoint;
        }

        /// <summary>
        ///     Sets the serializer to an instance of <see cref="JsonMessageSerializer{TMessage}" /> to deserialize the
        ///     consumed JSON into a message of type <typeparamref name="TMessage" />.
        /// </summary>
        /// <typeparam name="TMessage">
        ///     The type of the messages to be deserialized.
        /// </typeparam>
        /// <param name="endpoint">
        ///     The <see cref="ConsumerEndpoint" />.
        /// </param>
        /// <param name="configAction">
        ///     The (optional) additional configuration.
        /// </param>
        /// <returns>
        ///     The <see cref="ConsumerEndpoint" /> so that additional calls can be chained.
        /// </returns>
        public static ConsumerEndpoint DeserializeJson<TMessage>(
            this ConsumerEndpoint endpoint,
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
