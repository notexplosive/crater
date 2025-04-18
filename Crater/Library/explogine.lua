local explogine = {
    description = "explogine build utilities"
}

local git = lib "git"
local butler = lib "butler"
local explogine_macos = lib "explogine_macos"
explogine.platforms = { "macos-universal", "win-x64", "linux-x64" }

function explogine.publish(appName, platformBuild, copyExtraFiles, iconPath, platformToProject, buildDirectoryForPlatform)
    local canBuildMacOs = system.platform() == "macos"

    for _, platform in pairs(explogine.platforms) do
        local isMacOs = platform == "macos-universal"

        if (isMacOs and canBuildMacOs) or not isMacOs then
            local csproj = platformToProject[platform]

            if csproj then
                local platformBuildDirectory = buildDirectoryForPlatform(platform)

                if isMacOs then
                    explogine_macos.makeApp(platformBuildDirectory, csproj, appName, iconPath, copyExtraFiles,
                        platformBuild)
                else
                    platformBuild(csproj, platform)
                    copyExtraFiles(platform, platformBuildDirectory, platformBuildDirectory)
                end
            end
        else
            print("Skipping: " .. platform)
        end
    end
end

function explogine.upload(info, targetPlatform, channelSuffix)
    if channelSuffix ~= nil then
        channelSuffix = "_" .. channelSuffix
    else
        channelSuffix = ""
    end
    
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

    butler.push(targetDirectory, "notexplosive", info.itchUrl, info.butlerChannelForPlatform(targetPlatform) .. channelSuffix)
end

return explogine
