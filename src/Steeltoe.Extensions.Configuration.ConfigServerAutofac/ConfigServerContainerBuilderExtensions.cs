﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Autofac;
using Microsoft.Extensions.Configuration;
using Steeltoe.Common.HealthChecks;
using Steeltoe.Common.Options.Autofac;
using System;
using System.Linq;

namespace Steeltoe.Extensions.Configuration.ConfigServer
{
    /// <summary>
    /// Extension methods for adding services related to Spring Cloud Config Server.
    /// </summary>
    public static class ConfigServerContainerBuilderExtensions
    {
        public static void RegisterConfigServerClientOptions(this ContainerBuilder container, IConfiguration config)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var section = config.GetSection(ConfigServerClientSettingsOptions.CONFIGURATION_PREFIX);
            container.RegisterOption<ConfigServerClientSettingsOptions>(section);
        }

        public static void RegisterConfigServerHealthCheck(this ContainerBuilder services, IConfiguration configuration)
        {
#pragma warning disable SA1119 // Statement must not use unnecessary parenthesis
            if (!(configuration is IConfigurationRoot root))
#pragma warning restore SA1119 // Statement must not use unnecessary parenthesis
            {
                throw new ArgumentException($"Configuration must be a {nameof(IConfigurationRoot)}", nameof(configuration));
            }

            var configServerSource = root.Providers.FirstOrDefault(x => x is ConfigServerConfigurationProvider);
            if (configServerSource == null)
            {
                throw new InvalidOperationException("Config server is not registered as one of the sources in the configuration");
            }

            services.RegisterInstance(configServerSource).As<IHealthContributor>();
        }
    }
}
