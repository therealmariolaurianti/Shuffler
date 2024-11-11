using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using ShufflerPro.Database;
using ShufflerPro.Result;

namespace ShufflerPro.Installer;

public partial class Runner
{
    public void Run()
    {
        CheckForDotNetRuntimes()
            .IfFail(Console.WriteLine)
            .IfSuccess(_ => CheckForRootFile()
                .IfFail(Console.WriteLine)
                .IfSuccess(_ => Console.WriteLine("Ready to install")));
    }

    private NewResult<NewUnit> CheckForRootFile()
    {
        return RootFinder.FindRoot()
            .IfFail(_ =>
            {
                var appPath = Assembly.GetExecutingAssembly().Location;
                var appDirectory = Path.GetDirectoryName(appPath);
                if(appDirectory is null)
                    return NewResultExtensions.CreateFail<string>("Could not find app directory");

                var rootPath = Path.Combine(appDirectory, ".root");
                File.Create(rootPath);
                
                Console.WriteLine(rootPath);
                Console.Read();

                return rootPath;
            })
            .Map(_ => NewUnit.Default);
    }

    private NewResult<NewUnit> CheckForDotNetRuntimes()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "--list-runtimes",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        using var reader = process?.StandardOutput;

        if (reader == null)
            return NewResultExtensions.CreateFail<NewUnit>("Failed to start dotnet process");

        var output = reader.ReadToEnd();
        var runtimes = output
            .Split(Environment.NewLine)
            .Where(r => !string.IsNullOrEmpty(r)).ToList();

        foreach (var runtime in runtimes.ToList())
        {
            var actual = MyRegex().Replace(runtime, "").Trim();
            var matchingRuntime = CurrentRuntimes.Runtimes.SingleOrDefault(rt => rt == actual);
            if (matchingRuntime != null)
                runtimes.Remove(runtime);
        }

        if (runtimes.Count != 0)
            return NewResultExtensions.CreateFail<NewUnit>("Missing runtimes: " + 
                                                           Environment.NewLine +
                                                           string.Join($",{Environment.NewLine}", runtimes.Select(r => $"'{r}'")));

        return NewUnit.Default;
    }

    [GeneratedRegex(@"\[(.*?)\]")]
    private static partial Regex MyRegex();
}

internal static class CurrentRuntimes
{
    // @formatter:off
    public static string[] Runtimes => AspNetCore.Concat(NetCore).Concat(WindowsDesktop).ToArray();
    private static readonly string[] AspNetCore = ["Microsoft.AspNetCore.App 7.0.15", "Microsoft.AspNetCore.App 8.0.2"];
    private static readonly string[] NetCore = ["Microsoft.NETCore.App 7.0.15", "Microsoft.NETCore.App 8.0.2"];
    private static readonly string[] WindowsDesktop = ["Microsoft.WindowsDesktop.App 7.0.15", "Microsoft.WindowsDesktop.App 8.0.2"];
    // @formatter:on
}