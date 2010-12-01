using System.Text.RegularExpressions;

namespace Lua2WowLua
{
    public class LuaRegex
    {
        static string FileReference = @"\s*('|"")(?<name>[\w\\/]+)(\.lua)?('|"")\s*";
        static string EndOfLine = @"(--.*)?$";
        public static Regex SeeallModule = new Regex(@"^\s*module\s*\(" + FileReference + @",\s*package\s*\.\s*seeall\s*\)\s*(;?\s*)*" + EndOfLine);
        public static Regex UnhandledModule = new Regex(@"^\s*module\s*");
        public static Regex Require = new Regex(@"^\s*require\s*\(?" + FileReference + @"\)?\s*(;?\s*)*" + EndOfLine);
        public static Regex UnhandledRequire = new Regex(@"^\s*require\s*");
    }
}