using System.ComponentModel.DataAnnotations;

namespace MetaFrm.Management.Razor.Models
{
    /// <summary>
    /// MenuModel
    /// </summary>
    public class MenuModel
    {
        /// <summary>
        /// CHK
        /// </summary>
        [Display(Name = "체크")]
        public bool CHK { get; set; } = true;

        /// <summary>
        /// MENU_ID
        /// </summary>
        public int? MENU_ID { get; set; }

        /// <summary>
        /// PARENT_NAME
        /// </summary>
        public string? PARENT_NAME { get; set; }

        /// <summary>
        /// NAME
        /// </summary>
        public string? NAME { get; set; }

        /// <summary>
        /// DESCRIPTION
        /// </summary>
        public string? DESCRIPTION { get; set; }

        /// <summary>
        /// NAMESPACE
        /// </summary>
        public string? NAMESPACE { get; set; }
    }
}