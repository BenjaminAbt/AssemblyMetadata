using System;
using BenjaminAbt.AssemblyMetadata;

namespace BenjaminAbt.AssemblyMetadata.SampleApp
{
    /// <summary>
    /// Sample Program
    /// </summary>
    class Program
    {
        /// <summary>
        /// Sample Main
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine(AssemblyMetadataInfo.BuildInfo.BuildTimestamp);
            Console.ReadKey();
        }
    }
}
