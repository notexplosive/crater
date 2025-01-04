local macos_build = {
    description = "macos utilities for explogine builds"
}
local dotnet = lib "dotnet"

macos_build.universalArchitecture = "macos-universal"

function macos_build.makeApp(platformBuildDirectory, csproj, appName, iconPath, copyExtraFiles, platformBuild)
    -- todo: assert(files.exists("Info.plist"))

    local appDirectory = platformBuildDirectory .. "/" .. appName .. ".app"
    local contentDirectory = appDirectory .. "/" .. "Contents"
    local resourcesDirectory = contentDirectory .. "/" .. "Resources"
    local frameworksDirectory = contentDirectory .. "/" .. "Frameworks"
    local iconsetDirectory = ".iconset" .. "/" .. appName .. ".iconset"

    files.deleteDirectory(iconsetDirectory)

    -- when we generate the png icon, this is where it will live
    local iconPngPath = iconsetDirectory .. "/" .. "Icon.png"

    -- game exe gets copied here
    local macOSDirectory = contentDirectory .. "/" .. "MacOS"

    files.createDirectory(contentDirectory)
    files.createDirectory(resourcesDirectory)
    files.createDirectory(macOSDirectory)

    local armOutput = platformBuild(csproj, "osx-arm64")
    local x64Output = platformBuild(csproj, "osx-x64")

    local function createUniversalExecutableAndCopy()
        program.run("lipo",
            {
                "-create",
                armOutput .. "/" .. appName,
                x64Output .. "/" .. appName,
                "-output",

                macOSDirectory .. "/" .. appName
            }
        )
    end

    local function copyContentToApp()
        -- It doesn't matter if we take content from arm64 or x64, we just need to choose one
        print("Copying Content")
        files.copy(armOutput .. "/" .. "Content", resourcesDirectory .. "/" .. "Content")

        -- macOS only
        files.copy("Info.plist", contentDirectory)
    end

    local function copyDylibsToApp()
        -- It shouldn't matter where the dylibs come from because they're already in the universal format
        print("Copying Dylibs")
        for i, dylib in ipairs(files.list(armOutput, false, "dylib")) do
            files.copy(dylib, frameworksDirectory)
        end
    end

    local function createIconAtSize(size, is2x)
        local trueSize = size
        local at2xString = ""

        if is2x then
            at2xString = "@2x"
            trueSize = size * 2
        end

        program.run("sips",
            {
                "-z",
                trueSize,
                trueSize,
                iconPngPath,
                "--out",
                iconsetDirectory .. "/" .. "icon_" .. size .. "x" .. size .. at2xString .. ".png"
            }
        )
    end

    local function createMacIcon()
        files.createDirectory(iconsetDirectory)

        -- convert our icon (most likely a bmp) into a png
        program.run("sips",
            {
                "-s",
                "format",
                "png",
                iconPath,
                "--out",
                iconPngPath
            }
        )

        for _, size in ipairs({ 16, 32, 128, 256, 512 }) do
            for _, is2x in ipairs({ true, false }) do
                createIconAtSize(size, is2x)
            end
        end

        program.run("iconutil", {
            "-c",
            "icns",
            iconsetDirectory,
            "--output",
            resourcesDirectory .. "/" .. appName .. ".icns"
        })
    end

    local function testRun()
        -- test run

        program.run("open", { "-W", appDirectory })

        local logsLocation = files.homeDirectory() .. '/Library/Application Support/NotExplosive/FuncSynth/Logs'

        -- read logs to see why we crashed
        local allLogs = {}
        for i, logFile in ipairs(files.list(logsLocation, false, "log")) do
            table.insert(allLogs, logFile)
        end

        table.sort(allLogs)

        local latestLog = allLogs[#allLogs]

        if latestLog then
            print(latestLog .. ":\n" .. files.read(latestLog))
        else
            print("could not find log file")
        end
    end


    createUniversalExecutableAndCopy()
    copyContentToApp()
    copyDylibsToApp()
    createMacIcon()

    if copyExtraFiles then
        copyExtraFiles(macos_build.universalArchitecture, resourcesDirectory, platformBuildDirectory)
    end

    -- testRun()
end

return macos_build
