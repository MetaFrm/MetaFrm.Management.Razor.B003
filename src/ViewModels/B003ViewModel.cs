using MetaFrm.Management.Razor.Models;
using MetaFrm.MVVM;

namespace MetaFrm.Management.Razor.ViewModels
{
    /// <summary>
    /// C001ViewModel
    /// </summary>
    public partial class B003ViewModel : BaseViewModel
    {
        /// <summary>
        /// SearchModel
        /// </summary>
        public SearchModel SearchModel { get; set; } = new();

        /// <summary>
        /// SelectResultModel
        /// </summary>
        public List<MenuPermissionsModel> SelectResultModel { get; set; } = new List<MenuPermissionsModel>();

        /// <summary>
        /// C001ViewModel
        /// </summary>
        public B003ViewModel()
        {

        }
    }
}