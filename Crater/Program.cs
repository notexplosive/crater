using System.Text;
using Crater;
using ExplogineCore;
using ExplogineCore.Lua;
using MoonSharp.Interpreter;

Console.OutputEncoding = Encoding.UTF8;

var argsList = args.ToList();

var paths = new PathResolver();

var version = paths.LocalFiles.ReadFile("VERSION").Trim();

if (!string.IsNullOrEmpty(version))
{
    Log.Info(Log.CorePrefix, "Crater Version: " + version);
}
else
{
#if !DEBUG
    Log.Warning(Log.CorePrefix, "Unknown version, missing VERSION file");
#endif
}

Dictionary<string, DynValue> libraryCache = new();
if (args.Length > 0)
{
    var givenPath = args[0];
    argsList.RemoveAt(0);

    var finalPath = paths.Resolve(givenPath);

    if (finalPath == null)
    {
        Log.Error(Log.CorePrefix, $"File not found {givenPath}");
        return;
    }

    Log.Info(Log.CorePrefix, $"Running {finalPath}");
    var content = paths.WorkingFiles.ReadFile(finalPath);
    var luaRuntime = new LuaRuntime(paths.WorkingFiles);

    var argsTable = luaRuntime.NewTable();
    foreach (var arg in argsList)
    {
        argsTable.Append(DynValue.NewString(arg));
    }

    luaRuntime.SetGlobal("args", argsTable);

    var modules = new CraterModule[]
    {
        new FilesModule(luaRuntime, paths.WorkingFiles, paths.LocalFiles,
            new RealFileSystem(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))),
        new ProgramModule(luaRuntime),
        new StringModule(luaRuntime),
        new SystemModule(luaRuntime)
    };

    foreach (var module in modules)
    {
        luaRuntime.SetGlobal(module.LuaReadableName(), module);
    }

    luaRuntime.SetGlobal("lib", (string path) =>
    {
        var libraryId = path + ".lua";
        if (!libraryCache.ContainsKey(libraryId))
        {
            libraryCache[libraryId] = luaRuntime.Run(paths.LocalLibrary.ReadFile(libraryId), $"lib/{path}");
        }

        return libraryCache[libraryId];
    });

    luaRuntime.MessageLogged += items => Log.FromLua(Log.Severity.Info, items);
    luaRuntime.Run(content, givenPath);

    if (luaRuntime.CurrentError != null)
    {
        Log.FromLua(Log.Severity.Error, luaRuntime.CurrentError.Exception.Message);
        var callstack = luaRuntime.Callstack();
        Log.Info(Log.CorePrefix, callstack);
    }
}