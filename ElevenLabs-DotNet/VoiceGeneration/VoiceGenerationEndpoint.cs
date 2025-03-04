// Licensed under the MIT License. See LICENSE in the project root for license information.

using ElevenLabs.Extensions;
using ElevenLabs.Voices;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ElevenLabs.VoiceGeneration
{
    public sealed class VoiceGenerationEndpoint : BaseEndPoint
    {
        public VoiceGenerationEndpoint(ElevenLabsClient api) : base(api) { }

        protected override string Root => "voice-generation";

        /// <summary>
        /// Gets the available voice generation options.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="GeneratedVoiceOptions"/>.</returns>
        public async Task<GeneratedVoiceOptions> GetVoiceGenerationOptionsAsync(CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl("/generate-voice/parameters"), cancellationToken);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken);
            return JsonSerializer.Deserialize<GeneratedVoiceOptions>(responseAsString, ElevenLabsClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Generate a <see cref="Voice"/>.
        /// </summary>
        /// <param name="generatedVoicePreviewRequest"><see cref="GeneratedVoicePreviewRequest"/></param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>Tuple with generated voice id and audio data.</returns>
        public async Task<Tuple<string, ReadOnlyMemory<byte>>> GenerateVoicePreviewAsync(GeneratedVoicePreviewRequest generatedVoicePreviewRequest, CancellationToken cancellationToken = default)
        {
            var payload = JsonSerializer.Serialize(generatedVoicePreviewRequest, ElevenLabsClient.JsonSerializationOptions).ToJsonStringContent();
            var response = await Api.Client.PostAsync(GetUrl("/generate-voice"), payload, cancellationToken);
            await response.CheckResponseAsync(cancellationToken);
            var generatedVoiceId = response.Headers.FirstOrDefault(pair => pair.Key == "generated_voice_id").Value.FirstOrDefault();
            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            await using var memoryStream = new MemoryStream();
            int bytesRead;
            var totalBytesRead = 0;
            var buffer = new byte[8192];

            while ((bytesRead = await responseStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
            {
                await memoryStream.WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, bytesRead), cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
            }

            return new Tuple<string, ReadOnlyMemory<byte>>(generatedVoiceId, new ReadOnlyMemory<byte>(memoryStream.GetBuffer(), 0, totalBytesRead));
        }

        /// <summary>
        /// Clone a <see cref="Voice"/>.
        /// </summary>
        /// <param name="createVoiceRequest"><see cref="CreateVoiceRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Voice"/>.</returns>
        public async Task<Voice> CreateVoiceAsync(CreateVoiceRequest createVoiceRequest, CancellationToken cancellationToken = default)
        {
            var payload = JsonSerializer.Serialize(createVoiceRequest).ToJsonStringContent();
            var response = await Api.Client.PostAsync(GetUrl("/create-voice"), payload, cancellationToken);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken);
            return JsonSerializer.Deserialize<Voice>(responseAsString, ElevenLabsClient.JsonSerializationOptions);
        }
    }
}
