using System.Text.RegularExpressions;

namespace Lua2WowLua.Tokens
{
    public class UnhandledRequireTokenLoader : ITokenLoader
    {
        public Regex RegularExpression
        {
            get { return LuaRegex.UnhandledRequire; }
        }

        public IToken BuildFromMatch(Match match, int lineNumber)
        {
            return new UnhandledRequireToken(lineNumber);
        }
    }
}