# ElevenLabs-DotNet

[![Discord](https://img.shields.io/discord/855294214065487932.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/xQgMW9ufN4)
[![NuGet version (ElevenLabs-DotNet)](https://img.shields.io/nuget/v/ElevenLabs-DotNet.svg)](https://www.nuget.org/packages/ElevenLabs-DotNet/)
[![NuGet version (ElevenLabs-DotNet-Proxy)](https://img.shields.io/nuget/v/ElevenLabs-DotNet-Proxy.svg?label=ElevenLabs-DotNet-Proxy&logo=nuget)](https://www.nuget.org/packages/ElevenLabs-DotNet-Proxy/)
[![Nuget Publish](https://github.com/RageAgainstThePixel/ElevenLabs-DotNet/actions/workflows/Publish-Nuget.yml/badge.svg)](https://github.com/RageAgainstThePixel/ElevenLabs-DotNet/actions/workflows/Publish-Nuget.yml)

A non-official [Eleven Labs](https://elevenlabs.io/) voice synthesis RESTful client.

I am not affiliated with Eleven Labs and an account with api access is required.

***All copyrights, trademarks, logos, and assets are the property of their respective owners.***

## Getting started

### Install from NuGet

Install package [`ElevenLabs` from Nuget](https://www.nuget.org/packages/ElevenLabs-DotNet/).  Here's how via command line:

```powershell
Install-Package ElevenLabs-DotNet
```

> Looking to [use ElevenLabs in the Unity Game Engine](https://github.com/RageAgainstThePixel/com.rest.elevenlabs)? Check out our unity package on OpenUPM:
>
>[![openupm](https://img.shields.io/npm/v/com.rest.elevenlabs?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.rest.elevenlabs/)

---

## Documentation

### Table of Contents

- [Authentication](#authentication)
- [API Proxy](#api-proxy)
- [Text to Speech](#text-to-speech)
  - [Stream Text To Speech](#stream-text-to-speech) :new:
- [Voices](#voices)
  - [Get All Voices](#get-all-voices)
  - [Get Default Voice Settings](#get-default-voice-settings)
  - [Get Voice](#get-voice)
  - [Edit Voice Settings](#edit-voice-settings)
  - [Add Voice](#add-voice)
  - [Edit Voice](#edit-voice)
  - [Delete Voice](#delete-voice)
  - [Samples](#samples)
    - [Download Voice Sample](#download-voice-sample) :new:
    - [Delete Voice Sample](#delete-voice-sample)
- [History](#history)
  - [Get History](#get-history)
  - [Get History Item](#get-history-item)
  - [Download History Audio](#download-history-audio) :new:
  - [Download History Items](#download-history-items) :new:
  - [Delete History Item](#delete-history-item)
- [User](#user)
  - [Get User Info](#get-user-info)
  - [Get Subscription Info](#get-subscription-info)

### Authentication

There are 3 ways to provide your API keys, in order of precedence:

1. [Pass keys directly with constructor](#pass-keys-directly-with-constructor)
2. [Load key from configuration file](#load-key-from-configuration-file)
3. [Use System Environment Variables](#use-system-environment-variables)

#### Pass keys directly with constructor

```csharp
var api = new ElevenLabsClient("yourApiKey");
```

Or create a `ElevenLabsAuthentication` object manually

```csharp
var api = new ElevenLabsClient(new ElevenLabsAuthentication("yourApiKey"));
```

#### Load key from configuration file

Attempts to load api keys from a configuration file, by default `.elevenlabs` in the current directory, optionally traversing up the directory tree or in the user's home directory.

To create a configuration file, create a new text file named `.elevenlabs` and containing the line:

##### Json format

```json
{
  "apiKey": "yourApiKey",
}
```

You can also load the file directly with known path by calling a static method in Authentication:

```csharp
var api = new ElevenLabsClient(ElevenLabsAuthentication.LoadFromDirectory("your/path/to/.elevenlabs"));;
```

#### Use System Environment Variables

Use your system's environment variables specify an api key to use.

- Use `ELEVEN_LABS_API_KEY` for your api key.

```csharp
var api = new ElevenLabsClient(ElevenLabsAuthentication.LoadFromEnv());
```

### [API Proxy](ElevenLabs-DotNet-Proxy/Readme.md)

[![NuGet version (ElevenLabs-DotNet-Proxy)](https://img.shields.io/nuget/v/ElevenLabs-DotNet-Proxy.svg?label=ElevenLabs-DotNet-Proxy&logo=nuget)](https://www.nuget.org/packages/ElevenLabs-DotNet-Proxy/)

Using either the [ElevenLabs-DotNet](https://github.com/RageAgainstThePixel/ElevenLabs-DotNet) or [com.rest.elevenlabs](https://github.com/RageAgainstThePixel/com.rest.elevenlabs) packages directly in your front-end app may expose your API keys and other sensitive information. To mitigate this risk, it is recommended to set up an intermediate API that makes requests to ElevenLabs on behalf of your front-end app. This library can be utilized for both front-end and intermediary host configurations, ensuring secure communication with the ElevenLabs API.

### Front End Example

In the front end example, you will need to securely authenticate your users using your preferred OAuth provider. Once the user is authenticated, exchange your custom auth token with your API key on the backend.

Follow these steps:

1. Setup a new project using either the [ElevenLabs-DotNet](https://github.com/RageAgainstThePixel/ElevenLabs-DotNet) or [com.rest.elevenlabs](https://github.com/RageAgainstThePixel/com.rest.elevenlabs) packages.
2. Authenticate users with your OAuth provider.
3. After successful authentication, create a new `ElevenLabsAuthentication` object and pass in the custom token.
4. Create a new `ElevenLabsClientSettings` object and specify the domain where your intermediate API is located.
5. Pass your new `auth` and `settings` objects to the `ElevenLabsClient` constructor when you create the client instance.

Here's an example of how to set up the front end:

```csharp
var authToken = await LoginAsync();
var auth = new ElevenLabsAuthentication(authToken);
var settings = new ElevenLabsClientSettings(domain: "api.your-custom-domain.com");
var api = new ElevenLabsClient(auth, settings);
```

This setup allows your front end application to securely communicate with your backend that will be using the ElevenLabs-DotNet-Proxy, which then forwards requests to the ElevenLabs API. This ensures that your ElevenLabs API keys and other sensitive information remain secure throughout the process.

### Back End Example

In this example, we demonstrate how to set up and use `ElevenLabsProxyStartup` in a new ASP.NET Core web app. The proxy server will handle authentication and forward requests to the ElevenLabs API, ensuring that your API keys and other sensitive information remain secure.

1. Create a new [ASP.NET Core minimal web API](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-6.0) project.
2. Add the ElevenLabs-DotNet nuget package to your project.
    - Powershell install: `Install-Package ElevenLabs-DotNet-Proxy`
    - Manually editing .csproj: `<PackageReference Include="ElevenLabs-DotNet-Proxy" />`
3. Create a new class that inherits from `AbstractAuthenticationFilter` and override the `ValidateAuthentication` method. This will implement the `IAuthenticationFilter` that you will use to check user session token against your internal server.
4. In `Program.cs`, create a new proxy web application by calling `ElevenLabsProxyStartup.CreateDefaultHost` method, passing your custom `AuthenticationFilter` as a type argument.
5. Create `ElevenLabsAuthentication` and `ElevenLabsClientSettings` as you would normally with your API keys, org id, or Azure settings.

```csharp
public partial class Program
{
    private class AuthenticationFilter : AbstractAuthenticationFilter
    {
        public override void ValidateAuthentication(IHeaderDictionary request)
        {
            // You will need to implement your own class to properly test
            // custom issued tokens you've setup for your end users.
            if (!request["xi-api-key"].ToString().Contains(userToken))
            {
                throw new AuthenticationException("User is not authorized");
            }
        }
    }

    public static void Main(string[] args)
    {
        var client = new ElevenLabsClient();
        var proxy = ElevenLabsProxyStartup.CreateDefaultHost<AuthenticationFilter>(args, client);
        proxy.Run();
    }
}
```

Once you have set up your proxy server, your end users can now make authenticated requests to your proxy api instead of directly to the ElevenLabs API. The proxy server will handle authentication and forward requests to the ElevenLabs API, ensuring that your API keys and other sensitive information remain secure.

### [Text to Speech](https://docs.elevenlabs.io/api-reference/text-to-speech)

Convert text to speech.

```csharp
var api = new ElevenLabsClient();
var text = "The quick brown fox jumps over the lazy dog.";
var voice = (await api.VoicesEndpoint.GetAllVoicesAsync()).FirstOrDefault();
var voiceClip = await api.TextToSpeechEndpoint.TextToSpeechAsync(text, voice);
await File.WriteAllBytesAsync($"{voiceClip.Id}.mp3", voiceClip.ClipData.ToArray());
```

#### [Stream Text To Speech](https://docs.elevenlabs.io/api-reference/text-to-speech-stream)

Stream text to speech.

```csharp
var api = new ElevenLabsClient();
var text = "The quick brown fox jumps over the lazy dog.";
var voice = (await api.VoicesEndpoint.GetAllVoicesAsync()).FirstOrDefault();
string fileName = "myfile.mp3";
using var outputFileStream = File.OpenWrite(fileName);
var voiceClip = await TextToSpeechAsync(text, voice,
partialClipCallback: async (partialClip) =>
{
    // Write the incoming data to the output file stream.
    // Alternatively you can play this clip data directly.
    await outputFileStream.WriteAsync(partialClip.ClipData);
});
```

### [Voices](https://docs.elevenlabs.io/api-reference/voices)

Access to voices created either by the user or ElevenLabs.

#### Get All Voices

Gets a list of all available voices.

```csharp
var api = new ElevenLabsClient();
var allVoices = await api.VoicesEndpoint.GetAllVoicesAsync();

foreach (var voice in allVoices)
{
    Console.WriteLine($"{voice.Id} | {voice.Name} | similarity boost: {voice.Settings?.SimilarityBoost} | stability: {voice.Settings?.Stability}");
}
```

#### Get Default Voice Settings

Gets the global default voice settings.

```csharp
var api = new ElevenLabsClient();
var result = await api.VoicesEndpoint.GetDefaultVoiceSettingsAsync();
Console.WriteLine($"stability: {result.Stability} | similarity boost: {result.SimilarityBoost}");
```

#### Get Voice

```csharp
var api = new ElevenLabsClient();
var voice = await api.VoicesEndpoint.GetVoiceAsync("voiceId");
Console.WriteLine($"{voice.Id} | {voice.Name} | {voice.PreviewUrl}");
```

#### Edit Voice Settings

Edit the settings for a specific voice.

```csharp
var api = new ElevenLabsClient();
var success = await api.VoicesEndpoint.EditVoiceSettingsAsync(voice, new VoiceSettings(0.7f, 0.7f));
Console.WriteLine($"Was successful? {success}");
```

#### Add Voice

```csharp
var api = new ElevenLabsClient();
var labels = new Dictionary<string, string>
{
    { "accent", "american" }
};
var audioSamplePaths = new List<string>();
var voice = await api.VoicesEndpoint.AddVoiceAsync("Voice Name", audioSamplePaths, labels);
```

#### Edit Voice

```csharp
var api = new ElevenLabsClient();
var labels = new Dictionary<string, string>
{
    { "age", "young" }
};
var audioSamplePaths = new List<string>();
var success = await api.VoicesEndpoint.EditVoiceAsync(voice, audioSamplePaths, labels);
Console.WriteLine($"Was successful? {success}");
```

#### Delete Voice

```csharp
var api = new ElevenLabsClient();
var success = await api.VoicesEndpoint.DeleteVoiceAsync(voiceId);
Console.WriteLine($"Was successful? {success}");
```

#### [Samples](https://docs.elevenlabs.io/api-reference/samples)

Access to your samples, created by you when cloning voices.

##### Download Voice Sample

```csharp
var api = new ElevenLabsClient();
var voiceClip = await api.VoicesEndpoint.DownloadVoiceSampleAsync(voice, sample);
await File.WriteAllBytesAsync($"{voiceClip.Id}.mp3", voiceClip.ClipData.ToArray());
```

##### Delete Voice Sample

```csharp
var api = new ElevenLabsClient();
var success = await api.VoicesEndpoint.DeleteVoiceSampleAsync(voiceId, sampleId);
Console.WriteLine($"Was successful? {success}");
```

### [History](https://docs.elevenlabs.io/api-reference/history)

Access to your previously synthesized audio clips including its metadata.

#### Get History

```csharp
var api = new ElevenLabsClient();
var historyItems = await api.HistoryEndpoint.GetHistoryAsync();

foreach (var historyItem in historyItems.OrderBy(historyItem => historyItem.Date))
{
    Console.WriteLine($"{historyItem.State} {historyItem.Date} | {historyItem.Id} | {historyItem.Text.Length} | {historyItem.Text}");
}
```

#### Get History Item

```csharp
var api = new ElevenLabsClient();
var historyItem = await api.HistoryEndpoint.GetHistoryItemAsync(voiceClip.Id);
```

#### Download History Audio

```csharp
var api = new ElevenLabsClient();
var voiceClip = await api.HistoryEndpoint.DownloadHistoryAudioAsync(historyItem);
await File.WriteAllBytesAsync($"{voiceClip.Id}.mp3", voiceClip.ClipData.ToArray());
```

#### Download History Items

```csharp
var api = new ElevenLabsClient();
var historyInfo = await api.HistoryEndpoint.DownloadHistoryItemsAsync();
```

#### Delete History Item

```csharp
var api = new ElevenLabsClient();
var success = await api.HistoryEndpoint.DeleteHistoryItemAsync(historyItem);
Console.WriteLine($"Was successful? {success}");
```

### [User](https://docs.elevenlabs.io/api-reference/user)

Access to your user Information and subscription status.

#### Get User Info

Gets information about your user account with ElevenLabs.

```csharp
var api = new ElevenLabsClient();
var userInfo = await api.UserEndpoint.GetUserInfoAsync();
```

#### Get Subscription Info

Gets information about your subscription with ElevenLabs.

```csharp
var api = new ElevenLabsClient();
var subscriptionInfo = await api.UserEndpoint.GetSubscriptionInfoAsync();
```
