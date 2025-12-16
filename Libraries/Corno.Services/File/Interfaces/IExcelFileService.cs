using Corno.Services.Corno.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Corno.Services.File.Interfaces;

public interface IExcelFileService<TEntity> : IService
{
    // Excel Mapper
    IEnumerable<TEntity> Read(HttpPostedFileBase file, int sheetIndex = 0, int headerRowNo = 0);
    IEnumerable<TEntity> Read(Stream fileStream, int sheetIndex = 0, int headerRowNo = 0);
    IEnumerable<TEntity> Read(string fileName, int sheetIndex = 0, int headerRowNo = 0);
    IEnumerable<TEntity> Read(string fileName, string sheetName = "Sheet1", int headerRowNo = 0);

    void Save(string fileName, string sheetName, IEnumerable<TEntity> data);
    void Save(string fileName, int sheetNo, IEnumerable<TEntity> data);

    MemoryStream GetMemoryStream(IEnumerable<TEntity> data);

    void ChangeCellColor(string filePath, int sheetNo, IEnumerable<string> data);
}