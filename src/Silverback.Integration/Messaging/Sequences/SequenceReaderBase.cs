﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.Threading.Tasks;
using Silverback.Messaging.Broker.Behaviors;
using Silverback.Messaging.Messages;
using Silverback.Util;

namespace Silverback.Messaging.Sequences
{
    /// <summary>
    ///     The base class for the <see cref="ISequenceReader" /> implementations. It encapsulates the logic to
    ///     deal with the <see cref="ISequenceStore" />.
    /// </summary>
    public abstract class SequenceReaderBase : ISequenceReader
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SequenceReaderBase" /> class.
        /// </summary>
        /// <param name="handlesRawMessages">
        ///     A value indicating whether this reader handles the raw messages, before they are being deserialized,
        ///     decrypted, etc.
        /// </param>
        protected SequenceReaderBase(bool handlesRawMessages = false)
        {
            HandlesRawMessages = handlesRawMessages;
        }

        /// <inheritdoc cref="ISequenceReader.HandlesRawMessages" />
        public bool HandlesRawMessages { get; }

        /// <inheritdoc cref="ISequenceReader.CanHandleAsync" />
        public abstract Task<bool> CanHandleAsync(ConsumerPipelineContext context);

        /// <inheritdoc cref="ISequenceReader.GetSequenceAsync" />
        public async Task<ISequence?> GetSequenceAsync(ConsumerPipelineContext context)
        {
            Check.NotNull(context, nameof(context));

            string sequenceId = GetSequenceId(context);
            bool isNewSequence = IsNewSequence(context);

            if (string.IsNullOrEmpty(sequenceId))
                throw new InvalidOperationException("Sequence identifier not found or invalid.");

            return isNewSequence
                ? await CreateNewSequenceAsync(sequenceId, context).ConfigureAwait(false)
                : await GetExistingSequenceAsync(context, sequenceId).ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets the sequence identifier extracted from the current envelope.
        /// </summary>
        /// <param name="context">
        ///     The current <see cref="ConsumerPipelineContext" />.
        /// </param>
        /// <returns>
        ///     The recognized sequence identifier, or <c>null</c>.
        /// </returns>
        protected virtual string GetSequenceId(ConsumerPipelineContext context)
        {
            Check.NotNull(context, nameof(context));

            return context.Envelope.Headers.GetValue(DefaultMessageHeaders.MessageId) ?? "***default***";
        }

        /// <summary>
        ///     Determines if the current message correspond with the beginning of a new sequence.
        /// </summary>
        /// <param name="context">
        ///     The current <see cref="ConsumerPipelineContext" />.
        /// </param>
        /// <returns>
        ///     <c>true</c> if a new sequence is starting; otherwise <c>false</c>.
        /// </returns>
        protected abstract bool IsNewSequence(ConsumerPipelineContext context);

        /// <summary>
        ///     Creates the new sequence and adds it to the store.
        /// </summary>
        /// <param name="sequenceId">
        ///     The sequence identifier.
        /// </param>
        /// <param name="context">
        ///     The current <see cref="ConsumerPipelineContext" />.
        /// </param>
        /// <returns>
        ///     The new sequence.
        /// </returns>
        protected virtual async Task<ISequence> CreateNewSequenceAsync(
            string sequenceId,
            ConsumerPipelineContext context)
        {
            Check.NotNull(context, nameof(context));

            var sequence = CreateNewSequenceCore(sequenceId, context);

            if (context.SequenceStore.HasPendingSequences)
                await AbortPreviousSequencesAsync(context.SequenceStore, sequence).ConfigureAwait(false);

            await context.SequenceStore.AddAsync(sequence).ConfigureAwait(false);

            return sequence;
        }

        /// <summary>
        ///     Creates the new sequence object.
        /// </summary>
        /// <param name="sequenceId">
        ///     The sequence identifier.
        /// </param>
        /// <param name="context">
        ///     The current <see cref="ConsumerPipelineContext" />.
        /// </param>
        /// <returns>
        ///     The new sequence.
        /// </returns>
        protected abstract ISequence CreateNewSequenceCore(string sequenceId, ConsumerPipelineContext context);

        /// <summary>
        ///     Retrieves the existing incomplete sequence from the store.
        /// </summary>
        /// <param name="context">
        ///     The current <see cref="ConsumerPipelineContext" />.
        /// </param>
        /// <param name="sequenceId">
        ///     The sequence identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ISequence" /> or <c>null</c> if not found.
        /// </returns>
        protected virtual async Task<ISequence?> GetExistingSequenceAsync(
            ConsumerPipelineContext context,
            string sequenceId)
        {
            Check.NotNull(context, nameof(context));

            var sequence = await context.SequenceStore.GetAsync<ISequence>(sequenceId).ConfigureAwait(false);

            // Skip the message if a sequence cannot be found. It means that the consumer started in the
            // middle of a sequence.
            if (sequence == null)
            {
                // TODO: Log
            }

            return sequence;
        }

        private static async Task AbortPreviousSequencesAsync(ISequenceStore sequenceStore, ISequence currentSequence)
        {
            await sequenceStore.ForEachAsync(
                    previousSequence =>
                    {
                        // Prevent Sequence and RawSequence to mess with each other
                        if (currentSequence is RawSequence && previousSequence is Sequence ||
                            currentSequence is Sequence && previousSequence is RawSequence)
                            return Task.CompletedTask;

                        return previousSequence.AbortAsync(SequenceAbortReason.IncompleteSequence);
                    })
                .ConfigureAwait(false);
        }
    }
}
