using System;
using System.Web;
using System.Web.Mvc;
using Telerik.Reporting.Services.Engine;
using Telerik.Reporting.Services.WebApi;
using CacheFactory = Telerik.Reporting.Services.Engine.CacheFactory;
using ICache = Telerik.Reporting.Cache.Interfaces.ICache;

namespace Corno.OnlineExam.Controllers;

[Authorize]
public class ReportsController : ReportsControllerBase
{
    [Obsolete("CreateReportResolver method is now obsolete. Please provide service setup using the Telerik.Reporting.Services.WebApi.ReportsControllerBase.ReportServiceConfiguration property.")]
    protected override IReportResolver CreateReportResolver()
    {
        var reportsPath = HttpContext.Current.Server.MapPath(@"~/Reports");
        return new ReportFileResolver(reportsPath)
            .AddFallbackResolver(new ReportTypeResolver());

        //var appPath = HttpContext.Current.Server.MapPath("~/");
        //return new ReportFileResolver(appPath)
        //.AddFallbackResolver(new ReportTypeResolver());
    }

    [Obsolete("CreateReportResolver method is now obsolete. Please provide service setup using the Telerik.Reporting.Services.WebApi.ReportsControllerBase.ReportServiceConfiguration property.")]
    protected override ICache CreateCache()
    {
        return CacheFactory.CreateFileCache();
    }
}