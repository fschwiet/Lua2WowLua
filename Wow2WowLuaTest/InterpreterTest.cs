using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Lua2WowLua;
using NUnit.Framework;

namespace Wow2WowLuaTest
{
    [TestFixture]
    public class InterpreterTest
    {
        [Test]
        public void can_interpret_lua_file_with_module()
        {
            Stream telescopeDotLua =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("Wow2WowLuaTest.SampleFiles.telescope.lua");
             
            var result = Interpreter.FromStream(telescopeDotLua);

            Assert.That(result.IsModule, Is.True);
            Assert.That(result.ModuleName, Is.EqualTo("telescope"));
        }

        [Test]
        public void can_interupt_lua_file_with_requires()
        {
            Stream tscDotLua =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("Wow2WowLuaTest.SampleFiles.tsc");

            var result = Interpreter.FromStream(tscDotLua);

            Assert.That(result.IsModule, Is.False);
            Console.WriteLine(result.ModuleName);
        }

        [Test]
        public void warns_if_finds_unrecognized_module()
        {
            var contents = new MemoryStream(Encoding.UTF8.GetBytes(@"1
2
3
module('foo', lol)
"));

            try
            {
                var result = Interpreter.FromStream(contents);

            }
            catch (Exception e)
            {
                Assert.That(e.Message, Is.StringContaining("line 4"));
                return;
            }

            Assert.Fail("exception expected");
        }
    }
}
