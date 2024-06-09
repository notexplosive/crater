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
    public DynValue Run(string runCommand, DynValue args)
    {
        var finalArgs = ProgramModule.ExtractArgs(args);
        return RunAndGetOutput(new ExternalProgram(runCommand), finalArgs);
    }
    
    [LuaMember("runSilent")]
    public DynValue RunSilent(string runCommand, DynValue args)
    {
        var finalArgs = ProgramModule.ExtractArgs(args);
        var program = new ExternalProgram(runCommand)
        {
            SuppressLogging = true
        };
        return RunAndGetOutput(program, finalArgs);
    }

    private DynValue RunAndGetOutput(ExternalProgram program, string[] finalArgs)
    {
        var output = program.RunWithArgs(finalArgs);

        var table = _luaRuntime.NewTableAsDynValue();

        foreach (var item in output.Output)
        {
            table.Table.Append(DynValue.NewString(item));
        }

        return DynValue.NewTuple(DynValue.NewBoolean(output.WasSuccessful), table);
    }

    private static string[] ExtractArgs(DynValue args)
    {
        var finalArgs = new List<string>();
        if (args.Type == DataType.Table)
        {
            foreach (var keyPair in args.Table.Pairs.Where(kp => kp.Key.Type == DataType.Number))
            {
                finalArgs.Add(keyPair.Value.CastToString());
            }
        }
        else
        {
            Log.Error(ExternalProgram.Prefix,$"Invalid args to program.run, expected (string, table)");
        }

        return finalArgs.ToArray();
    }
}
