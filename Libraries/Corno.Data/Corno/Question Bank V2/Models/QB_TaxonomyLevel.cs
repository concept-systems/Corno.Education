using System;
using System.ComponentModel.DataAnnotations;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank_V2.Models
{
    [Serializable]
    public class QB_TaxonomyLevel : BaseModel
    {
        #region -- Properties --

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        [Required]
        public int Level { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(500)]
        public string Keywords { get; set; }

        [StringLength(7)]
        public string ColorCode { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        #endregion

        #region -- Methods --

        public override void Copy(BaseModel other)
        {
            if (other is not QB_TaxonomyLevel model) return;

            base.Copy(other);

            Name = model.Name;
            Code = model.Code;
            Level = model.Level;
            Description = model.Description;
            Keywords = model.Keywords;
            ColorCode = model.ColorCode;
            DisplayOrder = model.DisplayOrder;
            IsActive = model.IsActive;
        }

        #endregion
    }
}
