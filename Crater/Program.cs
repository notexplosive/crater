using System.Text;
using Crater;
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
    Log.Warning(Log.CorePrefix, "Unknown version, missing VERSION file");
}

Dictionary<string, DynValue> libraryCache = new();
if (args.Length > 0)
{
    var givenPath = args[0];
    argsList.RemoveAt(0);

    var finalPath = paths.Deduce(givenPath);

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
    luaRuntime.SetGlobal("files", new FilesModule(luaRuntime, paths.WorkingFiles, paths.LocalFiles));
    luaRuntime.SetGlobal("program", new ProgramModule(luaRuntime));
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
        Log.FromLua(Log.Severity.Error, callstack);
    }
}
