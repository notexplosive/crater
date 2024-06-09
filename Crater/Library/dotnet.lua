local dotnet = {
    description = "dotnet utilities"
}

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

return dotnet