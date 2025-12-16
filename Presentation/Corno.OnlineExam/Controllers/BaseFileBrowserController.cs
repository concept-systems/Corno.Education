using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.Infrastructure;
using Kendo.Mvc.Infrastructure.Implementation;
using Kendo.Mvc.UI;

namespace Corno.OnlineExam.Controllers;

[Authorize]
public class BaseFileBrowserController : BaseController
{
    private readonly IDirectoryBrowser _directoryBrowser;
    private readonly IDirectoryPermission _permission;
    private readonly IVirtualPathProvider _pathProvider;

    protected BaseFileBrowserController()
    {
        _directoryBrowser = new DirectoryBrowser();
        _permission = new DirectoryPermission();
        _pathProvider = new VirtualPathProviderWrapper();
    }

    /// <summary>
    /// Gets the base path from which content will be served.
    /// </summary>
    protected virtual string ContentPath
    {
        get;
    }

    /// <summary>
    /// Gets the valid file extensions by which served files will be filtered.
    /// </summary>
    protected virtual string Filter => "*.*";

    /// <summary>
    /// Determines if content of a given path can be browsed.
    /// </summary>
    /// <param name="path">The path which will be browsed.</param>
    /// <returns>true if browsing is allowed, otherwise false.</returns>
    public bool AuthorizeRead(string path)
    {
        return CanAccess(path);
    }

    protected bool CanAccess(string path)
    {
        return _permission.CanAccess(_pathProvider.ToAbsolute(ContentPath), path);
    }

    protected string NormalizePath(string path)
    {
        return string.IsNullOrEmpty(path) ? _pathProvider.ToAbsolute(ContentPath) : _pathProvider.CombinePaths(_pathProvider.ToAbsolute(ContentPath), path);
    }

    //[HttpGet]
    public virtual JsonResult Read(string path)
    {
        path = NormalizePath(path);

        if (!AuthorizeRead(path)) throw new HttpException(403, "Forbidden");
        try
        {
            _directoryBrowser.Server = Server;

            var result = _directoryBrowser.GetFiles(path, Filter)
                .Concat(_directoryBrowser.GetDirectories(path));

            return Json(result);
        }
        catch (DirectoryNotFoundException)
        {
            throw new HttpException(404, "File Not Found");
        }
    }

    /// <summary>
    /// Deletes a entry.
    /// </summary>
    /// <param name="path">The path to the entry.</param>
    /// <param name="entry">The entry.</param>
    /// <returns>An empty <see cref="ContentResult"/>.</returns>
    /// <exception cref="HttpException">Forbidden</exception>
    [AcceptVerbs(HttpVerbs.Post)]
    public virtual ActionResult Destroy(string path, FileBrowserEntry entry)
    {
        path = NormalizePath(path);

        if (entry == null) throw new HttpException(404, "File Not Found");

        path = _pathProvider.CombinePaths(path, entry.Name);
        if (entry.EntryType == FileBrowserEntryType.File)
        {
            DeleteFile(path);
        }
        else
        {
            DeleteDirectory(path);
        }

        return Json(Array.Empty<object>());
    }

    /// <summary>
    /// Determines if a file can be deleted.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>true if file can be deleted, otherwise false.</returns>
    public bool AuthorizeDeleteFile(string path)
    {
        return CanAccess(path);
    }

    /// <summary>
    /// Determines if a folder can be deleted.
    /// </summary>
    /// <param name="path">The path to the folder.</param>
    /// <returns>true if folder can be deleted, otherwise false.</returns>
    public bool AuthorizeDeleteDirectory(string path)
    {
        return CanAccess(path);
    }

    protected void DeleteFile(string path)
    {
        if (!AuthorizeDeleteFile(path))
        {
            throw new HttpException(403, "Forbidden");
        }

        var physicalPath = Server.MapPath(path);

        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }
    }

    protected void DeleteDirectory(string path)
    {
        if (!AuthorizeDeleteDirectory(path))
        {
            throw new HttpException(403, "Forbidden");
        }

        var physicalPath = Server.MapPath(path);

        if (Directory.Exists(physicalPath))
        {
            Directory.Delete(physicalPath, true);
        }
    }

    /// <summary>
    /// Determines if a folder can be created.
    /// </summary>
    /// <param name="path">The path to the parent folder in which the folder should be created.</param>
    /// <param name="name">Name of the folder.</param>
    /// <returns>true if folder can be created, otherwise false.</returns>
    public bool AuthorizeCreateDirectory(string path, string name)
    {
        return CanAccess(path);
    }

    /// <summary>
    /// Creates a folder with a given entry.
    /// </summary>
    /// <param name="path">The path to the parent folder in which the folder should be created.</param>
    /// <param name="entry">The entry.</param>
    /// <returns>An empty <see cref="ContentResult"/>.</returns>
    /// <exception cref="HttpException">Forbidden</exception>
    [AcceptVerbs(HttpVerbs.Post)]
    public virtual ActionResult Create(string path, FileBrowserEntry entry)
    {
        path = NormalizePath(path);
        var name = entry.Name;

        if (name.HasValue() && AuthorizeCreateDirectory(path, name))
        {
            var physicalPath = Path.Combine(Server.MapPath(path), name);

            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }

            return Json(entry);
        }

        throw new HttpException(403, "Forbidden");
    }

    /// <summary>
    /// Determines if a file can be uploaded to a given path.
    /// </summary>
    /// <param name="path">The path to which the file should be uploaded.</param>
    /// <param name="file">The file which should be uploaded.</param>
    /// <returns>true if the upload is allowed, otherwise false.</returns>
    public bool AuthorizeUpload(string path, HttpPostedFileBase file)
    {
        return CanAccess(path) && IsValidFile(file.FileName);
    }

    private bool IsValidFile(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var allowedExtensions = Filter.Split(',');

        return allowedExtensions.Any(e => e.Equals("*.*") || e.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// Uploads a file to a given path.
    /// </summary>
    /// <param name="path">The path to which the file should be uploaded.</param>
    /// <param name="file">The file which should be uploaded.</param>
    /// <returns>A <see cref="JsonResult"/> containing the uploaded file's size and name.</returns>
    /// <exception cref="HttpException">Forbidden</exception>
    [AcceptVerbs(HttpVerbs.Post)]
    public virtual ActionResult Upload(string path, HttpPostedFileBase file)
    {
        path = NormalizePath(path);
        var fileName = Path.GetFileName(file.FileName);

        if (AuthorizeUpload(path, file))
        {
            file.SaveAs(Path.Combine(Server.MapPath(path), fileName));

            return Json(new FileBrowserEntry
            {
                Size = file.ContentLength,
                Name = fileName
            }, "text/plain");
        }

        throw new HttpException(403, "Forbidden");
    }
}