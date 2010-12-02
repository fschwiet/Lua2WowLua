namespace Lua2WowLua.Tokens
{
    public class RequireToken : IToken
    {
        public string Value { get; private set; }

        public RequireToken(int lineNumber, string value) : base(lineNumber)
        {
            Value = value;
        }
    }
}