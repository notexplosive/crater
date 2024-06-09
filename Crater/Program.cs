﻿using System.Text;
using Crater;
using ExplogineCore;
using ExplogineCore.Lua;
using MoonSharp.Interpreter;

Console.OutputEncoding = Encoding.UTF8;

var argsList = args.ToList();

var localFiles = new RealFileSystem(AppDomain.CurrentDomain.BaseDirectory);
var workingFiles = new RealFileSystem(Directory.GetCurrentDirectory());
var libraryFiles = localFiles.GetDirectory("Library");

var corePrefix = "🌙";

var version = localFiles.ReadFile("VERSION").Trim();

if (!string.IsNullOrEmpty(version))
{
    Log.Info(corePrefix, "Crater Version: " + version);
}
else
{
    Log.Warning(corePrefix, "Unknown version, missing VERSION file");
}

Dictionary<string, DynValue> libraryCache = new();
if (args.Length > 0)
{
    var filePath = args[0];
    argsList.RemoveAt(0);

    if (!workingFiles.HasFile(filePath))
    {
        Log.Error(corePrefix, $"File not found {filePath}");
    }
    else
    {
        Log.Info(corePrefix, $"Running {filePath}");
    }

    var content = workingFiles.ReadFile(filePath);
    var luaRuntime = new LuaRuntime(workingFiles);

    var argsTable = luaRuntime.NewTable();
    foreach (var arg in argsList)
    {
        argsTable.Append(DynValue.NewString(arg));
    }

    luaRuntime.SetGlobal("args", argsTable);
    luaRuntime.SetGlobal("files", new FilesModule(luaRuntime, workingFiles, localFiles));
    luaRuntime.SetGlobal("program", new ProgramModule(luaRuntime));
    luaRuntime.SetGlobal("lib", (string path) =>
    {
        var libraryId = path + ".lua";
        if (!libraryCache.ContainsKey(libraryId))
        {
            libraryCache[libraryId] = luaRuntime.Run(libraryFiles.ReadFile(libraryId), $"lib/{path}");
        }

        return libraryCache[libraryId];
    });

    luaRuntime.MessageLogged += items => Log.FromLua(Log.Severity.Info, items);
    luaRuntime.Run(content, filePath);

    if (luaRuntime.CurrentError != null)
    {
        Log.FromLua(Log.Severity.Error, luaRuntime.CurrentError.Exception.Message);
        var callstack = luaRuntime.Callstack();
        Log.FromLua(Log.Severity.Error, callstack);
    }
}