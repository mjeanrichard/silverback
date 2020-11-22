// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using Silverback.Util;

namespace Silverback.Messaging.Configuration
{
    /// <summary>
    ///     Adds the <c>ThrowIfUnhandled</c> and <c>IgnoreUnhandledMessages</c> methods to the <see cref="ConsumerEndpoint" />.
    /// </summary>
    public static class ConsumerEndpointThrowIfUnhandledExtensions
    {
        /// <summary>
        ///     Specifies that an exception must be thrown if no subscriber is handling the received message. This
        ///     option is enabled by default. Use the <see cref="IgnoreUnhandledMessages"/> to disable it.
        /// </summary>
        /// <param name="endpoint">
        ///     The <see cref="ConsumerEndpoint" />.
        /// </param>
        /// <returns>
        ///     The <see cref="ConsumerEndpoint" /> so that additional calls can be chained.
        /// </returns>
        public static ConsumerEndpoint ThrowIfUnhandled(this ConsumerEndpoint endpoint)
        {
            Check.NotNull(endpoint, nameof(endpoint));

            endpoint.ThrowIfUnhandled = true;

            return endpoint;
        }

        /// <summary>
        ///     Specifies that the message has to be silently ignored if no subscriber is handling it.
        /// </summary>
        /// <param name="endpoint">
        ///     The <see cref="ConsumerEndpoint" />.
        /// </param>
        /// <returns>
        ///     The <see cref="ConsumerEndpoint" /> so that additional calls can be chained.
        /// </returns>
        public static ConsumerEndpoint IgnoreUnhandledMessages(this ConsumerEndpoint endpoint)
        {
            Check.NotNull(endpoint, nameof(endpoint));

            endpoint.ThrowIfUnhandled = false;

            return endpoint;
        }
    }
}
