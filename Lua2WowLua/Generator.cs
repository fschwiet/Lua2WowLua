using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Lua2WowLua.Tokens;

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

            ProcessFile(null, LoadStream(file), result, false);

            return result.ToString();
        }

        string ProcessFile(string location, IEnumerable<string> file, StringBuilder result, bool isEmbedded)
        {
            var tokens = Tokenizer.Tokenize(file);

            string fileModuleName = null;

            LinkedList<string> thisFile = new LinkedList<string>();

            foreach(var token in tokens)
            {
                if (token is RequireToken)
                {
                    string requireValue = (token as RequireToken).Value;
                    IEnumerable<string> requiredFile = null;

                    using (Stream requireStream = _fileFinder.Get(requireValue))
                    {
                        requiredFile = LoadStream(requireStream);
                    }

                    string subModuleName = ProcessFile(requireValue, requiredFile, result, true);

                    thisFile.AddLast(Lookup(LoaderTable, subModuleName) + "();");

                    if (!subModuleName.StartsWith(AnonymousModulePrefix))
                        thisFile.AddLast("local " + subModuleName + " = " + Lookup(EnvTable, subModuleName) + ";");

                }
                else if (token is UnhandledRequireToken)
                {
                    OnFileError(file, token,
                                "A require expression was detected that is likely not yet supported propery.");
                }
                else if (token is SeeallModuleToken)
                {
                    if (fileModuleName != null)
                        OnFileError(file, token, "A second module expression was found in the same file.");

                    fileModuleName = (token as SeeallModuleToken).Value;

                    if (location != null)
                        _loadedModules[location] = fileModuleName;
                }
                else if (token is UnhandledModuleToken)
                {
                    OnFileError(file, token,
                                "A module expression was detected that is likely not yet supported propery.");
                }
                else if (token is OtherToken)
                {
                    thisFile.AddLast((token as OtherToken).Value);
                }
                else
                {
                    OnFileError(file, token, "Unrecognized token.");
                }
            }
            
            fileModuleName = fileModuleName ?? (AnonymousModulePrefix + ++_anonymousObjectIndex);

            if (isEmbedded)
            {
                if (!fileModuleName.StartsWith(AnonymousModulePrefix))
                {
                    result.AppendLine(Lookup(EnvTable, fileModuleName) + " = {};");
                    result.AppendLine("for key,value in pairs(getfenv()) do");
                    result.AppendLine("    " + Lookup(EnvTable, fileModuleName) + "[key] = value;");
                    result.AppendLine("end");
                }
                result.AppendLine(Lookup(LoaderTable, fileModuleName) + " = function()");
                result.AppendLine("    " + Lookup(LoaderTable, fileModuleName) + " = function() end;");
            }

            foreach (string lineToCopy in thisFile)
                result.AppendLine(TabString + lineToCopy);
            
            if (isEmbedded)
            {
                result.AppendLine("end;");

                if (!fileModuleName.StartsWith(AnonymousModulePrefix))
                    result.AppendLine("setfenv(" + Lookup(LoaderTable, fileModuleName) + ", " + Lookup(EnvTable, fileModuleName) + ")");
            }

            return fileModuleName;
        }

        void OnFileError(IEnumerable<string> file, IToken token, string description)
        {
            var line = file.Skip(token.LineNumber - 1).Take(1);

            if (line.Any())
            {
                throw new Exception(String.Format("{0}, line: {1}, expression: {2}",
                                                  description,
                                                  token.LineNumber,
                                                  line.Single()));
            }

            throw new Exception(String.Format("{0}, line (not found): {1}", description, token.LineNumber));
        }

        public static IEnumerable<string> LoadStream(Stream stream)
        {
            List<string> result = new List<string>();

            using (var fileReader = new StreamReader(stream))
            {
                string line = null;

                while ((line = fileReader.ReadLine()) != null)
                {
                    result.Add(line);
                }
            }

            return result;
        }
    }
}