using System;
using BenjaminAbt.AssemblyMetadata;

namespace BenjaminAbt.AssemblyMetadata.SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(AssemblyMetadataInfo.BuildInfo.BuildTimestamp);
            Console.ReadKey();
        }
    }
}
