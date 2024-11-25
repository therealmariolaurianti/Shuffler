using System.Diagnostics;
using System.Reflection;
using HtmlAgilityPack;
using ShufflerPro.Database;
using ShufflerPro.Result;

namespace ShufflerPro.Web;

public class UpdateController
{
    private const string _updateLink =
        "https://github.com/therealmariolaurianti/Shuffler/releases/download/Release/shufflersetup.exe";

    private const string _versionCheckLink = "https://therealmariolaurianti.github.io/Shuffler/Web/version.html";
    private static readonly Version? _currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
    private string _rootPath = string.Empty;

    public UpdateController()
    {
        RootFinder.FindRoot()
            .Do(rootPath => _rootPath = $@"{rootPath}\Update");
    }

    private string _updatePath => _rootPath + @"\shufflersetup.exe";

    public async Task<NewResult<bool>> CheckIfUpdateIsAvailable()
    {
        return await CheckForDownloadedUpdate()
            .Bind(async doesUpdateExist =>
            {
                if (doesUpdateExist)
                    return true;

                return await GetLatestVersion()
                    .Bind(VerifyVersion)
                    .Bind(async _ => await DownloadUpdateAsync(_updateLink));
            });
    }

    private NewResult<bool> CheckForDownloadedUpdate()
    {
        return File.Exists(_updatePath);
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
        {
            //TODO close application
            //updateProcess.WaitForExit();
        }
        else
        {
            return NewResultExtensions.CreateFail<NewUnit>(new Exception("Update process is null."));
        }

        return NewUnit.Default;
    }

    private static NewResult<NewUnit> VerifyVersion(Version latestVersion)
    {
        var verifyVersion = _currentVersion != latestVersion
            ? NewUnit.Default
            : NewResultExtensions.CreateFail<NewUnit>("Up to date");
        return verifyVersion;
    }

    private static async Task<NewResult<Version>> GetLatestVersion()
    {
        try
        {
            using var client = new HttpClient();
            var page = await client.GetStringAsync(_versionCheckLink);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(page);

            var stringValue = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='current-version']").InnerHtml;
            var version = stringValue.Replace("Current Version: ", "").Trim();

            return new Version(version);
        }
        catch (Exception ex)
        {
            return NewResultExtensions.CreateFail<Version>(ex);
        }
    }

    private async Task<NewResult<bool>> DownloadUpdateAsync(string downloadUrl)
    {
        try
        {
            if (!Directory.Exists(_rootPath))
                Directory.CreateDirectory(_rootPath);

            using var client = new HttpClient();

            var response = await client.GetStreamAsync(downloadUrl);
            await using (var fileStream = new FileStream(_updatePath, FileMode.OpenOrCreate))
            {
                await response.CopyToAsync(fileStream).ConfigureAwait(true);
            }

            return true;
        }
        catch (Exception ex)
        {
            return NewResultExtensions.CreateFail<bool>(ex);
        }
    }
}