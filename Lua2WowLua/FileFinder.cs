using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lua2WowLua
{
    public class FileFinder : IFileFinder
    {
        readonly string _rootpath;

        public FileFinder(string rootpath)
        {
            _rootpath = rootpath;
        }

        public string GetNormalizedFilepath(string fileLocation)
        {
            return Path.Combine(_rootpath, fileLocation + ".lua");
        }
    }
}
