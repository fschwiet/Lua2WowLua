using System.Collections.Generic;
using System.IO;

namespace Lua2WowLua
{
    public class FileCollection : IFileFinder
    {
        Dictionary<string, Stream> _availableFiles = new Dictionary<string, Stream>();

        public void AddFile(string name, Stream file)
        {
            _availableFiles[name] = file;
        }

        public Stream Get(string fileLocation)
        {
            return _availableFiles[fileLocation];
        }
    }
}