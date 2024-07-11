using System.Reflection;
using System.Text;
using ExplogineCore.Lua;

namespace Crater;

public abstract class CraterModule
{
    [LuaMember("help")]
    public string Help()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"{ModuleName} gives you access to the local filesystem with convenience functions");

        foreach (var member in GetType().GetMembers()
                     .Where(method => Attribute.IsDefined(method, typeof(LuaMemberAttribute))))
        {
            foreach (var attribute in member.GetCustomAttributes<LuaMemberAttribute>())
            {
                if ((member.MemberType & MemberTypes.Method) != 0)
                {
                    var methodInfo = (member as MethodInfo)!;
                    var parameters = string.Join(", ", methodInfo.GetParameters().Select(a => a.Name));
                    stringBuilder.AppendLine($"\tfunction: {attribute.LuaVisibleName}({parameters})");
                }
                else
                {
                    stringBuilder.AppendLine($"\t{member.MemberType.ToString().ToLower()}: {attribute.LuaVisibleName}");
                }
            }
        }

        return stringBuilder.ToString();
    }

    public abstract string ModuleName { get; }
}
