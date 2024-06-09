using System.Reflection;
using ExplogineCore;
using ExplogineCore.Lua;
using NeatoApp;

Console.OutputEncoding = System.Text.Encoding.UTF8;

void PrintToConsole(object[] objects)
{
    Console.WriteLine($"🌑💬 {string.Join(" ", objects)}");
}

var argsList = args.ToList();

var localFiles = new RealFileSystem(AppDomain.CurrentDomain.BaseDirectory);
var workingFiles = new RealFileSystem(Directory.GetCurrentDirectory());
var appData = new RealFileSystem(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "NotExplosive", Assembly.GetEntryAssembly()!.GetName().Name));

if (args.Length > 0)
{
    var filePath = args[0];
    argsList.RemoveAt(0);

    var content = workingFiles.ReadFile(filePath);
    var luaRuntime = new LuaRuntime(workingFiles);
    luaRuntime.SetGlobal("files", new FilesModule(luaRuntime, workingFiles));
    
    luaRuntime.MessageLogged += PrintToConsole;
    luaRuntime.Run(content, "main");

    if (luaRuntime.CurrentError != null)
    {
        Console.Error.WriteLine(luaRuntime.CurrentError.Exception.Message);
        var callstack = luaRuntime.Callstack();
        Console.Error.WriteLine(callstack);
    }
}
