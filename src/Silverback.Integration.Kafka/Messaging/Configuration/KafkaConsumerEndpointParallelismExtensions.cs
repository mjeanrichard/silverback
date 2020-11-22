// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using Silverback.Messaging.Sequences.Batch;
using Silverback.Messaging.Sequences.Chunking;
using Silverback.Util;

namespace Silverback.Messaging.Configuration
{
    /// <summary>
    ///     Adds the <c>ProcessPartitionsIndependently</c>, <c>ProcessAllPartitionsTogether</c>,
    ///     <c>LimitParallelism</c> and <c>LimitBackpressure</c> methods to the <see cref="KafkaConsumerEndpoint" />.
    /// </summary>
    public static class KafkaConsumerEndpointParallelismExtensions
    {
        /// <summary>
        ///     Specifies that the partitions must be processed independently. This means that a stream will published
        ///     per each partition and the sequences (<see cref="ChunkSequence" />, <see cref="BatchSequence" />, ...)
        ///     cannot span across the partitions. This option is enabled by default. Use
        ///     <see cref="ProcessAllPartitionsTogether" /> to disable it.
        /// </summary>
        /// <param name="endpoint">
        ///     The <see cref="KafkaConsumerEndpoint" />.
        /// </param>
        /// <returns>
        ///     The <see cref="KafkaConsumerEndpoint" /> so that additional calls can be chained.
        /// </returns>
        public static KafkaConsumerEndpoint ProcessPartitionsIndependently(this KafkaConsumerEndpoint endpoint)
        {
            Check.NotNull(endpoint, nameof(endpoint));

            endpoint.ProcessPartitionsIndependently = true;

            return endpoint;
        }

        /// <summary>
        ///     Specifies that all partitions must be processed together. This means that a single stream will
        ///     published for the messages from all the partitions and the sequences (<see cref="ChunkSequence" />,
        ///     <see cref="BatchSequence" />, ...) can span across the partitions.
        /// </summary>
        /// <param name="endpoint">
        ///     The <see cref="KafkaConsumerEndpoint" />.
        /// </param>
        /// <returns>
        ///     The <see cref="KafkaConsumerEndpoint" /> so that additional calls can be chained.
        /// </returns>
        public static KafkaConsumerEndpoint ProcessAllPartitionsTogether(this KafkaConsumerEndpoint endpoint)
        {
            Check.NotNull(endpoint, nameof(endpoint));

            endpoint.ProcessPartitionsIndependently = false;

            return endpoint;
        }

        /// <summary>
        ///     Sets the maximum number of incoming message that can be processed concurrently. Up to a message per
        ///     each subscribed partition can be processed in parallel.
        ///     The default limit is 10.
        /// </summary>
        /// <param name="endpoint">
        ///     The <see cref="KafkaConsumerEndpoint" />.
        /// </param>
        /// <param name="maxDegreeOfParallelism">
        ///     The maximum number of incoming message that can be processed concurrently.
        /// </param>
        /// <returns>
        ///     The <see cref="KafkaConsumerEndpoint" /> so that additional calls can be chained.
        /// </returns>
        public static KafkaConsumerEndpoint LimitParallelism(
            this KafkaConsumerEndpoint endpoint,
            int maxDegreeOfParallelism)
        {
            Check.NotNull(endpoint, nameof(endpoint));

            endpoint.MaxDegreeOfParallelism = maxDegreeOfParallelism;

            return endpoint;
        }

        /// <summary>
        ///     Sets the maximum number of messages to be consumed and enqueued waiting to be processed.
        ///     The limit will be applied per partition when processing the partitions independently (default).
        ///     The default limit is 1.
        /// </summary>
        /// <param name="endpoint">
        ///     The <see cref="KafkaConsumerEndpoint" />.
        /// </param>
        /// <param name="backpressureLimit">
        ///     The maximum number of messages to be enqueued.
        /// </param>
        /// <returns>
        ///     The <see cref="KafkaConsumerEndpoint" /> so that additional calls can be chained.
        /// </returns>
        public static KafkaConsumerEndpoint LimitBackpressure(
            this KafkaConsumerEndpoint endpoint,
            int backpressureLimit)
        {
            Check.NotNull(endpoint, nameof(endpoint));

            endpoint.BackpressureLimit = backpressureLimit;

            return endpoint;
        }
    }
}
