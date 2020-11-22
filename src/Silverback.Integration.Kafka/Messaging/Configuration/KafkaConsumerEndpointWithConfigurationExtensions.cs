// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using Silverback.Util;

namespace Silverback.Messaging.Configuration
{
    /// <summary>
    ///     Adds the <c>WithConfiguration</c> method to the <see cref="KafkaConsumerEndpoint" />.
    /// </summary>
    public static class KafkaConsumerEndpointWithConfigurationExtensions
    {
        /// <summary>
        ///     Sets the Kafka connection configuration.
        /// </summary>
        /// <param name="endpoint">
        ///     The <see cref="KafkaConsumerEndpoint" />.
        /// </param>
        /// <param name="config">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The <see cref="KafkaConsumerEndpoint" /> so that additional calls can be chained.
        /// </returns>
        public static KafkaConsumerEndpoint WithConfiguration(
            this KafkaConsumerEndpoint endpoint,
            KafkaConsumerConfig config)
        {
            Check.NotNull(endpoint, nameof(endpoint));
            Check.NotNull(config, nameof(config));

            endpoint.Configuration = config;

            return endpoint;
        }

        /// <summary>
        ///     Sets the Kafka connection configuration.
        /// </summary>
        /// <param name="endpoint">
        ///     The <see cref="KafkaConsumerEndpoint" />.
        /// </param>
        /// <param name="configAction">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The <see cref="KafkaConsumerEndpoint" /> so that additional calls can be chained.
        /// </returns>
        public static KafkaConsumerEndpoint WithConfiguration(
            this KafkaConsumerEndpoint endpoint,
            Action<KafkaConsumerConfig> configAction)
        {
            Check.NotNull(endpoint, nameof(endpoint));
            Check.NotNull(configAction, nameof(configAction));

            var config = new KafkaConsumerConfig();
            configAction.Invoke(config);
            endpoint.Configuration = config;

            return endpoint;
        }
    }
}
