namespace Lua2WowLua.Tokens
{
    public class OtherToken : IToken
    {
        public string Value { get; private set; }

        public OtherToken(int lineNumber, string value) : base(lineNumber)
        {
            Value = value;
        }
    }
}