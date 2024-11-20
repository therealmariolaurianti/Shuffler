using System.Diagnostics;
using System.Reflection;
using ShufflerPro.Database;
using ShufflerPro.Result;

namespace ShufflerPro.Web;

public class UpdateController
{
    private const string _updateLink = "https://link.testfile.org/500MB"; //todo path to new exe
    private const string _versionCheckLink = "https://link.testfile.org/500MB"; //todo path to current version
    private static readonly Version? _currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
    private string _rootPath = string.Empty;

    public UpdateController()
    {
        RootFinder.FindRoot()
            .Do(rootPath => _rootPath = rootPath);
    }

    public bool IsUpdateReady { get; internal set; }

    private string _updatePath => _rootPath + @"\shufflersetup.exe";

    public async Task<NewResult<NewUnit>> CheckForUpdate()
    {
        return await GetLatestVersion()
            .Bind(VerifyVersion)
            .IfFailAsync(async _ => await DownloadUpdateAsync(_updateLink)
                .IfSuccess(_ => IsUpdateReady = true));
    }

    public NewResult<NewUnit> ApplyUpdate()
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = _updatePath,
            Arguments = $"/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP- /DIR=\"{_rootPath}\"",
            WorkingDirectory = _rootPath,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var updateProcess = Process.Start(processStartInfo);

        if (updateProcess != null)
            updateProcess.WaitForExit();
        else
            return NewResultExtensions.CreateFail<NewUnit>(new Exception("Update process is null."));

        return NewUnit.Default;
    }

    private NewResult<NewUnit> VerifyVersion(Version latestVersion)
    {
        return _currentVersion != latestVersion
            ? NewResultExtensions.CreateFail<NewUnit>(new Exception("New version available."))
            : NewUnit.Default;
    }

    private async Task<NewResult<Version>> GetLatestVersion()
    {
        try
        {
            using var client = new HttpClient();

            var response = await client.GetStringAsync(_versionCheckLink);
            return new Version(response);
        }
        catch (Exception ex)
        {
            return NewResultExtensions.CreateFail<Version>(ex);
        }
    }

    private async Task<NewResult<NewUnit>> DownloadUpdateAsync(string downloadUrl)
    {
        try
        {
            using var client = new HttpClient();

            var response = await client.GetStreamAsync(downloadUrl);
            await using (var fileStream = new FileStream(_updatePath, FileMode.OpenOrCreate))
            {
                await response.CopyToAsync(fileStream);
            }

            return await NewUnit.DefaultAsync;
        }
        catch (Exception ex)
        {
            return NewResultExtensions.CreateFail<NewUnit>(ex);
        }
    }
}