// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using Silverback.Util;

namespace Silverback.Messaging.Configuration
{
    /// <summary>
    ///     Adds the <c>AddKafkaInbound</c> method to the <see cref="IEndpointsConfigurationBuilder" />.
    /// </summary>
    public static class EndpointsConfigurationBuilderAddKafkaInboundExtensions
    {
        /// <summary>
        ///     Adds an inbound endpoint to consume from a Kafka topic.
        /// </summary>
        /// <param name="endpointsConfigurationBuilder">
        ///     The <see cref="IEndpointsConfigurationBuilder" />.
        /// </param>
        /// <param name="topicName">
        ///     The name of the topic to consume.
        /// </param>
        /// <param name="endpointAction">
        ///     Additional endpoint options such as the connection details.
        /// </param>
        /// <param name="consumersCount">
        ///     The number of consumers to be instantiated. The default is 1.
        /// </param>
        /// <returns>
        ///     The <see cref="IEndpointsConfigurationBuilder" /> so that additional calls can be chained.
        /// </returns>
        public static IEndpointsConfigurationBuilder AddKafkaInbound(
            this IEndpointsConfigurationBuilder endpointsConfigurationBuilder,
            string topicName,
            Action<KafkaConsumerEndpoint> endpointAction,
            int consumersCount = 1)
        {
            Check.NotNull(endpointsConfigurationBuilder, nameof(endpointsConfigurationBuilder));
            Check.NotEmpty(topicName, nameof(topicName));
            Check.NotNull(endpointAction, nameof(endpointAction));

            return endpointsConfigurationBuilder.AddKafkaInbound(
                new[] { topicName },
                endpointAction,
                consumersCount);
        }

        /// <summary>
        ///     Adds an inbound endpoint to consume from a Kafka topic.
        /// </summary>
        /// <param name="endpointsConfigurationBuilder">
        ///     The <see cref="IEndpointsConfigurationBuilder" />.
        /// </param>
        /// <param name="topicNames">
        ///     The name of the topics to be consumed.
        /// </param>
        /// <param name="endpointAction">
        ///     Additional endpoint options such as the connection details.
        /// </param>
        /// <param name="consumersCount">
        ///     The number of consumers to be instantiated. The default is 1.
        /// </param>
        /// <returns>
        ///     The <see cref="IEndpointsConfigurationBuilder" /> so that additional calls can be chained.
        /// </returns>
        public static IEndpointsConfigurationBuilder AddKafkaInbound(
            this IEndpointsConfigurationBuilder endpointsConfigurationBuilder,
            string[] topicNames,
            Action<KafkaConsumerEndpoint> endpointAction,
            int consumersCount = 1)
        {
            Check.NotNull(endpointsConfigurationBuilder, nameof(endpointsConfigurationBuilder));
            Check.NotEmpty(topicNames, nameof(topicNames));
            Check.HasNoNulls(topicNames, nameof(topicNames));
            Check.NotNull(endpointAction, nameof(endpointAction));

            var endpoint = new KafkaConsumerEndpoint(topicNames);
            endpointAction.Invoke(endpoint);

            return endpointsConfigurationBuilder.AddInbound(endpoint, consumersCount);
        }
    }
}
