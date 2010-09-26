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
        [Parameter(ValueFromPipeline = true)] 
        public FileInfo Include;

        [Parameter(HelpMessage = "The main lua file to run.")]
        public FileInfo Source;

        FileCollection _files;
        List<IDisposable> _disposables = new List<IDisposable>();

        protected override void BeginProcessing()
        {
            _files = new FileCollection();
        }

        protected override void ProcessRecord()
        {
            System.IO.Directory.SetCurrentDirectory(this.SessionState.Path.CurrentLocation.Path);

            var filename = Include.Name;

            if (filename.LastIndexOf('.') > -1)
                filename = filename.Substring(0, filename.LastIndexOf('.'));

            var includeStream = Include.OpenRead();
            _disposables.Add(includeStream);

            _files.AddFile(filename, includeStream);
        }

        protected override void EndProcessing()
        {
            System.IO.Directory.SetCurrentDirectory(this.SessionState.Path.CurrentLocation.Path);
            
            Generator generator = new Generator(_files);
            
            var sourceStream = Source.OpenRead();
            _disposables.Add(sourceStream);
            
            var result = generator.Process(sourceStream);

            base.WriteObject(result);
        }

        public void Dispose()
        {
            var disposables = _disposables;
            _disposables = new List<IDisposable>();

            foreach (var disposible in disposables)
                disposible.Dispose();
        }

        ~ConvertWow2WowLua()
        {
            Dispose();
        }
    }
}
