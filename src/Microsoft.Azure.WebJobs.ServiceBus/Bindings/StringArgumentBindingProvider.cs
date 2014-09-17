﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Microsoft.Azure.WebJobs.ServiceBus.Bindings
{
    internal class StringArgumentBindingProvider : IQueueArgumentBindingProvider
    {
        public IArgumentBinding<ServiceBusEntity> TryCreate(ParameterInfo parameter)
        {
            if (!parameter.IsOut || parameter.ParameterType != typeof(string).MakeByRefType())
            {
                return null;
            }

            return new StringArgumentBinding();
        }

        private class StringArgumentBinding : IArgumentBinding<ServiceBusEntity>
        {
            public Type ValueType
            {
                get { return typeof(string); }
            }

            /// <remarks>
            /// As this method handles out string parameter it distinguishes following possible scenarios:
            /// <item>
            /// <description>
            /// If the value is <see langword="null"/>, no message will be sent.
            /// </description>
            /// </item>
            /// <item>
            /// <description>
            /// If the value is an empty string, a message with empty content will be sent.
            /// </description>
            /// </item>
            /// <item>
            /// <description>
            /// If the value is a non-empty string, a message with content from given argument will be sent.
            /// </description>
            /// </item>
            /// </remarks>
            public Task<IValueProvider> BindAsync(ServiceBusEntity value, ValueBindingContext context)
            {
                IValueProvider provider = new NonNullConverterValueBinder<string>(value,
                    new StringToBrokeredMessageConverter(), context.FunctionInstanceId);
                return Task.FromResult(provider);
            }
        }
    }
}
