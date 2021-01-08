﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using FluentAssertions;
using MQTTnet.Client.Options;
using MQTTnet.Formatter;
using Silverback.Messaging;
using Silverback.Messaging.Configuration.Mqtt;
using Xunit;

namespace Silverback.Tests.Integration.Mqtt.Messaging.Configuration.Mqtt
{
    public class MqttClientConfigTests
    {
        [Fact]
        public void Default_ProtocolVersionV500Set()
        {
            var config = new MqttClientConfig();

            config.ProtocolVersion.Should().Be(MqttProtocolVersion.V500);
        }

        [Fact]
        public void Validate_ValidConfiguration_NoExceptionThrown()
        {
            var config = GetValidConfig();

            Action act = () => config.Validate();

            act.Should().NotThrow();
        }

        [Fact]
        public void Validate_MissingClientId_ExceptionThrown()
        {
            var config = GetValidConfig();

            config.ClientId = string.Empty;

            Action act = () => config.Validate();

            act.Should().ThrowExactly<EndpointConfigurationException>();
        }

        [Fact]
        public void Validate_MissingChannelOptions_ExceptionThrown()
        {
            var config = GetValidConfig();

            config.ChannelOptions = null;

            Action act = () => config.Validate();

            act.Should().ThrowExactly<EndpointConfigurationException>();
        }

        [Fact]
        public void Validate_MissingTcpServer_ExceptionThrown()
        {
            var config = GetValidConfig();

            config.ChannelOptions = new MqttClientTcpOptions();

            Action act = () => config.Validate();

            act.Should().ThrowExactly<EndpointConfigurationException>();
        }

        private static MqttClientConfig GetValidConfig() => new()
        {
            ChannelOptions = new MqttClientTcpOptions
            {
                Server = "test-server"
            }
        };
    }
}
