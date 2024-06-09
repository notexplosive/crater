using Crater.ExternalPrograms;
using ExplogineCore.Lua;
using MoonSharp.Interpreter;

namespace Crater;

[LuaBoundType]
public class ProgramModule
{
    private readonly LuaRuntime _luaRuntime;

    public ProgramModule(LuaRuntime luaRuntime)
    {
        _luaRuntime = luaRuntime;
    }
    
    [LuaMember("run")]
    public DynValue Run(string runCommand, string[] args)
    {
        var program = new ExternalProgram(runCommand);
        var output = program.RunWithArgs(args);

        var table = _luaRuntime.NewTableAsDynValue();

        foreach (var item in output.Output)
        {
            table.Table.Append(DynValue.NewString(item));
        }

        return DynValue.NewTuple(DynValue.NewBoolean(output.WasSuccessful), table);
    }
}
