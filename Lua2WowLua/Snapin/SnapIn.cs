using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Lua2WowLua.Snapin
{
    [RunInstaller(true)]
    public class SnapIn : PSSnapIn
    {
        public override string Name
        {
            get { return "Lua2WowLua"; }
        }

        public override string Vendor
        {
            get { return "Frank Schwieterman"; }
        }

        public override string Description
        {
            get { return "Converts Lua script to run within the limited compiler provided by World of Warcraft."; }
        }
    }
}
