using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;

namespace Corno.OnlineExam.Controllers;

[Authorize]
public class EditorImageBrowserController : BaseFileBrowserController
{
    private readonly IThumbnailCreator _thumbnailCreator;

    private const int ThumbnailHeight = 80;
    private const int ThumbnailWidth = 80;

    protected EditorImageBrowserController()
    {
        _thumbnailCreator = new ThumbnailCreator(new FitImageResizer());
    }

    /// <summary>
    /// Gets the valid file extensions by which served files will be filtered.
    /// </summary>
    protected override string Filter => EditorImageBrowserSettings.DefaultFileTypes;

    public virtual bool AuthorizeThumbnail(string path)
    {
        return CanAccess(path);
    }

    /// <summary>
    /// Serves an image's thumbnail by given path.
    /// </summary>
    /// <param name="path">The path to the image.</param>
    /// <returns>Thumbnail of an image.</returns>
    /// <exception cref="HttpException">Throws 403 Forbidden if the <paramref name="path"/> is outside of the valid paths.</exception>
    /// <exception cref="HttpException">Throws 404 File Not Found if the <paramref name="path"/> refers to a non existing image.</exception>
    [OutputCache(Duration = 3600, VaryByParam = "path")]
    public virtual ActionResult Thumbnail(string path)
    {
        path = NormalizePath(path);

        if (!AuthorizeThumbnail(path)) throw new HttpException(403, "Forbidden");

        var physicalPath = Server.MapPath(path);

        if (!System.IO.File.Exists(physicalPath)) throw new HttpException(404, "File Not Found");

        Response.AddFileDependency(physicalPath);

        return CreateThumbnail(physicalPath);

    }

    private FileContentResult CreateThumbnail(string physicalPath)
    {
        using var fileStream = System.IO.File.OpenRead(physicalPath);
        var desiredSize = new ImageSize
        {
            Width = ThumbnailWidth,
            Height = ThumbnailHeight
        };

        const string contentType = "image/png";

        return File(_thumbnailCreator.Create(fileStream, desiredSize, contentType), contentType);
    }
}