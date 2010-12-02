using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lua2WowLua
{
    public class Generator
    {
        readonly IFileFinder _fileFinder;
        const string TabString = "    ";

        Dictionary<string, string> _loadedModules = new Dictionary<string, string>();

        public Generator(IFileFinder fileFinder)
        {
            _fileFinder = fileFinder;
        }

        public string Process(string filepath)
        {
            using (var file = File.OpenRead(filepath))
            {
                return Process(file);
            }
        }

        public string Process(Stream file)
        {
            StringBuilder result = new StringBuilder();

            ProcessFile(null, file, result, 0);

            return result.ToString();
        }

        void ProcessFile(string location, Stream file, StringBuilder result, int depth)
        {
            string line;
            var reader = new StreamReader(file);
            string fileModuleName = null;

            LinkedList<string> thisFile = new LinkedList<string>();

            while((line = reader.ReadLine()) != null)
            {
                Match require = LuaRegex.Require.Match(line);

                if (require.Success)
                {
                    var requireLocation = require.Groups["name"].Value;

                    if (!_loadedModules.ContainsKey(requireLocation))
                    {
                        using (Stream requireStream = _fileFinder.Get(requireLocation))
                        {
                            ProcessFile(requireLocation, requireStream, result, 1);
                        }
                    }

                    if (_loadedModules.ContainsKey(requireLocation))
                    {
                        string subModuleName = _loadedModules[requireLocation];

                        thisFile.AddLast("_LOADER_zz_" + subModuleName + "();");
                        thisFile.AddLast("local " + subModuleName + " = _LOADED_zz_" + subModuleName + "_env;");
                    }
 
                    continue;
                }
                else
                {
                    var fail = LuaRegex.UnhandledRequire.Match(line);

                    if (fail.Success)
                    {
                        throw new NotSupportedException("A require expression was detected that is likely not yet supported propery.  The expression was: " + line);
                    }
                }

                Match module = LuaRegex.SeeallModule.Match(line);

                if (module.Success)
                {
                    if (fileModuleName != null)
                        throw new NotSupportedException("A second module expression was found in the same file.  The second expression was: " + line);

                    fileModuleName = module.Groups["name"].Value;

                    if (location != null)
                        _loadedModules[location] = fileModuleName;

                    continue;
                }
                else
                {
                    var fail = LuaRegex.UnhandledModule.Match(line);

                    if (fail.Success)
                    {
                        throw new NotSupportedException("A module expression was detected that is likely not yet supported propery.  The expression was: " + line);
                    }
                }

                thisFile.AddLast(line);
            }
            
            Action<string> appendContaingLine = delegate(string l)
            {
                result.AppendLine(TabString.Repeat(depth - 1) + l);
            };

            var envVariable = "_LOADED_zz_" + fileModuleName + "_env";
            var loader = "_LOADER_zz_" + fileModuleName;

            if (depth > 0)
            {
                if (fileModuleName == null)
                {
                    appendContaingLine("(function()");
                }
                else
                {
                    appendContaingLine("local " + envVariable + " = {};");
                    appendContaingLine("for key,value in pairs(getfenv()) do");
                    appendContaingLine("    " + envVariable + "[key] = value;");
                    appendContaingLine("end");
                    appendContaingLine("local " + loader + " = function()");
                    appendContaingLine("    " + loader + " = function() end;");


                    depth++;
                }
            }

            foreach (string lineToCopy in thisFile)
                result.AppendLine(TabString.Repeat(depth) + lineToCopy);
            
            if (depth > 0)
            {
                if (fileModuleName == null)
                {
                    appendContaingLine("end)();");
                }
                else
                {
                    depth--;

                    appendContaingLine("end;");
                    appendContaingLine("setfenv(" + loader + ", " + envVariable + ")");
                }
            }
        }
    }
}