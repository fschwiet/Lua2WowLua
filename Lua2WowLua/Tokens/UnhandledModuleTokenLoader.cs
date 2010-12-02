using System;
using System.Text.RegularExpressions;

namespace Lua2WowLua.Tokens
{
    public class UnhandledModuleTokenLoader : ITokenLoader
    {
        public Regex RegularExpression
        {
            get { return LuaRegex.UnhandledModule; }
        }

        public IToken BuildFromMatch(Match match, int lineNumber)
        {
            return new UnhandledModuleToken(lineNumber);
        }
    }
}