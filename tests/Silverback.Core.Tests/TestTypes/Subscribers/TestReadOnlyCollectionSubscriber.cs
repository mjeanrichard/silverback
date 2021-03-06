﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Silverback.Messaging.Subscribers;
using Silverback.Tests.Core.TestTypes.Messages;

namespace Silverback.Tests.Core.TestTypes.Subscribers
{
    public class TestReadOnlyCollectionSubscriber
    {
        public int ReceivedMessagesCount { get; private set; }

        public int ReceivedBatchesCount { get; private set; }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = Justifications.CalledBySilverback)]
        [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = Justifications.CalledBySilverback)]
        [Subscribe]
        public void OnTestMessagesReceived(IReadOnlyCollection<ITestMessage> messages)
        {
            ReceivedBatchesCount++;
            ReceivedMessagesCount += messages.Count;
        }
    }
}
