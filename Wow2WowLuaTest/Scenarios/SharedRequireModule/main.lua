require('shared');
require('other');
shared.set(3);
print("expecting 3, actual: " .. shared.get());
other.set(4);
print("expecting 4, actual: " .. shared.get());
print("expected set to be nil, actual: " .. tostring(set));
