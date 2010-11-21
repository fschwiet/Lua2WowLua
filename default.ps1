
task default -depends deploy

task build {
	exec { & $msbuild Lua2WowLua.sln }

	if (test-path .\build\) {
		rm .\build\ -recurse
	}

	$null = mkdir .\build\
	cp .\Lua2WowLua\bin\debug\* .\build\
}

task tests -depends build {

	exec { nunit-console .\Wow2WowLuaTest\bin\Debug\Wow2WowLuaTest.dll }

}

task demo -depends tests {
	import-module .\build\Lua2WowLua.dll
	gi  .\Wow2WowLuaTest\Scenarios\SharedRequireModule\*.lua | convert-Lua2WowLua -Source (gi  .\Wow2WowLuaTest\Scenarios\SharedRequireModule\main.lua)
}