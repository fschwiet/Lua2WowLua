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
        const string TabString = "    ";

        Dictionary<string, Stream> _availableFiles = new Dictionary<string, Stream>();

        public void AddFile(string name, Stream file)
        {
            _availableFiles[name] = file;
        }

        public string Process(Stream file)
        {
            StringBuilder result = new StringBuilder();

            ProcessFile(file, result, 0);

            return result.ToString();
        }

        void ProcessFile(Stream file, StringBuilder result, int depth)
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
                    ProcessFile(_availableFiles[require.Groups["name"].Value], result, depth+1);
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

            if (depth > 0)
            {
                if (fileModuleName == null)
                {
                    appendContaingLine("(function()");
                }
                else
                {
                    appendContaingLine("local _LOADED_zz_" + fileModuleName + "_env = (function(e)");
                    appendContaingLine("    local result = {};");
                    appendContaingLine("    local key,value;");
                    appendContaingLine("    for key,value in pairs(e) do");
                    appendContaingLine("        result[key] = value;");
                    appendContaingLine("    end");
                    appendContaingLine("    return result;");
                    appendContaingLine("end)(getfenv());");
                    appendContaingLine("(function()");
                    appendContaingLine("    local _LOADED_zz_" + fileModuleName + " = function()");

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

                    appendContaingLine("    end;");
                    appendContaingLine("    setfenv(_LOADED_zz_" + fileModuleName + ", _LOADED_zz_" + fileModuleName + "_env);");
                    appendContaingLine("    _LOADED_zz_include();");
                    appendContaingLine("end)();");
                    appendContaingLine("local " + fileModuleName + " = _LOADED_zz_" + fileModuleName + "_env;");
                }
            }
        }
    }
}