local string_util = lib("string_util")

local version = {}

function version.bumpPatch(versionString)
    local split = string_util.split(versionString, ".")
    split[3] = tonumber(split[3]) + 1

    return string_util.join(split, ".")
end

function version.bumpMinor(versionString)
    local split = string_util.split(versionString, ".")
    local minor = tonumber(split[2]) + 1

    return split[1] .. "." .. minor .. ".0"
end

function version.bumpMajor(versionString)
    local split = string_util.split(versionString, ".")
    split[1] = tonumber(split[1]) + 1

    return split[1] .. ".0.0"
end

return version