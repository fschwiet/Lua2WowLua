using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lua2WowLua
{
    public interface IFileFinder
    {
        Stream Get(string fileLocation);
    }
}
