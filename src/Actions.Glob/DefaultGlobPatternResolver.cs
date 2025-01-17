﻿// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Actions.Glob;

/// <inheritdoc cref="IGlobPatternResolver" />
internal sealed class DefaultGlobPatternResolver : IGlobPatternResolver
{
    private readonly IEnumerable<string> _includePatterns = Enumerable.Empty<string>();
    private readonly IEnumerable<string> _excludePatterns = Enumerable.Empty<string>();

    private DefaultGlobPatternResolver(
        IEnumerable<string> includePatterns,
        IEnumerable<string> exlcudePatterns) =>
        (_includePatterns, _excludePatterns) = (includePatterns, exlcudePatterns);

    /// <inheritdoc />
    internal static IGlobPatternResolver Factory(
        IEnumerable<string> includePatterns,
        IEnumerable<string> exlcudePatterns) =>
        new DefaultGlobPatternResolver(includePatterns, exlcudePatterns);

    /// <inheritdoc />
    IEnumerable<string> IGlobPatternResolver.GetGlobFiles(
        string? directory) =>
        directory.GetGlobFiles(
            _includePatterns, 
            _excludePatterns);

    /// <inheritdoc />
    GlobResult IGlobPatternResolver.GetGlobResult(
        string? directory) =>
        directory.GetGlobResult(
            _includePatterns, 
            _excludePatterns);
}
