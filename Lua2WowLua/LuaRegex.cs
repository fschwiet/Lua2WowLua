using System.Text.RegularExpressions;

namespace Lua2WowLua
{
    public class LuaRegex
    {
        static string NameSubexpression = @"\s*('|"")(?<name>\w+)('|"")\s*";
        public static Regex SeeallModule = new Regex(@"^\s*module\s*\(" + NameSubexpression + @",\s*package\s*\.\s*seeall\s*\)\s*$");
        public static Regex UnhandledModule = new Regex(@"^\s*module\s*\(");
        public static Regex Require = new Regex(@"^\s*require\s*\(" + NameSubexpression + @"\)\s*$");
        public static Regex UnhandledRequire = new Regex(@"^\s*require\s*\(");
    }
}