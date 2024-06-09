using Crater.ExternalPrograms;
using ExplogineCore;
using ExplogineCore.Lua;

namespace Crater;

[LuaBoundType]
public class DotnetModule
{
    private readonly RealFileSystem _files;
    private readonly DotnetProgram _dotnet = new();

    public DotnetModule(RealFileSystem files)
    {
        _files = files;
    }

    [LuaMember("publish")]
    public void Publish(string csProjPath, string workingLocalPath)
    {
        _dotnet.NormalPublish(csProjPath, _files.ToAbsolutePath(workingLocalPath));
    }
    
    [LuaMember("version")]
    public void PrintVersion()
    {
        _dotnet.Version();
    }
}
