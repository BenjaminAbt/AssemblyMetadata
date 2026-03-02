using BenjaminAbt.AssemblyMetadata;

// Display all build metadata constants embedded by the AssemblyMetadata source generator
Console.WriteLine($"Build Timestamp : {AssemblyMetadataInfo.BuildInfo.BuildTimestamp}");
Console.WriteLine($"Build Date      : {AssemblyMetadataInfo.BuildInfo.BuildDateYear}-{AssemblyMetadataInfo.BuildInfo.BuildDateMonth:D2}-{AssemblyMetadataInfo.BuildInfo.BuildDateDay:D2}");
Console.WriteLine($"Build Time      : {AssemblyMetadataInfo.BuildInfo.BuildTimeHour:D2}:{AssemblyMetadataInfo.BuildInfo.BuildTimeMinute:D2}:{AssemblyMetadataInfo.BuildInfo.BuildTimeSecond:D2}");
Console.WriteLine($"Build FileTime  : {AssemblyMetadataInfo.BuildInfo.BuildFileTimeUtc}");
