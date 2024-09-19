using ExplogineCore;
using ExplogineCore.Lua;
using MoonSharp.Interpreter;

namespace Crater;

[LuaBoundType]
public class FilesModule : CraterModule
{
    private const string FolderPrefix = "📂";
    private const string FilePrefix = "📄";
    private const string WritePrefix = "📝";
    private const string CopyPrefix = "🔄";
    private const string DeletePrefix = "💥";
    private readonly RealFileSystem _homeFiles;
    private readonly RealFileSystem _localFiles;
    private readonly LuaRuntime _luaRuntime;
    private readonly RealFileSystem _workingFiles;

    public FilesModule(LuaRuntime luaRuntime, RealFileSystem workingFiles, RealFileSystem localFiles,
        RealFileSystem homeFiles)
    {
        _luaRuntime = luaRuntime;
        _workingFiles = workingFiles;
        _localFiles = localFiles;
        _homeFiles = homeFiles;
    }

    [LuaMember("workingDirectory")]
    public string WorkingDirectory()
    {
        // appended so all of these paths end with a /
        return _workingFiles.FullNormalizedRootPath + "/";
    }

    [LuaMember("localDirectory")]
    public string LocalDirectory()
    {
        return _localFiles.FullNormalizedRootPath;
    }

    [LuaMember("homeDirectory")]
    public string HomeDirectory()
    {
        // appended so all of these paths end with a /
        return _homeFiles.FullNormalizedRootPath + "/";
    }

    [LuaMember("read")]
    public string Read(string path)
    {
        return _workingFiles.ReadFile(path);
    }

    [LuaMember("write")]
    public void Write(string path, string content)
    {
        Log.Info(FilesModule.WritePrefix, $"Writing to file {path}");
        _workingFiles.WriteToFile(path, content.SplitLines());
    }

    [LuaMember("createOrOverwrite")]
    public void CreateOrOverwrite(string path)
    {
        Log.Info(FilesModule.FilePrefix, $"Overwriting/Creating file {path}");
        _workingFiles.CreateOrOverwriteFile(path);
    }

    [LuaMember("create")]
    public void Create(string path)
    {
        Log.Info(FilesModule.FilePrefix, $"Created file {path}");
        _workingFiles.CreateFile(path);
    }

    [LuaMember("createDirectory")]
    public void CreateDirectory(string path)
    {
        Log.Info(FilesModule.FolderPrefix, $"Created directory {path}");
        _workingFiles.GetDirectory(path).CreateDirectory();
    }

    [LuaMember("list")]
    public Table ListFiles(string path, bool recursive = false, string extension = "*")
    {
        var result = _luaRuntime.NewTable();
        foreach (var file in _workingFiles.GetFilesAt(path, extension, recursive))
        {
            result.Append(DynValue.NewString(file));
        }

        return result;
    }

    [LuaMember("deleteDirectory")]
    public void DeleteDirectory(string path)
    {
        foreach (var file in _workingFiles.GetFilesAt(path))
        {
            Log.Info(FilesModule.DeletePrefix, $"Deleting file {file}");
            _workingFiles.DeleteFile(file);
        }

        Log.Info(FilesModule.DeletePrefix, $"Deleting directory {path}");
        _workingFiles.DeleteDirectory(path, true);
    }

    [LuaMember("delete")]
    public void DeleteFile(string path)
    {
        Log.Info(FilesModule.DeletePrefix, $"Deleting file {path}");
        _workingFiles.DeleteFile(path);
    }

    [LuaMember("copy")]
    public void CopyFile(string sourcePath, string destinationPath)
    {
        var sourceIsFile = _workingFiles.HasFile(sourcePath);
        var destinationIsFile = _workingFiles.HasFile(destinationPath);

        if (sourceIsFile && destinationIsFile)
        {
            var content = _workingFiles.ReadBytes(sourcePath);
            _workingFiles.CreateOrOverwriteFile(destinationPath);

            Log.Info(FilesModule.CopyPrefix, $"Copying file {sourcePath} to file {destinationPath}");
            _workingFiles.WriteToFileBytes(destinationPath, content);
        }

        if (sourceIsFile && !destinationIsFile)
        {
            var newFileSystem = _workingFiles.CreateDirectory(destinationPath);
            var content = _workingFiles.ReadBytes(sourcePath);
            var fileName = Path.GetFileName(sourcePath);

            Log.Info(FilesModule.CopyPrefix, $"Copying file {sourcePath} to directory {destinationPath}");
            newFileSystem.WriteToFileBytes(fileName, content);
        }

        if (!sourceIsFile && !destinationIsFile)
        {
            var sourceDirectory = _workingFiles.GetDirectory(sourcePath);
            var destinationDirectory = _workingFiles.GetDirectory(destinationPath);

            sourceDirectory.CreateDirectory();
            destinationDirectory.CreateDirectory();

            foreach (var sourceItemRelativePath in sourceDirectory.GetFilesAt("."))
            {
                Log.Info(FilesModule.CopyPrefix,
                    $"Copying file {sourceItemRelativePath} to directory {destinationPath}");
                var sourceItemContent = sourceDirectory.ReadBytes(sourceItemRelativePath);
                destinationDirectory.WriteToFileBytes(sourceItemRelativePath, sourceItemContent);
            }
        }

        if (!sourceIsFile && destinationIsFile)
        {
            throw new Exception($"Cannot copy from non-file {sourcePath} to file {destinationPath}");
        }
    }

    public override string LuaReadableName()
    {
        return "files";
    }

    protected override string LuaReadableDescription()
    {
        return "gives you access to the local filesystem with convenience functions";
    }
}
