using Xunit;
using BenjaminAbt.AssemblyMetadata;
using FluentAssertions;
using System;

namespace BenjaminAbt.AssemblyMetadata.UnitTests
{
    public class AssemblyMetadataInfoTests
    {
        [Fact]
        public void BuildInfoTests()
        {
            DateTimeOffset buildOn = DateTimeOffset.ParseExact(AssemblyMetadataInfo.BuildInfo.BuildTimestamp, "o", null);

            AssemblyMetadataInfo.BuildInfo.BuildDateYear.Should().Be(buildOn.Year);
            AssemblyMetadataInfo.BuildInfo.BuildDateMonth.Should().Be(buildOn.Month);
            AssemblyMetadataInfo.BuildInfo.BuildDateDay.Should().Be(buildOn.Day);

            AssemblyMetadataInfo.BuildInfo.BuildTimeHour.Should().Be(buildOn.Hour);
            AssemblyMetadataInfo.BuildInfo.BuildTimeMinute.Should().Be(buildOn.Minute);
            AssemblyMetadataInfo.BuildInfo.BuildTimeSecond.Should().Be(buildOn.Second);
        }
    }
}
