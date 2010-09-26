using System.Collections.Generic;
using System.IO;

namespace Lua2WowLua
{
    public class Interpreter
    {
        public class Result
        {
            public string Filename;
            public bool IsModule;
            public string ModuleName;
            public IEnumerable<string> Requires;
        }

        public static Result FromStream(Stream input)
        {
            var result = new Result();

            using(var reader = new StreamReader(input))
            {
                string nextLine;
                var lineIndex = 0;

                while((nextLine = reader.ReadLine()) != null)
                {
                    lineIndex++;

                    var match = LuaRegex.SeeallModule.Match(nextLine);

                    if (match.Success)
                    {
                        result.IsModule = true;
                        result.ModuleName = match.Groups["name"].Value;
                    }
                    else
                    {
                        match = LuaRegex.UnhandledModule.Match(nextLine);

                        if (match.Success)
                            throw new InvalidDataException("Found unhandled module declaration on line " + lineIndex + ".");
                    }


                }
            }

            return result;
        }
    }
}