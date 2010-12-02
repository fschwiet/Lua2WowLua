using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lua2WowLua.Tokens
{
    public interface ITokenLoader
    {
        Regex RegularExpression { get; }
        IToken BuildFromMatch(Match match, int lineNumber);
    }
}
