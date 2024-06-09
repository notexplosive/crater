using ExplogineCore;

namespace Crater;

public class PathResolver
{
    public PathResolver()
    {
        LocalLibrary = LocalFiles.GetDirectory("Library");
        LocalScripts = (LocalFiles.GetDirectory("Scripts") as RealFileSystem)!;
    }

    public IFileSystem LocalLibrary { get; }
    public RealFileSystem LocalScripts { get; }
    public RealFileSystem LocalFiles { get; } = new(AppDomain.CurrentDomain.BaseDirectory);
    public RealFileSystem WorkingFiles { get; } = new(Directory.GetCurrentDirectory());

    private static string? GetPathIfFileExists(RealFileSystem files, string pathWithExtension)
    {
        if (files.HasFile(pathWithExtension))
        {
            return files.ToAbsolutePath(pathWithExtension);
        }

        return null;
    }

    private string? ResolveExtension(RealFileSystem files, string path)
    {
        if (files.HasFile(path))
        {
            return files.ToAbsolutePath(path);
        }

        return
            PathResolver.GetPathIfFileExists(files, path)
            ?? PathResolver.GetPathIfFileExists(files, path + ".crater")
            ?? PathResolver.GetPathIfFileExists(files, path + ".lua")
            ;
    }

    public string? Resolve(string path)
    {
        return
            ResolveExtension(WorkingFiles, path)
            ?? ResolveExtension(LocalScripts, path)
            ;
    }
}
