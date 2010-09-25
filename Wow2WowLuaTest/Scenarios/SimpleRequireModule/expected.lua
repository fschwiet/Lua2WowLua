_LOADED_zz_include = (function()
	function a()
		return 2;
	end
	return getfenv();
end)();
include = _LOADED_zz_include;
print("expecting include.a() to be 2, it is: " .. include.a())
