using System.Text.RegularExpressions;

namespace Lua2WowLua.Tokens
{
    public class SeeallModuleTokenLoader : ITokenLoader
    {
        public Regex RegularExpression
        {
            get { return LuaRegex.SeeallModule; }
        }

        public IToken BuildFromMatch(Match match, int lineNumber)
        {
            return new SeeallModuleToken(lineNumber, match.Groups["name"].Value);
        }
    }
}