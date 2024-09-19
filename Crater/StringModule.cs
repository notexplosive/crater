using System.Reflection;
using ExplogineCore.Lua;
using JetBrains.Annotations;
using MoonSharp.Interpreter;

namespace Crater;

public class StringModule : CraterModule
{
    private readonly LuaRuntime _luaRuntime;

    public StringModule(LuaRuntime luaRuntime)
    {
        _luaRuntime = luaRuntime;
    }
    
    public override string LuaReadableName()
    {
        return "strings";
    }

    protected override string LuaReadableDescription()
    {
        return "provides an easy api for string manipulation";
    }

    [UsedImplicitly]
    [LuaMember("split")]
    public string[] Split(string input, string separator)
    {
        return input.Split(separator);
    }
    
    [UsedImplicitly]
    [LuaMember("splitMany")]
    public string[] SplitMany(string input, params string[] separators)
    {
        return input.Split(separators, StringSplitOptions.None);
    }

    [UsedImplicitly]
    [LuaMember("join")]
    public string Join(string joiner, string[] items)
    {
        return string.Join(joiner, items);
    }
    
    [UsedImplicitly]
    [LuaMember("trim")]
    public string Trim(string subject)
    {
        return subject.Trim();
    }

    [UsedImplicitly]
    [LuaMember("replace")]
    public string Replace(string subject, string original, string replacement)
    {
        return subject.Replace(original, replacement);
    }
}
