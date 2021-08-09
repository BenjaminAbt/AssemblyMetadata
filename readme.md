# AssemblyMetadata

Hi,

I'm AssemblyMetadata by Benjamin Abt. I'm a small sample how to use Source Code Generators with .NET.
In this case, this example just adds the local timestamp on build into a static class. You can use it to show your users when your application was built.

## NuGet

| NuGet |
|-|
| [![AssemblyMetadata](https://img.shields.io/nuget/v/AssemblyMetadata.svg?logo=nuget&label=AssemblyMetadata)](https://www.nuget.org/packages/AssemblyMetadata) |

```xml
   <PackageReference Include="AssemblyMetadata" 
      Version="VERSIONHERE" 
      OutputItemType="Analyzer" 
      ReferenceOutputAssembly="false" />
```

## Usage

Right now, only one sub class is offered.

### Build Time

The build time is provided as ISO 8601 string.

```csharp
string timeIso8601 = AssemblyMetadataInfo.BuildInfo.BuildTimestamp;
```

You can use the format "o" to parse the string into DateTimeOffset.

```csharp
DateTimeOffset buildOn = DateTimeOffset.ParseExact(AssemblyMetadataInfo.BuildInfo.BuildTimestamp, "o", null);
```

Also, the Source Code Generator adds the the build time as digits, so you can just concat the values instead to parse the string.

```csharp
int day = AssemblyMetadataInfo.BuildInfo.BuildTimeDay;
int month = AssemblyMetadataInfo.BuildInfo.BuildTimeMonth;
int hour = AssemblyMetadataInfo.BuildInfo.BuildTimeYear;
```

Have fun!

## Thank you

Please donate - if possible - to necessary institutions of your choice such as child cancer aid, children's hospices etc. Thanks!

