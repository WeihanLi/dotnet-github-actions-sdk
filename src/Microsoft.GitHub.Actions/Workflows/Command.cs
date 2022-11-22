﻿// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.GitHub.Actions.Workflows;

/// <summary>
/// Command format:
/// <c>::name key=value,key=value::message</c>
/// </summary>
/// <example>
/// <list type="bullet">
/// <item><c>::warning::This is the message</c></item>
/// <item><c>::set-env name=MY_VAR::some value</c></item>
/// </list>
/// </example>
internal readonly record struct Command<T>(
    string? CommandName,
    T? Message,
    IDictionary<string, string>? CommandProperties = default)
{
    const string CMD_STRING = "::";

    internal bool Conventional =>
        CommandConstants.IsConventional(CommandName);

    /// <summary>
    /// The string representation of the workflow command, i.e.; <code>::name key=value,key=value::message</code>.
    /// </summary>
    public override string ToString()
    {
        StringBuilder builder = new($"{CMD_STRING}{CommandName}");

        if (CommandProperties?.Any() ?? false)
        {
            foreach (var (first, key, value)
                in CommandProperties.Select((kvp, i) => (i == 0, kvp.Key, kvp.Value)))
            {
                if (!first)
                {
                    builder.Append(',');
                }
                builder.Append($" {key}={EscapeProperty(value)}");
            }
        }

        builder.Append($"{CMD_STRING}{EscapeData(Message)}");

        return builder.ToString();
    }

    static string EscapeData<TSource>(TSource? value) =>
        value.ToCommandValue()
            .Replace("%", "%25")
            .Replace("\r", "%0D")
            .Replace("\n", "%0A");

    static string EscapeProperty<TSource>(TSource? value) =>
        value.ToCommandValue()
            .Replace("%", "%25")
            .Replace("\r", "%0D")
            .Replace("\n", "%0A")
            .Replace(":", "%3A")
            .Replace(",", "%2C");
}
