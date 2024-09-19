using System.Reflection;
using System.Text;
using ExplogineCore.Lua;
using MoonSharp.Interpreter;

namespace Crater;

public abstract class CraterModule
{
    public abstract string LuaReadableName();

    [LuaMember("help")]
    public string Help()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"{LuaReadableName()} {LuaReadableDescription()}");

        foreach (var member in GetType().GetMembers()
                     .Where(method => Attribute.IsDefined(method, typeof(LuaMemberAttribute))))
        {
            foreach (var attribute in member.GetCustomAttributes<LuaMemberAttribute>())
            {
                if ((member.MemberType & MemberTypes.Method) != 0)
                {
                    var methodInfo = (member as MethodInfo)!;
                    var parameters = string.Join(", ", methodInfo.GetParameters().Select(a => a.Name));
                    stringBuilder.AppendLine(
                        $"\tfunction: {attribute.LuaVisibleName}({parameters}) -> {CraterModule.TypeToSafeName(methodInfo.ReturnType)}");
                }
                else
                {
                    stringBuilder.AppendLine($"\t{member.MemberType.ToString().ToLower()}: {attribute.LuaVisibleName}");
                }
            }
        }

        return stringBuilder.ToString();
    }

    protected abstract string LuaReadableDescription();

    private static string TypeToSafeName(Type type)
    {
        return type == typeof(DynValue) ? "any" : type.Name.ToLower();
    }
}
