﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Silverback.Messaging.Broker.Behaviors;
using Silverback.Util;

namespace Silverback.Messaging.Sequences
{
    /// <summary>
    ///     Uses the available implementations of <see cref="ISequenceWriter" /> to set the proper headers and
    ///     split the published message or messages set to create the sequences.
    /// </summary>
    /// <remarks>
    ///     A sequence is a set of messages that are handled as a single unit of work. A sequence could be used to
    ///     group all chunks belonging to the same source message, all messages belonging to the same data set or
    ///     to implement batch processing.
    /// </remarks>
    public class SequencerProducerBehavior : IProducerBehavior
    {
        private readonly IReadOnlyCollection<ISequenceWriter> _sequenceWriters;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SequencerProducerBehavior" /> class.
        /// </summary>
        /// <param name="sequenceWriters">
        ///     The <see cref="ISequenceWriter" /> implementations to be used.
        /// </param>
        public SequencerProducerBehavior(IEnumerable<ISequenceWriter> sequenceWriters)
        {
            _sequenceWriters = sequenceWriters.ToList();
        }

        /// <inheritdoc cref="ISorted.SortIndex" />
        public int SortIndex => BrokerBehaviorsSortIndexes.Producer.Sequencer;

        /// <inheritdoc cref="IProducerBehavior.Handle" />
        public async Task Handle(ProducerPipelineContext context, ProducerBehaviorHandler next)
        {
            Check.NotNull(context, nameof(context));
            Check.NotNull(next, nameof(next));

            foreach (var sequenceWriter in _sequenceWriters)
            {
                var envelopesEnumerable = await sequenceWriter.CreateSequence(context.Envelope).ConfigureAwait(false);

                if (envelopesEnumerable != null)
                {
                    await foreach (var envelope in envelopesEnumerable)
                    {
                        await next(context).ConfigureAwait(false);
                    }

                    return;
                }
            }

            await next(context).ConfigureAwait(false);
        }
    }
}