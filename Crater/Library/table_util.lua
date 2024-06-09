local table_util = {
    description = "table utilities to amend standard library"
}

function table_util.contains(list, target)
    for i, element in ipairs(list) do
        if element == target then
            return true
        end
    end
    
    return false
end

function table_util.indexOf(list, target)
    for i, element in ipairs(list) do
        if element == target then
            return i
        end
    end

    return -1
end

return table_util