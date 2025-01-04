local explogine = {
    description = "explogine build utilities"
}

local git = lib "git"
local butler = lib "butler"
local macos_build = lib "explogine_macos"
explogine.platforms = { "macos-universal", "win-x64", "linux-x64" }

function explogine.publish(appName, platformBuild, copyExtraFiles, iconPath, platformToProject, buildDirectoryForPlatform)
    for i, platform in pairs(explogine.platforms) do
        local csproj = platformToProject[platform]

        if csproj then
            local platformBuildDirectory = buildDirectoryForPlatform(platform)

            if platform == "macos-universal" then
                macos_build.makeApp(platformBuildDirectory, csproj, appName, iconPath, copyExtraFiles, platformBuild)
            else
                platformBuild(csproj, platform)
                copyExtraFiles(platform, platformBuildDirectory, platformBuildDirectory)
            end
        end
    end
end

function explogine.upload(info, targetPlatform)
    local build = info.buildDirectory

    local platformToDirectory = {
        ["mac"] = build .. "/" .. "macos-universal",
        ["windows"] = build .. "/" .. "win-x64",
        ["linux"] = build .. "/" .. "linux-x64"
    }

    local targetDirectory = platformToDirectory[targetPlatform]

    if targetDirectory == nil then
        print("could not parse target")
        print("expected: mac, windows, linux")
        return;
    end

    print("target directory: ", targetDirectory)

    if not git:isWorkspaceClean() then
        print("Git workspace is not clean!")
        print("upload cancelled")
        return
    end

    butler.push(targetDirectory, "notexplosive", info.itchUrl, info.butlerChannelForPlatform(targetPlatform))
end

return explogine
