module('shared', package.seeall)
local internal_value = 0;
function set(value)
    internal_value = value;
end
function get()
    return internal_value;
end
