using System.Runtime.InteropServices;
using Crater;
using ExplogineCore.Lua;
using JetBrains.Annotations;

public class SystemModule : CraterModule
{
    public SystemModule(LuaRuntime luaRuntime)
    {
        
    }

    public override string LuaReadableName()
    {
        return "system";
    }

    protected override string LuaReadableDescription()
    {
        return "Information about the local system";
    }
    
    [UsedImplicitly]
    [LuaMember("platform")]
    public string Platform()
    {
        var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var isMacOs = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        if (isLinux)
        {
            return "linux";
        }

        if (isWindows)
        {
            return "windows";
        }

        if (isMacOs)
        {
            return "macos";
        }

        return "unknown";
    }
}
