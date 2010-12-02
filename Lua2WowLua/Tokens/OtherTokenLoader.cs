using System.Text.RegularExpressions;

namespace Lua2WowLua.Tokens
{
    public class OtherTokenLoader : ITokenLoader
    {
        static Regex MatchAny = new Regex("^.*$", RegexOptions.Compiled);

        public Regex RegularExpression
        {
            get { return MatchAny; }
        }

        public IToken BuildFromMatch(Match match, int lineNumber)
        {
            return new OtherToken(lineNumber, match.Groups[0].Value);
        }
    }
}