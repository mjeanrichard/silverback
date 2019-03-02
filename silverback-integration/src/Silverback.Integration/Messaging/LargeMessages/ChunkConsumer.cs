﻿// Copyright (c) 2018-2019 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.Collections.Generic;
using System.Linq;

namespace Silverback.Messaging.LargeMessages
{
    public class ChunkConsumer
    {
        private readonly IChunkStore _store;
        private readonly List<string> _completedMessagesId = new List<string>();

        public ChunkConsumer(IChunkStore store)
        {
            _store = store;
        }

        public byte[] JoinIfComplete(MessageChunk chunk)
        {
            var count = _store.CountChunks(chunk.OriginalMessageId);

            if (count >= chunk.ChunksCount - 1)
            {
                var chunks = _store.GetChunks(chunk.OriginalMessageId);
                if (chunks.ContainsKey(chunk.ChunkId))
                    return null;

                chunks.Add(chunk.ChunkId, chunk.Content);

                var completeMessage = Join(chunks);

                _completedMessagesId.Add(chunk.OriginalMessageId);

                return completeMessage;
            }
            else
            {
                _store.Store(chunk);
                return null;
            }
        }

        public void Commit() => _store.Commit();

        public void Rollback() => _store.Rollback();

        public void CleanupProcessedMessages()
        {
            foreach (var messageId in _completedMessagesId)
                _store.Cleanup(messageId);

            _completedMessagesId.Clear();
        }

        private static byte[] Join(Dictionary<int, byte[]> chunks)
        {
            var buffer = new byte[chunks.Sum(c => c.Value.Length)];
            var offset = 0;
            foreach (var chunk2 in chunks.OrderBy(c => c.Key).Select(c => c.Value))
            {
                Buffer.BlockCopy(chunk2, 0, buffer, offset, chunk2.Length);
                offset += chunk2.Length;
            }

            return buffer;
        }
    }
}