namespace Lua2WowLua.Tokens
{
    public class SeeallModuleToken : IToken
    {
        public string Value { get; private set; }

        public SeeallModuleToken(int lineNumber, string value) : base(lineNumber)
        {
            Value = value;
        }
    }
}