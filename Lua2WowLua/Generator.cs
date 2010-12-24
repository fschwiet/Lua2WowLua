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
        const string TabString = "    ";
        public static int _anonymousObjectIndex = 0;

        readonly string Namespace;
        readonly string EnvTable;
        readonly string LoaderTable;
        readonly string AnonymousModulePrefix;

        readonly IFileFinder _fileFinder;
        Dictionary<string, string> _modulesLoadedByFilepath;

        public Generator(IFileFinder fileFinder, string @namespace)
        {
            _fileFinder = fileFinder;

            Namespace = @namespace;
            EnvTable = @namespace + "_env";
            LoaderTable = @namespace + "_loader";;
            AnonymousModulePrefix = @namespace + "_module";
        }

        public string Process(string filepath)
        {
            StringBuilder result = new StringBuilder();
            _modulesLoadedByFilepath = new Dictionary<string, string>();

            result.AppendLine(Namespace + " = {};");
            result.AppendLine("setmetatable(" + Namespace + ", { __index = _G });");
            result.AppendLine("setfenv(function()");

            result.AppendLine(EnvTable + " = {}");
            result.AppendLine(LoaderTable + " = {}");

            ProcessFile(File.ReadAllLines(filepath), result, null);

            result.Append("end," + Namespace + ")();");

            return result.ToString();
        }

        string ProcessFile(IEnumerable<string> file, StringBuilder result, LinkedList<string> afterOuterContextAccumulator)
        {
            var tokens = Tokenizer.Tokenize(file);

            string referenceName = null;
            bool referencingModule = false;

            LinkedList<string> afterAccumulator = new LinkedList<string>();

            foreach(var token in tokens)
            {
                if (token is RequireToken)
                {
                    string requireValue = (token as RequireToken).Value;
                    string filePath = _fileFinder.GetNormalizedFilepath(requireValue);

                    string subModuleName = LoadRequireIfNecessaryAndGetModuleName(filePath, result, afterAccumulator);

                    if (subModuleName != null)
                        afterAccumulator.AddLast(subModuleName + " = " + Lookup(EnvTable, subModuleName) + ";");
                }
                else if (token is UnhandledRequireToken)
                {
                    OnFileError(file, token,
                                "A require expression was detected that is likely not yet supported propery.");
                }
                else if (token is SeeallModuleToken)
                {
                    if (referenceName != null)
                        OnFileError(file, token, "A second module expression was found in the same file.");

                    referenceName = (token as SeeallModuleToken).Value;
                    referencingModule = true;
                }
                else if (token is UnhandledModuleToken)
                {
                    OnFileError(file, token,
                                "A module expression was detected that is likely not yet supported propery.");
                }
                else if (token is OtherToken)
                {
                    afterAccumulator.AddLast((token as OtherToken).Value);
                }
                else
                {
                    OnFileError(file, token, "Unrecognized token.");
                }
            }
            
            referenceName = referenceName ?? (AnonymousModulePrefix + ++_anonymousObjectIndex);

            if (afterOuterContextAccumulator != null)
            {
                if (referencingModule)
                {
                    result.AppendLine(Lookup(EnvTable, referenceName) + " = {};");
                    result.AppendLine("setmetatable(" + Lookup(EnvTable, referenceName) + ", { __index = getfenv() });");
                }
                result.AppendLine(Lookup(LoaderTable, referenceName) + " = function()");
                result.AppendLine("    " + Lookup(LoaderTable, referenceName) + " = function() end;");
            }

            foreach (string lineToCopy in afterAccumulator)
                result.AppendLine(TabString + lineToCopy);

            if (afterOuterContextAccumulator != null)
            {
                result.AppendLine("end;");

                if (referencingModule)
                    result.AppendLine("setfenv(" + Lookup(LoaderTable, referenceName) + ", " + Lookup(EnvTable, referenceName) + ")");

                afterOuterContextAccumulator.AddLast(Lookup(LoaderTable, referenceName) + "();");
            }

            return referencingModule ? referenceName : null;
        }

        string LoadRequireIfNecessaryAndGetModuleName(string filePath, StringBuilder result, LinkedList<string> afterOuterContextAccumulator)
        {
            string normalizedFilepath = new Uri(filePath).LocalPath;

            if (_modulesLoadedByFilepath.ContainsKey(normalizedFilepath))
                return _modulesLoadedByFilepath[normalizedFilepath];

            IEnumerable<string> requiredFile = File.ReadAllLines(normalizedFilepath);

            string moduleNameOrNull = ProcessFile(requiredFile, result, afterOuterContextAccumulator);

            _modulesLoadedByFilepath[normalizedFilepath] = moduleNameOrNull;

            return moduleNameOrNull;
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

        static string Lookup(string tableName, string valueName)
        {
            return tableName + "[\"" + valueName + "\"]";
        }
    }
}