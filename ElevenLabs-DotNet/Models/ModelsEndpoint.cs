﻿// Licensed under the MIT License. See LICENSE in the project root for license information.

using ElevenLabs.Extensions;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElevenLabs.Models
{
    public sealed class ModelsEndpoint : BaseEndPoint
    {
        public ModelsEndpoint(ElevenLabsClient api) : base(api) { }

        protected override string Root => "models";

        /// <summary>
        /// Access the different models available to the platform.
        /// </summary>
        /// <returns>A list of <see cref="Model"/>s you can use.</returns>
        public async Task<IReadOnlyList<Model>> GetModelsAsync()
        {
            var response = await Api.Client.GetAsync(GetUrl());
            var responseAsString = await response.ReadAsStringAsync(EnableDebug);
            return JsonSerializer.Deserialize<IReadOnlyList<Model>>(responseAsString, ElevenLabsClient.JsonSerializationOptions);
        }
    }
}
