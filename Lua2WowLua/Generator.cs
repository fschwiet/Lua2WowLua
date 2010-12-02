﻿using System;
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
        public static int _anonymousObjectIndex = 0;
        static readonly string EnvTable = "_ZZZ_env";
        static readonly string LoaderTable = "_ZZZ_loader";
        static readonly string AnonymousModulePrefix = "_zzz_module";

        static string Lookup(string tableName, string valueName)
        {
            return tableName + "[\"" + valueName + "\"]";
        }


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

            result.AppendLine(EnvTable + " = {}");
            result.AppendLine(LoaderTable + " = {}");

            ProcessFile(null, file, result, 0);

            return result.ToString();
        }

        string ProcessFile(string location, Stream file, StringBuilder result, int depth)
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

                    using (Stream requireStream = _fileFinder.Get(requireLocation))
                    {
                        string subModuleName = ProcessFile(requireLocation, requireStream, result, 1);

                        thisFile.AddLast(Lookup(LoaderTable, subModuleName) + "();");

                        if (!subModuleName.StartsWith(AnonymousModulePrefix))
                            thisFile.AddLast("local " + subModuleName + " = " + Lookup(EnvTable, subModuleName) + ";");
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

            fileModuleName = fileModuleName ?? (AnonymousModulePrefix + ++_anonymousObjectIndex);

            if (depth > 0)
            {
                if (!fileModuleName.StartsWith(AnonymousModulePrefix))
                {
                    appendContaingLine(Lookup(EnvTable, fileModuleName) + " = {};");
                    appendContaingLine("for key,value in pairs(getfenv()) do");
                    appendContaingLine("    " + Lookup(EnvTable, fileModuleName) + "[key] = value;");
                    appendContaingLine("end");
                }
                appendContaingLine(Lookup(LoaderTable, fileModuleName) + " = function()");
                appendContaingLine("    " + Lookup(LoaderTable, fileModuleName) + " = function() end;");

                depth++;
            }

            foreach (string lineToCopy in thisFile)
                result.AppendLine(TabString.Repeat(depth) + lineToCopy);
            
            if (depth > 0)
            {
                depth--;

                appendContaingLine("end;");

                if (!fileModuleName.StartsWith(AnonymousModulePrefix))
                    appendContaingLine("setfenv(" + Lookup(LoaderTable, fileModuleName) + ", " + Lookup(EnvTable, fileModuleName) + ")");
            }

            return fileModuleName;
        }
    }
}