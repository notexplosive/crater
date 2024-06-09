local git = {
    description = "git utilities"
}

function git.hasRemote()
    local success, output = program.runSilent("git", { "remote" })
    assert(success, "git failed to run")

    if #output == 0 then
        return false
    else
        return true
    end
end

function git.isWorkspaceClean()
    local success, output = program.runSilent("git", { "status" })
    assert(success, "git failed to run")

    for i, line in ipairs(output) do
        if string.match(line, "nothing to commit, working tree clean") then
            return true
        end
    end

    return false
end

function git.isUpToDateWithRemote()
    if not git.hasRemote() then
        return true
    end

    local success, output = program.runSilent("git", { "status" })

    assert(success, "git failed to run")

    return string.match(output[2], "Your branch is up to date") ~= nil
end

return git