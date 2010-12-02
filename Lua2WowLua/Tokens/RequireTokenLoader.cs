using System.Text.RegularExpressions;

namespace Lua2WowLua.Tokens
{
    public class RequireTokenLoader : ITokenLoader
    {
        public Regex RegularExpression
        {
            get { return LuaRegex.Require; }
        }

        public IToken BuildFromMatch(Match match, int lineNumber)
        {
            return new RequireToken(lineNumber, match.Groups["name"].Value);
        }
    }
}