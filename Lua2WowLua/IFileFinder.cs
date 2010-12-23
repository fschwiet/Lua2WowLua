using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lua2WowLua
{
    public interface IFileFinder
    {
        string GetNormalizedFilepath(string fileLocation);
    }
}
