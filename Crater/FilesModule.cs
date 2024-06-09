using ExplogineCore;
using ExplogineCore.Lua;
using MoonSharp.Interpreter;

namespace Crater;

[LuaBoundType]
public class FilesModule
{
    private const string Prefix = "📂";
    private readonly RealFileSystem _files;
    private readonly LuaRuntime _luaRuntime;

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
        Log.Info(FilesModule.Prefix, $"Writing to file {path}");
        _files.WriteToFile(path, content.SplitLines());
    }

    [LuaMember("createOrOverwrite")]
    public void CreateOrOverwrite(string path)
    {
        Log.Info(FilesModule.Prefix, $"Overwrite/Create file {path}");
        _files.CreateOrOverwriteFile(path);
    }

    [LuaMember("create")]
    public void Create(string path)
    {
        Log.Info(FilesModule.Prefix, $"Created file {path}");
        _files.CreateFile(path);
    }

    [LuaMember("createDirectory")]
    public void CreateDirectory(string path)
    {
        Log.Info(FilesModule.Prefix, $"Created directory {path}");
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
            Log.Info(FilesModule.Prefix, $"Deleted file {file}");
            _files.DeleteFile(file);
        }

        Log.Info(FilesModule.Prefix, $"Deleted directory {path}");
        _files.DeleteDirectory(path, true);
    }

    [LuaMember("delete")]
    public void DeleteFile(string path)
    {
        Log.Info(FilesModule.Prefix, $"Deleted file {path}");
        _files.DeleteFile(path);
    }

    [LuaMember("copy")]
    public void CopyFile(string sourcePath, string destinationPath)
    {
        var sourceIsFile = _files.HasFile(sourcePath);
        var destinationIsFile = _files.HasFile(destinationPath);

        if (sourceIsFile && destinationIsFile)
        {
            var content = _files.ReadBytes(sourcePath);
            _files.CreateOrOverwriteFile(destinationPath);

            Log.Info(FilesModule.Prefix, $"Copying file {sourcePath} to file {destinationPath}");
            _files.WriteToFileBytes(destinationPath, content);
        }

        if (sourceIsFile && !destinationIsFile)
        {
            var newFileSystem = _files.CreateDirectory(destinationPath);
            var content = _files.ReadBytes(sourcePath);
            var fileName = Path.GetFileName(sourcePath);

            Log.Info(FilesModule.Prefix, $"Copying file {sourcePath} to directory {destinationPath}");
            newFileSystem.WriteToFileBytes(fileName, content);
        }

        if (!sourceIsFile && !destinationIsFile)
        {
            var sourceDirectory = _files.GetDirectory(sourcePath);
            var destinationDirectory = _files.GetDirectory(destinationPath);

            sourceDirectory.CreateDirectory();
            destinationDirectory.CreateDirectory();

            foreach (var sourceItemRelativePath in sourceDirectory.GetFilesAt("."))
            {
                Log.Info(FilesModule.Prefix, $"Copying file {sourceItemRelativePath} to directory {destinationPath}");
                var sourceItemContent = sourceDirectory.ReadBytes(sourceItemRelativePath);
                destinationDirectory.WriteToFileBytes(sourceItemRelativePath, sourceItemContent);
            }
        }

        if (!sourceIsFile && destinationIsFile)
        {
            throw new Exception($"Cannot copy from non-file {sourcePath} to file {destinationPath}");
        }
    }
}
