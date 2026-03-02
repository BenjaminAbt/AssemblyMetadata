using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Globalization;
using System.Text;

namespace BenjaminAbt.AssemblyMetadata;

/// <summary>
/// Roslyn incremental source generator that embeds build-time metadata
/// as compile-time constants into the consuming assembly.
/// </summary>
/// <remarks>
/// <para>
/// The generator registers itself against
/// <see cref="IncrementalGeneratorInitializationContext.CompilationProvider"/> so that it
/// executes on every compilation. Once invoked, it captures <see cref="DateTimeOffset.UtcNow"/>
/// and emits a file named <c>AssemblyMetadataInfo.gen.cs</c> that defines the internal class
/// <c>BenjaminAbt.AssemblyMetadata.AssemblyMetadataInfo.BuildInfo</c> with eight
/// <c>public const</c> members.
/// </para>
/// <para>
/// Because all output members are <c>const</c>, the C# compiler inlines them at every call site.
/// There is therefore zero runtime overhead: no method call, no property lookup, no heap
/// allocation. The source generator itself is shipped as an analyzer-only NuGet package, so it
/// adds <strong>no assembly reference</strong> to the consuming project's runtime dependency graph.
/// </para>
/// <para>
/// The generator uses the modern <see cref="IIncrementalGenerator"/> API (introduced in
/// Roslyn 4) rather than the legacy <c>ISourceGenerator</c> API. The incremental pipeline ensures
/// the generator only re-executes when the compilation inputs change, keeping build times fast.
/// </para>
/// </remarks>
[Generator]
public sealed class AssemblyMetadataGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Initializes the incremental generator pipeline by registering the source-production
    /// callback against the compilation provider.
    /// </summary>
    /// <param name="context">
    /// The <see cref="IncrementalGeneratorInitializationContext"/> provided by the Roslyn host.
    /// Used to register source-production callbacks and to access incremental value providers.
    /// </param>
    /// <remarks>
    /// This implementation registers against
    /// <see cref="IncrementalGeneratorInitializationContext.CompilationProvider"/> because the
    /// generated output depends solely on the current wall-clock time - captured at generation
    /// time - not on any specific compilation symbol or project property. The callback
    /// <see cref="Execute"/> is therefore invoked for every new compilation.
    /// </remarks>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register against the compilation provider so the generator
        // fires on every new compilation (i.e., on every build).
        context.RegisterSourceOutput(context.CompilationProvider, Execute);
    }

    /// <summary>
    /// Source-production callback invoked by Roslyn for each new compilation.
    /// Captures the current UTC time and delegates source emission to <see cref="BuildSource"/>.
    /// </summary>
    /// <param name="context">
    /// The <see cref="SourceProductionContext"/> that accepts the generated source file and
    /// allows reporting diagnostics back to the compiler.
    /// </param>
    /// <param name="compilation">
    /// The current <see cref="Compilation"/>. Not actively used by this generator because the
    /// emitted output depends solely on the build time, not on project symbols.
    /// </param>
    /// <remarks>
    /// The UTC timestamp is captured fresh on every invocation so that each build produces
    /// an accurate, up-to-date build timestamp. The generated file is added via
    /// <see cref="SourceProductionContext.AddSource(string, SourceText)"/> with the hint name
    /// <c>AssemblyMetadataInfo.gen.cs</c>.
    /// </remarks>
    private static void Execute(SourceProductionContext context, Compilation compilation)
    {
        // Capture the current UTC time at the moment the generator runs.
        // Using DateTimeOffset (not DateTime) preserves the UTC offset in the ISO 8601 string.
        DateTimeOffset buildOn = DateTimeOffset.UtcNow;
        string source = BuildSource(buildOn);
        context.AddSource("AssemblyMetadataInfo.gen.cs", SourceText.From(source, Encoding.UTF8));
    }

    /// <summary>
    /// Builds the complete C# source text for <c>AssemblyMetadataInfo.gen.cs</c>
    /// from the provided <paramref name="buildOn"/> timestamp.
    /// </summary>
    /// <param name="buildOn">
    /// The UTC timestamp captured at source-generation time. All eight generated constants are
    /// derived exclusively from this single value to guarantee internal consistency across
    /// <c>BuildTimestamp</c>, <c>BuildFileTimeUtc</c>, and the individual date/time components.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> containing a complete, valid C# compilation unit that defines
    /// <c>BenjaminAbt.AssemblyMetadata.AssemblyMetadataInfo.BuildInfo</c>
    /// with eight <c>public const</c> members.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The timestamp is formatted with the <c>"o"</c> (round-trip) format specifier so that
    /// it can be losslessly parsed back into a <see cref="DateTimeOffset"/> with full
    /// sub-second precision using
    /// <c>DateTimeOffset.ParseExact(value, "o", null)</c>.
    /// </para>
    /// <para>
    /// All numeric values are formatted with <see cref="CultureInfo.InvariantCulture"/> to
    /// guarantee culture-neutral, locale-independent output regardless of the build machine's
    /// regional settings.
    /// </para>
    /// <para>
    /// A <see cref="StringBuilder"/> is used to compose the source text in a single pass,
    /// avoiding repeated intermediate string allocations. The generated file begins with an
    /// <c>// &lt;auto-generated /&gt;</c> header so that Roslyn and other tooling recognise it
    /// as machine-generated and suppress additional analyzer warnings on the output.
    /// </para>
    /// </remarks>
    private static string BuildSource(DateTimeOffset buildOn)
    {
        // Format the UTC timestamp as an ISO 8601 round-trip string
        // (e.g. "2026-03-02T14:35:07.1234567+00:00").
        // The "o" specifier preserves the full DateTimeOffset including the +00:00 UTC offset.
        string iso8601 = buildOn.ToString("o", CultureInfo.InvariantCulture);

        // Convert to Windows FileTime (100-nanosecond intervals since 1601-01-01T00:00:00Z).
        // This allows callers to reconstruct a DateTimeOffset without any string parsing:
        //   DateTimeOffset.FromFileTime(AssemblyMetadataInfo.BuildInfo.BuildFileTimeUtc)
        long fileTime = buildOn.ToFileTime();

        return new StringBuilder()
            .AppendLine("// <auto-generated />")
            .AppendLine("namespace BenjaminAbt.AssemblyMetadata")
            .AppendLine("{")
            .AppendLine("    internal static class AssemblyMetadataInfo")
            .AppendLine("    {")
            .AppendLine("        /// <summary>Contains compile-time build metadata constants.</summary>")
            .AppendLine("        internal static class BuildInfo")
            .AppendLine("        {")
            .AppendLine("            /// <summary>Build time as UTC ISO 8601 string.</summary>")
            .AppendLine($"            public const string BuildTimestamp = \"{iso8601}\";")
            .AppendLine()
            .AppendLine("            /// <summary>Build time as Windows FileTime (100-ns intervals since 1601-01-01 UTC).</summary>")
            .AppendLine($"            public const long BuildFileTimeUtc = {fileTime}L;")
            .AppendLine()
            .AppendLine("            /// <summary>Year component of the build date (UTC).</summary>")
            .AppendLine($"            public const int BuildDateYear = {buildOn.Year.ToString(CultureInfo.InvariantCulture)};")
            .AppendLine()
            .AppendLine("            /// <summary>Month component of the build date (UTC).</summary>")
            .AppendLine($"            public const int BuildDateMonth = {buildOn.Month.ToString(CultureInfo.InvariantCulture)};")
            .AppendLine()
            .AppendLine("            /// <summary>Day component of the build date (UTC).</summary>")
            .AppendLine($"            public const int BuildDateDay = {buildOn.Day.ToString(CultureInfo.InvariantCulture)};")
            .AppendLine()
            .AppendLine("            /// <summary>Hour component of the build time (UTC, 24-hour).</summary>")
            .AppendLine($"            public const int BuildTimeHour = {buildOn.Hour.ToString(CultureInfo.InvariantCulture)};")
            .AppendLine()
            .AppendLine("            /// <summary>Minute component of the build time (UTC).</summary>")
            .AppendLine($"            public const int BuildTimeMinute = {buildOn.Minute.ToString(CultureInfo.InvariantCulture)};")
            .AppendLine()
            .AppendLine("            /// <summary>Second component of the build time (UTC).</summary>")
            .AppendLine($"            public const int BuildTimeSecond = {buildOn.Second.ToString(CultureInfo.InvariantCulture)};")
            .AppendLine("        }")
            .AppendLine("    }")
            .AppendLine("}")
            .ToString();
    }
}
