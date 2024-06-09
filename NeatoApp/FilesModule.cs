using ExplogineCore;
using ExplogineCore.Lua;
using MoonSharp.Interpreter;

namespace NeatoApp;

[LuaBoundType]
public class FilesModule
{
    private readonly LuaRuntime _luaRuntime;
    private readonly RealFileSystem _files;

    public FilesModule(LuaRuntime luaRuntime, RealFileSystem files)
    {
        _luaRuntime = luaRuntime;
        _files = files;
    }

    [LuaMember("read")]
    public string Read(string path)
    {
        return _files.ReadFile(path);
    }
    
    [LuaMember("write")]
    public void Write(string path, string content)
    {
        _files.WriteToFile(path, content.SplitLines());
    }
    
    [LuaMember("createOrOverwrite")]
    public void CreateOrOverwrite(string path)
    {
        _files.CreateOrOverwriteFile(path);
    }
    
    [LuaMember("create")]
    public void Create(string path)
    {
        _files.CreateFile(path);
    }
    
    [LuaMember("createDirectory")]
    public void CreateDirectory(string path)
    {
        _files.GetDirectory(path).CreateDirectory();
    }
    
    [LuaMember("list")]
    public Table ListFiles(string path, bool recursive = false, string extension = "*")
    {
        var result = _luaRuntime.NewTable();
        foreach (var file in _files.GetFilesAt(path, extension, recursive))
        {
            result.Append(DynValue.NewString(file));
        }

        return result;
    }

    [LuaMember("deleteDirectory")]
    public void DeleteDirectory(string path)
    {
        foreach (var file in _files.GetFilesAt(path))
        {
            _files.DeleteFile(file);
        }

        _files.DeleteDirectory(path, true);
    }

    [LuaMember("copy")]
    public void CopyFile(string sourcePath, string destinationPath)
    {
        var sourceIsFile = _files.HasFile(sourcePath);
        var destinationIsFile = _files.HasFile(destinationPath);
        
        if (sourceIsFile && destinationIsFile)
        {
            var content = _files.ReadFile(sourcePath);
            _files.CreateOrOverwriteFile(destinationPath);
            _files.WriteToFile(destinationPath, content);
        }

        if (sourceIsFile && !destinationIsFile)
        {
            var newFileSystem = _files.CreateDirectory(destinationPath);
            var content = _files.ReadFile(sourcePath);
            var fileName = Path.GetFileName(sourcePath);
            newFileSystem.WriteToFile(fileName, content);
        }

        if (!sourceIsFile && !destinationIsFile)
        {
            var sourceDirectory = _files.GetDirectory(sourcePath);
            var destinationDirectory = _files.GetDirectory(destinationPath);

            sourceDirectory.CreateDirectory();
            destinationDirectory.CreateDirectory();
            
            foreach (var sourceItemRelativePath in sourceDirectory.GetFilesAt("."))
            {
                var sourceItemContent = sourceDirectory.ReadFile(sourceItemRelativePath);
                destinationDirectory.WriteToFile(sourceItemRelativePath, sourceItemContent);
            }
        }
        
        if (!sourceIsFile && destinationIsFile)
        {
            throw new Exception($"Cannot copy from non-file {sourcePath} to file {destinationPath}");
        }
    }
}
