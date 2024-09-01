using Dassie.Extensions;
using System;

namespace Dassie.Preprocessor;

public class Extension : IPackage
{
    public PackageMetadata Metadata { get; } = new()
    {
        Author = "jgh07",
        Description = "A tool for preprocessing any file with embedded Dassie source cdoe.",
        Name = "Dassie.Preprocessor",
        Version = new(1, 0, 0, 0)
    };

    public Type[] Commands { get; } = [typeof(PreprocessCommand)];
}