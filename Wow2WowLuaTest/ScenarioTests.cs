using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Lua2WowLua;
using NJasmine;
using NJasmine.Extras;
using Should.Fluent;

namespace Wow2WowLuaTest
{
    public class ScenarioTests : NJasmineFixture 
    {
        public class ScenarioTest
        {
            public string SourcePath;
            public string MainSource;
            public string ExpectedSource;
        }

        public override void Tests()
        {
            foreach(var test in GetScenarioTests())
            {
                it("integration test: " + test.MainSource, delegate
                {
                        string expectedSource = File.ReadAllText(test.ExpectedSource);

                        Generator sut = new Generator(new FileFinder(test.SourcePath));

                        var results = sut.Process(test.MainSource);

                        results.Should().Equal(expectedSource);
                });
            }

            it("each scenario has the expected files", delegate
            {
                foreach(var test in GetScenarioTests())
                {
                    File.Exists(test.MainSource).Should().Be.True();
                    File.Exists(test.ExpectedSource).Should().Be.True();
                }
            });

            it("can load the scenarios", delegate
            {
                IEnumerable<string> results = GetScenarioPaths();

                results.Count().Should().Be.GreaterThan(2);
            });
        }

        IEnumerable<ScenarioTest> GetScenarioTests()
        {
            return GetScenarioPaths().Select(scenario => new ScenarioTest()
            {
                SourcePath = scenario,
                MainSource = Path.Combine(scenario, "main.lua"),
                ExpectedSource = Path.Combine(scenario, "expected.lua")
            });
        }

        IEnumerable<string> GetScenarioPaths()
        {
            var tempDir = ExtractZip();
            return Directory.EnumerateDirectories(tempDir);
        }

        string ExtractZip()
        {
            return ZipFixtureLoader.UnzipBinDeployedToTempDirectory("Scenarios.zip", "Wow2WowLuaTest.tests");
        }
    }
}
