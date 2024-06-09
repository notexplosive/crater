local version = lib("version")
local butler = {
    description = "itch.io butler utility"
}

function butler.login()
    program.run("butler", { "login" })
end

function butler.push(directory, itchUrl, gameUrl, channel)
    assert(directory, "directory is nil")
    assert(itchUrl, "itchUrl is nil")
    assert(gameUrl, "gameUrl is nil")
    assert(channel, "channel is nil")
    
    local args = { "push", directory, itchUrl .. "/" .. gameUrl .. ":" .. channel }

    if version.isValid(files.read("VERSION")) then
        table.insert(args, "--userversion-file=VERSION")
    end
    
    program.run("butler", args)
end

return butler