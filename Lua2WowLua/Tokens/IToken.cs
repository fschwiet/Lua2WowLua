using System;

namespace Lua2WowLua.Tokens
{
    public abstract class IToken
    {
        public int LineNumber { get; protected set; }

        public IToken(int lineNumber)
        {
            LineNumber = lineNumber;
        }
    }
}