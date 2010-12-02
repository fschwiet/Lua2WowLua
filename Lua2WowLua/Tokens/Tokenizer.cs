using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Lua2WowLua.Tokens;

namespace Lua2WowLua.Tokens
{
    public class Tokenizer
    {
        static IEnumerable<ITokenLoader> TokenSources = new List<ITokenLoader>()
        {
            new RequireTokenLoader(),
            new UnhandledRequireTokenLoader(),
            new SeeallModuleTokenLoader(),
            new UnhandledModuleTokenLoader(),
            new OtherTokenLoader()
        };

        static public IEnumerable<IToken> Tokenize(IEnumerable<string> input)
        {
            List<IToken> result = new List<IToken>();

            int lineNumber = 0;

            foreach(var line in input)
            {
                lineNumber++;

                foreach (var loader in TokenSources)
                {
                    Match match = loader.RegularExpression.Match(line);

                    if (match.Success)
                    {
                        result.Add(loader.BuildFromMatch(match, lineNumber));
                        break;
                    }
                }
            }

            return result;
        }
    }
}
