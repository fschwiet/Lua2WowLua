
module('modu', package.seeall)

function runModule()
    print("for A, module sees " .. tostring(A));
    print("for B, module sees " .. tostring(B));
    print("for I, module sees " .. tostring(I));
    print("for M, module sees " .. tostring(M));
end

runModule();

M = "module"


