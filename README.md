# GitHub Actions Workflow .NET SDK

[![build-and-test](https://github.com/IEvangelist/dotnet-github-actions-sdk/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/IEvangelist/dotnet-github-actions-sdk/actions/workflows/build-and-test.yml)

The .NET equivalent of the official GitHub [actions/toolkit](https://github.com/actions/toolkit) `@actions/core` project.

## Usage

To use the `ICoreService` in your .NET project, you can install the `GitHub.Actions` N.Get package. From an `IServiceCollection` instance, call `AddGitHubActions` and then your consuming code can require the `ICoreService` via constructor dependency injection.

This was modified, but borrowed from the [_core/README.md_](https://github.com/actions/toolkit/blob/main/packages/core/README.md).

## `Microsoft.GitHub.Actions`

> Core functions for setting results, logging, registering secrets and exporting variables across actions

## Usage

### Using declarations

```csharp
global using Microsoft.GitHub.Actions;
```

#### Inputs/Outputs

Action inputs can be read with `GetInput` which returns a `string` or `GetBoolInput` which parses a `bool` based on the [yaml 1.2 specification](https://yaml.org/spec/1.2/spec.html#id2804923). If `required` is `false`, the input should have a default value in `action.yml`.

Outputs can be set with `SetOutputAsync` which makes them available to be mapped into inputs of other actions to ensure they are decoupled.

```csharp
var myInput = core.GetInput("inputName", new InputOptions(true));
var myBoolInput = core.GetBoolInput("boolInputName", new InputOptions(true));
var myMultilineInput = core.GetMultilineInput("multilineInputName", new InputOptions(true));
await core.SetOutputAsync("outputKey", "outputVal");
```

#### Exporting variables

Since each step runs in a separate process, you can use `ExportVariableAsync` to add it to this step and future steps environment blocks.

```csharp
await core.ExportVariableAsync("envVar", "Val");
```

#### Setting a secret

Setting a secret registers the secret with the runner to ensure it is masked in logs.

```csharp
core.SetSecret("myPassword");
```

#### PATH Manipulation

To make a tool's path available in the path for the remainder of the job (without altering the machine or containers state), use `AddPathAsync`.  The runner will prepend the path given to the jobs PATH.

```csharp
await core.AddPathAsync("/path/to/mytool");
```

#### Exit codes

You should use this library to set the failing exit code for your action.  If status is not set and the script runs to completion, that will lead to a success.

```csharp
using var services = new ServiceCollection()
    .AddGitHubActions()
    .BuildServiceProvider();

var core = services.GetRequiredService<ICoreService>();

try 
{
    // Do stuff
}
catch (Exception ex)
{
  // SetFailed logs the message and sets a failing exit code
  core.SetFailed($"Action failed with error {ex}"");
}
```

#### Logging

Finally, this library provides some utilities for logging. Note that debug logging is hidden from the logs by default. This behavior can be toggled by enabling the [Step Debug Logs](../../docs/action-debugging.md#step-debug-logs).

```csharp
using var services = new ServiceCollection()
    .AddGitHubActions()
    .BuildServiceProvider();

var core = services.GetRequiredService<ICoreService>();

var myInput = core.GetInput("input");
try
{
    core.Debug("Inside try block");
    
    if (!myInput)
    {
        core.Warning("myInput was not set");
    }
    
    if (core.IsDebug)
    {
        // curl -v https://github.com
    }
    else
    {
        // curl https://github.com
    }
    
    // Do stuff
    core.Info("Output to the actions build log");
    
    core.Notice("This is a message that will also emit an annotation");
}
catch (Exception ex)
{
    core.Error($"Error {ex}, action may still succeed though");
}
```

This library can also wrap chunks of output in foldable groups.

```csharp
using var services = new ServiceCollection()
    .AddGitHubActions()
    .BuildServiceProvider();

var core = services.GetRequiredService<ICoreService>();

// Manually wrap output
core.StartGroup("Do some function");
doSomeFunction();
core.EndGroup();

// Wrap an asynchronous function call
var result = await core.GroupAsync("Do something async", async () => {
  var response = await DoSomeHttpRequestAsync();
  return response
});
```

#### Styling output

Colored output is supported in the Action logs via standard [ANSI escape codes](https://en.wikipedia.org/wiki/ANSI_escape_code). 3/4 bit, 8 bit and 24 bit colors are all supported.

Foreground colors:

```csharp
// 3/4 bit
core.Info("\u001b[35mThis foreground will be magenta");

// 8 bit
core.Info("\u001b[38;5;6mThis foreground will be cyan");

// 24 bit
core.Info("\u001b[38;2;255;0;0mThis foreground will be bright red");
```

Background colors:

```csharp
// 3/4 bit
core.Info("\u001b[43mThis background will be yellow");

// 8 bit
core.Info("\u001b[48;5;6mThis background will be cyan");

// 24 bit
core.Info("\u001b[48;2;255;0;0mThis background will be bright red");
```

Special styles:

```csharp
core.Info("\u001b[1mBold text");
core.Info("\u001b[3mItalic text");
core.Info("\u001b[4mUnderlined text");
```

ANSI escape codes can be combined with one another:

```csharp
core.Info("\u001b[31;46mRed foreground with a cyan background and \u001b[1mbold text at the end");
```
> Note: Escape codes reset at the start of each line.

```csharp
core.Info("\u001b[35mThis foreground will be magenta");
core.Info("This foreground will reset to the default");
```
