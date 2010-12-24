using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Lua2WowLua.Snapin
{
    [Cmdlet("convert", "Lua2WowLua")]
    public class ConvertWow2WowLua : PSCmdlet, IDisposable
    {
        [Parameter(HelpMessage = "The lua file to process.", ValueFromPipeline = true)]
        public FileInfo Source;

        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            System.IO.Directory.SetCurrentDirectory(this.SessionState.Path.CurrentLocation.Path);

            var fileFinder = new FileFinder(Source.Directory.FullName);

            Generator generator = new Generator(fileFinder);

            string result = generator.Process(Source.FullName);

            WriteObject(result);
        }

        protected override void EndProcessing()
        {
        }

        public void Dispose()
        {
        }

        ~ConvertWow2WowLua()
        {
            Dispose();
        }
    }
}
