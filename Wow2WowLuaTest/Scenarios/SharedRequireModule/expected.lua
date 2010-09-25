local _LOADED_zz_shared_env = (function(e)
    local result = {};
    local key,value;
    for key,value in pairs(e) do
        result[key] = value;
    end
    return result;
end)(getfenv());
(function()
    local _LOADED_zz_shared = function()
		local internal_value = 0;
		function set(value)
			internal_value = value;
		end
		function get()
			return internal_value;
		end
    end;
    setfenv(_LOADED_zz_shared, _LOADED_zz_shared_env);
    _LOADED_zz_shared();
end)();
local shared = _LOADED_zz_shared_env;
local _LOADED_zz_other_env = (function(e)
    local result = {};
    local key,value;
    for key,value in pairs(e) do
        result[key] = value;
    end
    return result;
end)(getfenv());
(function()
    local _LOADED_zz_other = function()
		local shared = _LOADED_zz_shared_env;
		function set(value)
			shared.set(value);
		end
    end;
    setfenv(_LOADED_zz_other, _LOADED_zz_other_env);
    _LOADED_zz_other();
end)();
local other = _LOADED_zz_other_env;
shared.set(3);
print("expecting 3, actual: " .. shared.get());
other.set(4);
print("expecting 4, actual: " .. shared.get());
print("expected set to be nil, actual: " .. tostring(set));
