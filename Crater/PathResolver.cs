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

    private string? CheckPath(RealFileSystem files, string path)
    {
        if (files.HasFile(path))
        {
            return files.ToAbsolutePath(path);
        }

        return
            PathResolver.CheckIndividual(files, path)
            ?? PathResolver.CheckIndividual(files, path + ".crater")
            ?? PathResolver.CheckIndividual(files, path + ".lua")
            ;
    }

    private static string? CheckIndividual(RealFileSystem files, string pathWithExtension)
    {
        if (files.HasFile(pathWithExtension))
        {
            return files.ToAbsolutePath(pathWithExtension);
        }

        return null;
    }

    public string? Deduce(string path)
    {
        var working = CheckPath(WorkingFiles, path);
        var local = CheckPath(LocalScripts, path);

        return working ?? local;
    }
}
