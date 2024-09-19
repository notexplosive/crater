local string_util = lib("string_util")

local version = {
    description = "utilities for parsing/modifying Semantic Version style strings"
}

function version.isValid(versionString)
    local split = string_util.split(versionString, ".")
    return #split == 3
        and tonumber(split[1]) ~= nil
        and tonumber(split[2]) ~= nil
        and tonumber(split[3]) ~= nil
end

function version.bumpPatch(versionString)
    assert(version.isValid(versionString), "Version is invalid")
    local split = string_util.split(versionString, ".")
    split[3] = tonumber(split[3]) + 1

    return string_util.join(split, ".")
end

function version.bumpMinor(versionString)
    assert(version.isValid(versionString), "Version is invalid")
    local split = string_util.split(versionString, ".")
    local minor = tonumber(split[2]) + 1

    return split[1] .. "." .. minor .. ".0"
end

function version.bumpMajor(versionString)
    assert(version.isValid(versionString), "Version is invalid")
    local split = string_util.split(versionString, ".")
    split[1] = tonumber(split[1]) + 1

    return split[1] .. ".0.0"
end

return version
