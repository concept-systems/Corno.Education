using Corno.Data.Common;
using Corno.Data.Corno.Masters;
using Corno.Data.ViewModels;
using Corno.Globals.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace Corno.Data.Corno;

/*[Serializable]*/
public class Link : UniversityBaseModel
{
    #region -- Constructors -- 
    public Link()
    {
        LinkDetails = new List<LinkDetail>();

        NotMapped = new NotMapped();
    }
    #endregion

    public int? FormTypeId { get; set; }

    [NotMapped]
    public NotMapped NotMapped { get; set; }

    public List<LinkDetail> LinkDetails { get; set; }

    #region -- Methods --

    public IEnumerable<LinkDetail> GetSelectedDetails()
    {
        var selectedLinks = LinkDetails.Where(d =>
            d.Selected && d.Status == StatusConstants.Active).ToList();
        if (selectedLinks.Count <= 0)
            throw new Exception("Please, select at least 1 row.");
        return selectedLinks;
        /*return LinkDetails.Where(d =>
            d.Selected && d.Status == StatusConstants.Active);*/
    }
    #endregion

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not Link newLink) return false;

        foreach (var linkDetail in LinkDetails)
        {
            var newLinkDetail = newLink.LinkDetails.FirstOrDefault(d =>
                d.Id == linkDetail.Id);
            linkDetail.Copy(newLinkDetail);
        }

        // Add new entries
        var newLinkDetails = newLink.LinkDetails.Where(d => d.Id <= 0).ToList();
        LinkDetails.AddRange(newLinkDetails);
        // Remove items from list1 that are not in list2
        LinkDetails.RemoveAll(x => newLink.LinkDetails.All(y => y.Id != x.Id));

        return true;
    }
    #endregion
}