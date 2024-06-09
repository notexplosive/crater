local dotnet = {}

function dotnet.publish(csprojPath, absoluteOutputPath)
    program.run("dotnet",
            { "publish", csprojPath,
              "-c", "Release",
              "-r", "win-x64",
              "/p:PublishReadyToRun=false",
              "/p:TieredCompilation=false",
              "--self-contained",
              "--output", absoluteOutputPath })
end

return dotnet