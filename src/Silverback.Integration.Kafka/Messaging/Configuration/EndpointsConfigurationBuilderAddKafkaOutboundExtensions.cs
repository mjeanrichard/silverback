// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Silverback.Messaging.Outbound.Routing;
using Silverback.Util;

namespace Silverback.Messaging.Configuration
{
    /// <summary>
    ///     Adds the <c>AddKafkaOutbound</c> method to the <see cref="IEndpointsConfigurationBuilder" />.
    /// </summary>
    public static class EndpointsConfigurationBuilderAddKafkaOutboundExtensions
    {
        /// <summary>
        ///     Adds an outbound endpoint to produce the specified message type to a Kafka topic.
        /// </summary>
        /// <param name="endpointsConfigurationBuilder">
        ///     The <see cref="IEndpointsConfigurationBuilder" />.
        /// </param>
        /// <param name="topicName">
        ///     The name of the topic.
        /// </param>
        /// <param name="endpointAction">
        ///    Additional endpoint options such as the connection details.
        /// </param>
        /// <typeparam name="TMessage">
        ///     The type of the messages to be published to this endpoint.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IEndpointsConfigurationBuilder" /> so that additional calls can be chained.
        /// </returns>
        public static IEndpointsConfigurationBuilder AddKafkaOutbound<TMessage>(
            this IEndpointsConfigurationBuilder endpointsConfigurationBuilder,
            string topicName,
            Action<KafkaProducerEndpoint> endpointAction)
        {
            Check.NotNull(endpointsConfigurationBuilder, nameof(endpointsConfigurationBuilder));
            Check.NotEmpty(topicName, nameof(topicName));
            Check.NotNull(endpointAction, nameof(endpointAction));

            return endpointsConfigurationBuilder.AddKafkaOutbound(typeof(TMessage), topicName, endpointAction);
        }

        /// <summary>
        ///     Adds an outbound endpoint to produce the specified message type to a Kafka topic.
        /// </summary>
        /// <param name="endpointsConfigurationBuilder">
        ///     The <see cref="IEndpointsConfigurationBuilder" />.
        /// </param>
        /// <param name="messageType">
        ///     The type of the messages to be published to this endpoint.
        /// </param>
        /// <param name="topicName">
        ///     The name of the topic.
        /// </param>
        /// <param name="endpointAction">
        ///    Additional endpoint options such as the connection details.
        /// </param>
        /// <returns>
        ///     The <see cref="IEndpointsConfigurationBuilder" /> so that additional calls can be chained.
        /// </returns>
        public static IEndpointsConfigurationBuilder AddKafkaOutbound(
            this IEndpointsConfigurationBuilder endpointsConfigurationBuilder,
            Type messageType,
            string topicName,
            Action<KafkaProducerEndpoint> endpointAction)
        {
            Check.NotNull(endpointsConfigurationBuilder, nameof(endpointsConfigurationBuilder));
            Check.NotNull(messageType, nameof(messageType));
            Check.NotEmpty(topicName, nameof(topicName));
            Check.NotNull(endpointAction, nameof(endpointAction));

            var endpoint = new KafkaProducerEndpoint(topicName);
            endpointAction.Invoke(endpoint);

            return endpointsConfigurationBuilder.AddOutbound(messageType, endpoint);
        }
    }
}
