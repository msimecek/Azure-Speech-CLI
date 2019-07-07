# Azure Speech CLI

Unofficial CLI tool for [Microsoft Azure Speech Service](https://docs.microsoft.com/azure/cognitive-services/speech-service/overview) management - datasets, models, tests, endpoints etc. Useful especially for automation.

![Build status](https://dev.azure.com/msimecek/AzureSpeechCLI/_apis/build/status/AzureSpeechCLI-GitHub)

## Speech API

This tool is using [Speech Services API v2.0](https://westus.cris.ai/swagger/ui/index). SDK was generated automatically from the Swagger definition using [AutoRest](https://github.com/Azure/AutoRest), but a few adjustments had to be made to the generated code.

**Until this is refactored, it's not safe to regenerate the SDK with AutoRest.**

## Usage

**Now available as dotnet tool!** With .NET Core installed just run:

```bash
dotnet tool install -g azurespeechcli
```

Alternatively, you can go to [Releases](https://github.com/msimecek/Azure-Speech-CLI/releases) and download a compiled version for your operating system, or build directly from sources.

> CLI is created with .NET Core and builds are currently running for Windows, MacOS and Linux.
>

### Configuration

Before using the tool, you need to set your Speech Service credentials.

```
speech config set --name Project1 --key ABCD12345 --region northeurope --select
```

Or shorter version:

```
speech config set -n Project2 -k ABCD54321 -r westus -s
```

Both commands store your credentials as configuration set and automatically make these credentials selected (by using the `--select` parameter). You can have multiple sets and switch between them:

```
speech config select Project1
```

This can be useful when you work with multiple subscriptions.

### Help

If you're not sure what commands and parameters are available, try adding `--help` to the command you want to use.

For example:

```
speech --help
speech dataset --help
speech dataset create --help
```

### Entity operations

Every entity supports basic set of operations:

* `create`
* `list`
* `show`
* `delete`

When working with a specific entity, ID is usually required:

```
speech dataset show <GUID>
speech model delete <GUID>
```

### Wait

Every *create* command offers optional `--wait` (`-w`) flag which makes the CLI block and wait for the create operation to complete (dataset processed, model trained, endpoint provisioned etc.). When new entity is created, it writes corresponding ID to console.

This is useful in automation pipelines when commands are run as individual steps in a complex process.

```
speech dataset create --name CLI --audio "C:\Test.zip" --transcript "C:\test.txt" --wait
Uploading acoustic dataset...
Processing [..............]
c34d53e4-oooo-48d5-b18f-7492332f287c
```

## Commands

### Compile

After setting your subscription key and endpoint you usually start by preparing data. CLI can help by providing the `compile` command.

```
speech compile --audio <source folder> --transcript <txt file> --output <target folder> --test-percentage 10
```

This command expects a folder with all audio samples as WAV files and TXT file with corresponding transcripts.

It creates the output folder, divides data in two sets ("train" and "test") a compresses them into ZIP files. At the end you will get:

* Train.zip
* train.txt
* Test.zip
* test.txt

### Datasets

There are three types of datasets in the Speech Service: **acoustic**, **language** and **pronunciation**.

To create acoustic dataset, you need to provide a ZIP file with all audio samples and TXT file with corresponding transcriptions.

To create language and pronunciation datasets, you need to provide TXT file with language data.

To **create an acoustic dataset** use:

```
dataset create --name CLI --audio "C:\Train.zip" --transcript "C:\train.txt" --wait
```

To **create a language dataset** use:

```
dataset create --name CLI-Lang --language "C:\language.txt" --wait
```

To create a **pronunciation dataset** use:

```
dataset create --name CLI-Pro --pronunciation "C:\pronunciation.txt" --wait
```

To **list available datasets**:

```
dataset list
```

To **show details of dataset**:

```
dataset show 63f20d88-f531-4af0-bc85-58e0e9dAAACCDD
```

To **show available locales**:

```
dataset locales acoustic
dataset locales language
dataset locales pronunciation
```

### Base models

Similarly to datasets there are two types of models in the Speech Service: acoustic and language. Both are created from previously uploaded datasets.

To **create an acoustic model** you first need to get GUID of base model (referred to as *scenario*):

```
model list-scenarios --locale en-us
```

*`en-us` is the default locale, but you can choose a different one.*

The list will be order by model date - it's recommended to use the newest. Beware that GUIDs can vary between datacenters!

Scenarios can be filtered by **purpose**. Possible values are:

* `OnlineTranscription`
* `BatchTranscription`
* `LanguageAdaptation`
* `AcousticAdaptation`
* `LanguageOnlineInterpolation`

Default is `AcousticAdaptation`, because that's the type you will use when creating custom speech models.

```
model list-scenarios --purpose AcousticAdaptation
```

To disable filtering by purpose, use `--purpose all`.

Output:

```
Getting scenarios...
042de52f-d7b7-489d-921c-1b0a59d89dd1     v4.5 Unified 11. 1. 2019 13:38:33
7a1d51ce-a26d-4ee3-aee8-0fa020a65086     v3.3 Unified 26. 11. 2018 19:59:20
5d3b7fb0-f493-4e97-9616-e20130327304     v3.2 Unified 26. 11. 2018 18:59:20
d36f6c4b-8f75-41d1-b126-c38e46a059af     Unified V3 EMBR - ULM 2. 8. 2018 15:12:17
c7a69da3-27de-4a4b-ab75-b6716f6321e5 V2.5 Conversational (AM/LM adapt) 16. 4. 2018 11:55:00
a1f8db59-40ff-4f0e-b011-37629c3a1a53 V2.0 Conversational (AM/LM adapt) - Deprecated 17. 8. 2017 12:00:00
cc7826ac-5355-471d-9bc6-a54673d06e45 V1.0 Conversational (AM/LM adapt) - Deprecated 4. 11. 2016 12:01:02
a3d8aab9-6f36-44cd-9904-b37389ce2bfa V1.0 Interactive (AM/LM adapt) - Deprecated 4. 11. 2016 8:23:42
```

To use in scripts, you may want to get just a list of IDs:

```
model list-scenarios --simple
```

Output:

```
042de52f-d7b7-489d-921c-1b0a59d89dd1
7a1d51ce-a26d-4ee3-aee8-0fa020a65086
5d3b7fb0-f493-4e97-9616-e20130327304
d36f6c4b-8f75-41d1-b126-c38e46a059af
c7a69da3-27de-4a4b-ab75-b6716f6321e5
a1f8db59-40ff-4f0e-b011-37629c3a1a53
cc7826ac-5355-471d-9bc6-a54673d06e45
a3d8aab9-6f36-44cd-9904-b37389ce2bfa
```

### Models

Then you can use GUID of selected **scenario** (see [Base models](#base-models)) in the `create` command:

```
model create --name CLI --locale en-us --audio-dataset <GUID> --scenario c7a69da3-27de-4a4b-ab75-b6716f6321e5 --wait
```

To **create a language model** you need the same scenario GUID and then call:

```
model create --name CLI-Lang --locale en-us --language-dataset <GUID> --scenario c7a69da3-27de-4a4b-ab75-b6716f6321e5 --wait
```

**Pronunciation models** work the same, just provide ID of the pronunciation dataset:

```
model create --name CLI-Pro --locale en-us --pronunciation-dataset <GUID> --scenario c7a69da3-27de-4a4b-ab75-b6716f6321e5 --wait
```

To **show available locales**:

```
model locales acoustic
model locales language
```

### Tests

To **create an accuracy test** you need three GUIDs: testing audio dataset ID, ID of the acoustic model you are testing and ID of a language model:

```
speech test create --name CLI --audio-dataset <GUID> --model <GUID> --language-model <GUID> --wait
```

To see the **detail of particular test**, call:

```
speech test list
...
speech test show <GUID>
```

### Endpoints

And finally, to be able to use the model, you need to create an endpoint.

To **create an endpoint** use:

```
speech endpoint create --name CLI --locale en-us --model <GUID> --language-model <GUID> --concurrent-recognitions 1 --wait
```

### Batch transcriptions

A bonus command, which doesn't revolve around entities. Batch transcription generates a transcript of long audio file with timestamps, using your custom model.

```
speech transcript create --name CLI --locale en-us --recording <URL> --model <GUID> --language <GUID> --wait
```

To include **word-level timestamps**, use the `--word-level-timestamps` (`-wt`) parameter.

To activate **diariazation** (speaker separation), use the `--diarization` (`-di`) parameter. This will also force word-level timestamps.

To include **sentiment** score, use the `--sentiment` (`-s`) parameter.

Once the batch is done, you can call:

```
speech transcript show <GUID>
```

And get result URLs from response JSON.

Or you can call **download** to get it as file:

```
speech transcript download <GUID> --out-dir <PATH> --format <format> --file-name <filename>
```

Supported output formats:

* JSON (default)
* VTT
* TXT

If you specify `--file-name` this value will be used for the output file (with extension given by format).

If you don't specify `--out-dir` current working directory will be used.

To update name or description of batch transcription, use the **update** command:

```
speech transcript update <GUID> --name <name> --description <description>
```

### Single transcription

If you want to perform transcript of a single, short WAV file, you can use single transcription command like this:

```
speech transcript single --input "C:\test.wav" --endpoint <GUID> --output-format detailed
```

## TODO

- [ ] Work with names too, not just GUIDs
- [ ] Check if uploaded files are in the correct format (UTF-8 BOM text files)

-----

By participating in this project, you
agree to abide by the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).