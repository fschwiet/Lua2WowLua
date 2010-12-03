_ZZZ_env = {}
_ZZZ_loader = {}
_ZZZ_env["include"] = {};
for key,value in pairs(getfenv()) do
    _ZZZ_env["include"][key] = value;
end
_ZZZ_loader["include"] = function()
    _ZZZ_loader["include"] = function() end;
    function a()
        return 2;
    end
end;
setfenv(_ZZZ_loader["include"], _ZZZ_env["include"])
    _ZZZ_loader["include"]();
    local include = _ZZZ_env["include"];
    print("expecting include.a() to be 2, it is: " .. include.a())
    print("expecting a to be nil, it is: " .. tostring(a));

