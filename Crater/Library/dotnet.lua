local dotnet = {
    description = "dotnet utilities"
}

-- createts args for `[dotnet] run`
local function parseRunArgs(csprojPath, givenArgs)
    local finalArgs = {
        "run",
        "--project",
        csprojPath,
        "--"
    }

    for i, arg in ipairs(givenArgs) do
        table.insert(finalArgs, arg)
    end

    return finalArgs
end

-- runs dotnet project in a given "mode"
-- mode = "Silent" | "Fork" | nil
local function runDotnetArgs(mode, csprojPath, givenArgs)
    local programRun = program["run".. (mode or "")]
    programRun("dotnet", parseRunArgs(csprojPath, givenArgs))
end

function dotnet.publish(csprojPath, absoluteOutputPath)
    program.runSilent("dotnet",
            { "publish", csprojPath,
              "-c", "Release",
              "-r", "win-x64",
              "/p:PublishReadyToRun=false",
              "/p:TieredCompilation=false",
              "--self-contained",
              "--output", absoluteOutputPath })
end

function dotnet.runFork(csprojPath, givenArgs)
    runDotnetArgs("Fork", csprojPath, givenArgs)
end


function dotnet.runSilent(csprojPath, givenArgs)
    runDotnetArgs("Silent", csprojPath, givenArgs)
end

function dotnet.run(csprojPath, givenArgs)
    runDotnetArgs(nil, csprojPath, givenArgs)
end

return dotnet