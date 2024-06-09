namespace Crater.ExternalPrograms
{
    public class DotnetProgram : ExternalProgram
    {
        public DotnetProgram() : base("dotnet")
        {
        }

        /// <summary>
        /// This is the normal publish we used to run from release_build.bat
        /// </summary>
        /// <returns></returns>
        public ProgramOutput NormalPublish(string csprojPath, string absoluteOutputPath)
        {
            return RunWithArgs(ProgramOutputLevel.AllowProgramToEmitToConsole,
                        "publish",
                        csprojPath,
                        "-c", "Release",
                        "-r", "win-x64",
                        "/p:PublishReadyToRun=false",
                        "/p:TieredCompilation=false",
                        "--self-contained",
                        "--output", absoluteOutputPath);
        }

        public ProgramOutput Version()
        {
            return RunWithArgs(ProgramOutputLevel.AllowProgramToEmitToConsole, "--version");
        }

        public bool Exists()
        {
            return RunWithArgs(ProgramOutputLevel.SuppressProgramFromEmittingToConsole, "--version").WasSuccessful;
        }

        public void AddToSln(ProgramOutputLevel outputLevel, string path)
        {
            RunWithArgs(outputLevel, "sln", "add", path);
        }

        public void NewSln(ProgramOutputLevel outputLevel)
        {
            RunWithArgs(outputLevel, "new", "sln");
        }
    }
}
