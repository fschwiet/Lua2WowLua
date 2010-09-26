
task default -depends deploy

task uninstall {
	if (test-path .\build\Lua2WowLua.dll) {
		& installutil /uninstall .\build\Lua2WowLua.dll
	}
}

task build {
	exec { & $msbuild Lua2WowLua.sln }
}

task tests -depends build {
	exec { nunit-console .\Wow2WowLuaTest\bin\Debug\Wow2WowLuaTest.dll }
}

task deploy -depends tests, uninstall {
	
	if (test-path .\build\) {
		rm .\build\ -recurse
	}

	mkdir .\build\
	cp .\Lua2WowLua\bin\debug\* .\build\

	exec { & installutil /install .\build\Lua2WowLua.dll }
}

task demo -depends deploy {
	add-pssnapin Lua2WowLua
	gi  .\Wow2WowLuaTest\Scenarios\SharedRequireModule\*.lua | convert-Lua2WowLua -Source (gi  .\Wow2WowLuaTest\Scenarios\SharedRequireModule\main.lua)
}