local string_util = {
    description = "string utilities to amend standard library"
}

function string_util.split(inputstr, sep)
    assert(sep, "seperator is nil")
    if sep == nil then
        sep = "%s"
    end
    local t = {}
    for str in string.gmatch(inputstr, "([^"..sep.."]+)") do
        table.insert(t, str)
    end
    return t
end

function string_util.join(sequence, joiner)
    assert(joiner, "joiner is nil")
    local result = ""
    local length = #sequence
    for i, str in ipairs(sequence) do
        result = result .. str
        if i ~= length then
            result = result .. joiner
        end
    end

    return result
end

return string_util