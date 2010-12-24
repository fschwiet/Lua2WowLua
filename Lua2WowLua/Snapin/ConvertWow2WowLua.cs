using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Lua2WowLua.Snapin
{
    [Cmdlet("convert", "Lua2WowLua")]
    public class ConvertWow2WowLua : PSCmdlet
    {
        [Parameter(HelpMessage = "The lua file to process.", ValueFromPipeline = true)]
        public FileInfo Source;

        [Parameter(HelpMessage = "Directories to search when referencing files.")]
        public DirectoryInfo[] Includes;

        [Parameter(HelpMessage = "The name of the global value acting as the included source's namespace.")] 
        public string Namespace;

        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            System.IO.Directory.SetCurrentDirectory(this.SessionState.Path.CurrentLocation.Path);

            var includes = (Includes ?? new[] {Source.Directory}).Select(d => d.FullName);

            var fileFinder = new FileFinder(new []{Source.Directory.FullName});

            Generator generator = new Generator(fileFinder, Namespace);

            string result = generator.Process(Source.FullName);

            WriteObject(result);
        }

        protected override void EndProcessing()
        {
        }
    }
}
