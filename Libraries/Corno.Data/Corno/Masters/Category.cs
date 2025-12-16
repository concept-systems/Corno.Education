using System;
using Corno.Data.Common;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class Category : MasterModel
{
    public string ShortName { get; set; }
}