local _LOADED_zz_include_env = (function(e)
    local result = {};
    local key,value;
    for key,value in pairs(e) do
        result[key] = value;
    end
    return result;
end)(getfenv());
(function()
    local _LOADED_zz_include = function()
        function a()
            return 2;
        end
    end;
    setfenv(_LOADED_zz_include, _LOADED_zz_include_env);
    _LOADED_zz_include();
end)();
local include = _LOADED_zz_include_env;
print("expecting include.a() to be 2, it is: " .. include.a())
print("expecting a to be nil, it is: " .. tostring(a));
