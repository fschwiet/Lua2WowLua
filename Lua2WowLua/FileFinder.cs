using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lua2WowLua
{
    public class FileFinder : IFileFinder
    {
        readonly IEnumerable<string> _includes;

        public FileFinder(IEnumerable<string> includes)
        {
            _includes = includes;
        }

        public string GetNormalizedFilepath(string fileLocation)
        {
            foreach(var include in _includes)
            {
                var potentialFile = Path.Combine(include, fileLocation + ".lua");
                if (File.Exists(potentialFile))
                    return potentialFile;
            }

            throw new FileNotFoundException("Could not locate file referenced as '{" + fileLocation + "}'.");
        }
    }
}
