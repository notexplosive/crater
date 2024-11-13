local version = lib("version")
local butler = {
    description = "itch.io butler utility"
}

function butler.login()
    program.run("butler", { "login" })
end

local function internalPush(directory, itchUrl, gameUrl, channel, isDryRun)
    assert(directory, "directory is nil")
    assert(itchUrl, "itchUrl is nil")
    assert(gameUrl, "gameUrl is nil")
    assert(channel, "channel is nil")

    local args = { "push", directory, itchUrl .. "/" .. gameUrl .. ":" .. channel }
    if isDryRun then
        table.insert(args, 1, "--dry-run")
    end

    if version.isValid(files.read("VERSION")) then
        table.insert(args, "--userversion-file=VERSION")
    end

    program.run("butler", args)
end

function butler.push(directory, itchUrl, gameUrl, channel)
    internalPush(directory, itchUrl, gameUrl, channel, false)
end

function butler.pushDry(directory, itchUrl, gameUrl, channel)
    internalPush(directory, itchUrl, gameUrl, channel, true)
end

return butler