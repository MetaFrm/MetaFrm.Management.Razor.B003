using System.ComponentModel.DataAnnotations;

namespace MetaFrm.Management.Razor.Models
{
    /// <summary>
    /// MenuPermissionsModel
    /// </summary>
    public class MenuPermissionsModel
    {
        /// <summary>
        /// RESPONSIBILITY_ID
        /// </summary>
        public decimal? RESPONSIBILITY_ID { get; set; }

        /// <summary>
        /// NAME
        /// </summary>
        [Required]
        [Display(Name = "Permissions NAME")]
        public string? NAME { get; set; }

        /// <summary>
        /// INACTIVE_DATE
        /// </summary>
        [Display(Name = "Inactive date")]
        public DateTime? INACTIVE_DATE { get; set; } = DateTime.Now.AddYears(100);

        /// <summary>
        /// Menu
        /// </summary>
        public List<MenuModel> Menu { get; set; } = new();
    }
}