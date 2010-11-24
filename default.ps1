
task default -depends tests

task MakeScenarioZip {
    if (test-path .\Wow2WowLuaTest\Scenarios.zip) {
        gi .\Wow2WowLuaTest\Scenarios.zip | rm
    }
    gi .\Wow2WowLuaTest\Scenarios | % { CreateClientTestsZipResource $_.fullname }
}

task build -depends MakeScenarioZip {
	exec { & $msbuild Lua2WowLua.sln }

	if (test-path .\build\) {
		rm .\build\ -recurse
	}

	$null = mkdir .\build\
	cp .\Lua2WowLua\bin\debug\* .\build\
}

task tests -depends build {

	exec { & .\packages\NUnit.2.5.7.10213\tools\nunit-console .\Wow2WowLuaTest\bin\Debug\Wow2WowLuaTest.dll }

}

task demo -depends tests {
	import-module .\build\Lua2WowLua.dll
	gi  .\Wow2WowLuaTest\Scenarios\SharedRequireModule\*.lua | convert-Lua2WowLua -Source (gi  .\Wow2WowLuaTest\Scenarios\SharedRequireModule\main.lua)
}


#  zips directory $source="c:\directory" to "c:\directory.zip"
function CreateClientTestsZipResource($source) {
    $target = "$source.zip"
    if (test-path $target) {
        rm $target
    }
    .\lib\7-Zip\7za.exe a $target ($source + "\*")  
}
