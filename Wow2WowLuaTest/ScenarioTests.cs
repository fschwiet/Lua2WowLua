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
    public class ScenarioTests
    {
        Stream GetScenarioFile(string sharedrequiremoduleMainLua)
        {
            var result = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "Wow2WowLuaTest.Scenarios." + sharedrequiremoduleMainLua);

            Assert.That(result,Is.Not.Null);
            return result;
        }

        [Test]
        public void simple_require()
        {
            var mainFile = GetScenarioFile("SimpleRequire.main.lua");
            var includeFile = GetScenarioFile("SimpleRequire.include.lua");
            var expectedFile = new StreamReader(GetScenarioFile("SimpleRequire.expected.lua")).ReadToEnd();

            FileCollection fileFinder = new FileCollection();
            fileFinder.AddFile("include", includeFile);
            Generator sut = new Generator(fileFinder);

            var result = sut.Process(mainFile);

            Assert.That(result, Is.EqualTo(expectedFile));
        }

        [Test]
        public void simple_require_module()
        {
            var mainFile = GetScenarioFile("SimpleRequireModule.main.lua");
            var includeFile = GetScenarioFile("SimpleRequireModule.include.lua");
            var expectedFile = new StreamReader(GetScenarioFile("SimpleRequireModule.expected.lua")).ReadToEnd();

            FileCollection fileFinder = new FileCollection();
            fileFinder.AddFile("include", includeFile);
            Generator sut = new Generator(fileFinder);

            var result = sut.Process(mainFile);

            Assert.That(result, Is.EqualTo(expectedFile));
        }

        [Test]
        public void shared_require_module()
        {
            var mainFile =   GetScenarioFile("SharedRequireModule.main.lua");
            var otherFile =  GetScenarioFile("SharedRequireModule.other.lua");
            var sharedFile = GetScenarioFile("SharedRequireModule.shared.lua");
            var expectedFile = new StreamReader(GetScenarioFile("SharedRequireModule.expected.lua")).ReadToEnd();

            FileCollection fileFinder = new FileCollection();
            fileFinder.AddFile("other", otherFile);
            fileFinder.AddFile("shared", sharedFile);
            Generator sut = new Generator(fileFinder);

            var result = sut.Process(mainFile);

            Assert.That(result, Is.EqualTo(expectedFile));
        }
    }
}
