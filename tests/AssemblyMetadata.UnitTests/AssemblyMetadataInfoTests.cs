using Xunit;
using FluentAssertions;
using System;

namespace BenjaminAbt.AssemblyMetadata.UnitTests
{
    /// <summary>
    /// Tests for AssemblyMetadata
    /// </summary>
    public class AssemblyMetadataInfoTests
    {
        /// <summary>
        /// Tests for AssemblyMetadata.BuildInfo
        /// </summary>
        [Fact]
        public void BuildInfoTests()
        {

            DateTimeOffset buildOn = DateTimeOffset.ParseExact(AssemblyMetadataInfo.BuildInfo.BuildTimestamp, "o", null);

            AssemblyMetadataInfo.BuildInfo.BuildFileTimeUtc.Should().Be(buildOn.ToFileTime());

            AssemblyMetadataInfo.BuildInfo.BuildDateYear.Should().Be(buildOn.Year);
            AssemblyMetadataInfo.BuildInfo.BuildDateMonth.Should().Be(buildOn.Month);
            AssemblyMetadataInfo.BuildInfo.BuildDateDay.Should().Be(buildOn.Day);

            AssemblyMetadataInfo.BuildInfo.BuildTimeHour.Should().Be(buildOn.Hour);
            AssemblyMetadataInfo.BuildInfo.BuildTimeMinute.Should().Be(buildOn.Minute);
            AssemblyMetadataInfo.BuildInfo.BuildTimeSecond.Should().Be(buildOn.Second);
        }
    }

}
