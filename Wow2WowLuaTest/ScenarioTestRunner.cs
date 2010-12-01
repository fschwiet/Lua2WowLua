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
    public class ScenarioTestRunner : NJasmineFixture 
    {
        public class ScenarioTest
        {
            public string SourcePath;
            public string MainSource;
            public string ExpectedOutput;
            public string SourceProduced;
        }

        public override void Tests()
        {
            foreach(var test in GetScenarioTests())
            {
                it("integration test: " + test.MainSource, delegate
                {
                    if (File.Exists(test.SourceProduced))
                        File.Delete(test.SourceProduced);

                    string expectedOutput = File.ReadAllText(test.ExpectedOutput);

                    Generator sut = new Generator(new FileFinder(test.SourcePath));

                    var resultSource = sut.Process(test.MainSource);

                    File.WriteAllText(test.SourceProduced, resultSource, Encoding.ASCII);

                    string actualOutput = RunLuaFile(test.SourceProduced);

                    actualOutput.Should().Equal(expectedOutput);
                });
            }

            it("each scenario has the expected files", delegate
            {
                foreach(var test in GetScenarioTests())
                {
                    File.Exists(test.MainSource).Should().Be.True();
                    File.Exists(test.ExpectedOutput).Should().Be.True();
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
                ExpectedOutput = Path.Combine(scenario, "expected.txt"),
                SourceProduced = Path.Combine(scenario, "result.tmp.lua")
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

        string RunLuaFile(string filepath)
        {
            ProcessStartInfo psi = new ProcessStartInfo("lua", filepath);

            psi.WorkingDirectory = new FileInfo(filepath).Directory.FullName;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = true;

            using (var luaProcess = Process.Start(psi))
            {
                luaProcess.StandardOutput.Peek();

                luaProcess.WaitForExit();

                return luaProcess.StandardOutput.ReadToEnd();
            }
        }
    }
}
