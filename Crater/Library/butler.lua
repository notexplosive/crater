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

    local dryRunString = ""
    if isDryRun then
        dryRunString  = "--dry-run"
    end

    local args = { "push", dryRunString, directory, itchUrl .. "/" .. gameUrl .. ":" .. channel }

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