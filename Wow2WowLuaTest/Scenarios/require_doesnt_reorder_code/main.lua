local results = {};
function Record(value)
    table.insert(results, value);
end

Record(1);

require('recordA')

Record(2);

require('recordB')

Record(3);

require('recordC')

Record(4);

for index, value in ipairs(results) do
    print(value);
end
