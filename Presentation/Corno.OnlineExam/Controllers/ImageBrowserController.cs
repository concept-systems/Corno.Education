using System.IO;
using System.Web.Mvc;

namespace Corno.OnlineExam.Controllers;

[Authorize]
public class ImageBrowserController : EditorImageBrowserController
{
    private const string ContentFolderRoot = "~/Content/";
    private const string PrettyName = "Images/";
    private static readonly string[] FoldersToCopy = new[] { "~/Content/shared/" };

    //public ImageBrowserController()
    //{

    //}

    /// <summary>
    /// Gets the base paths from which content will be served.
    /// </summary>
    protected override string ContentPath => CreateUserFolder();

    private string CreateUserFolder()
    {
        var virtualPath = Path.Combine(ContentFolderRoot, "UserFiles", PrettyName);

        var path = Server.MapPath(virtualPath);
        if (Directory.Exists(path)) return virtualPath;

        Directory.CreateDirectory(path);
        foreach (var sourceFolder in FoldersToCopy)
        {
            CopyFolder(Server.MapPath(sourceFolder), path);
        }
        return virtualPath;
    }

    private static void CopyFolder(string source, string destination)
    {
        if (!Directory.Exists(destination))
            Directory.CreateDirectory(destination);

        foreach (var file in Directory.EnumerateFiles(source))
        {
            var dest = Path.Combine(destination, Path.GetFileName(file));
            System.IO.File.Copy(file, dest);
        }

        foreach (var folder in Directory.EnumerateDirectories(source))
        {
            var dest = Path.Combine(destination, Path.GetFileName(folder));
            CopyFolder(folder, dest);
        }
    }
}