using System;
using Xunit;

namespace BenjaminAbt.AssemblyMetadata.UnitTests;

/// <summary>
/// Integration tests that validate the <em>values</em> of the constants produced by
/// <see cref="AssemblyMetadataGenerator"/> and compiled into this assembly.
/// </summary>
/// <remarks>
/// These tests run against the actual generated output that is baked into the test binary at
/// compile time. They verify correctness of the generated values - not the generator code itself.
/// For tests that exercise the generator <em>code paths</em>, see
/// <see cref="AssemblyMetadataGeneratorTests"/>.
/// </remarks>
public class AssemblyMetadataInfoTests
{
    /// <summary>
    /// Verifies that <see cref="AssemblyMetadataInfo.BuildInfo.BuildTimestamp"/> is a valid
    /// ISO 8601 round-trip string and that all other constants are consistent with the
    /// timestamp it encodes.
    /// </summary>
    /// <remarks>
    /// Parses the stored string using the <c>"o"</c> format specifier - the same format used
    /// by the generator - and then compares each derived constant against the parsed value.
    /// A parse failure here indicates the generator emitted a malformed timestamp.
    /// </remarks>
    [Fact]
    public void BuildInfo_Timestamp_IsValidIso8601()
    {
        DateTimeOffset buildOn = DateTimeOffset.ParseExact(
            AssemblyMetadataInfo.BuildInfo.BuildTimestamp, "o", null);

        // xUnit2000: constant/expected value must be the first argument
        Assert.Equal(AssemblyMetadataInfo.BuildInfo.BuildFileTimeUtc, buildOn.ToFileTime());

        Assert.Equal(AssemblyMetadataInfo.BuildInfo.BuildDateYear, buildOn.Year);
        Assert.Equal(AssemblyMetadataInfo.BuildInfo.BuildDateMonth, buildOn.Month);
        Assert.Equal(AssemblyMetadataInfo.BuildInfo.BuildDateDay, buildOn.Day);

        Assert.Equal(AssemblyMetadataInfo.BuildInfo.BuildTimeHour, buildOn.Hour);
        Assert.Equal(AssemblyMetadataInfo.BuildInfo.BuildTimeMinute, buildOn.Minute);
        Assert.Equal(AssemblyMetadataInfo.BuildInfo.BuildTimeSecond, buildOn.Second);
    }

    /// <summary>
    /// Verifies that <see cref="AssemblyMetadataInfo.BuildInfo.BuildFileTimeUtc"/> is a
    /// positive value, confirming it represents a date after 1601-01-01T00:00:00Z.
    /// </summary>
    [Fact]
    public void BuildInfo_FileTime_IsPositive()
    {
        Assert.True(AssemblyMetadataInfo.BuildInfo.BuildFileTimeUtc > 0);
    }

    /// <summary>
    /// Verifies that each date and time component falls within its valid calendar / clock range.
    /// </summary>
    /// <remarks>
    /// The year lower bound of 2020 reflects a minimum reasonable build date for this project.
    /// </remarks>
    [Fact]
    public void BuildInfo_DateComponents_AreInValidRanges()
    {
        Assert.InRange(AssemblyMetadataInfo.BuildInfo.BuildDateYear, 2020, 2100);
        Assert.InRange(AssemblyMetadataInfo.BuildInfo.BuildDateMonth, 1, 12);
        Assert.InRange(AssemblyMetadataInfo.BuildInfo.BuildDateDay, 1, 31);

        Assert.InRange(AssemblyMetadataInfo.BuildInfo.BuildTimeHour, 0, 23);
        Assert.InRange(AssemblyMetadataInfo.BuildInfo.BuildTimeMinute, 0, 59);
        Assert.InRange(AssemblyMetadataInfo.BuildInfo.BuildTimeSecond, 0, 59);
    }

    /// <summary>
    /// Verifies that <see cref="AssemblyMetadataInfo.BuildInfo.BuildTimestamp"/> encodes a UTC
    /// offset of exactly <c>+00:00</c>, confirming the generator always uses UTC.
    /// </summary>
    [Fact]
    public void BuildInfo_Timestamp_IsUtc()
    {
        DateTimeOffset buildOn = DateTimeOffset.ParseExact(
            AssemblyMetadataInfo.BuildInfo.BuildTimestamp, "o", null);

        Assert.Equal(TimeSpan.Zero, buildOn.Offset);
    }
}

