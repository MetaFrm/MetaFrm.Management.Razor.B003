using MetaFrm.Database;
using MetaFrm.Extensions;
using MetaFrm.Management.Razor.Models;
using MetaFrm.Management.Razor.ViewModels;
using MetaFrm.Razor.DataGrid;
using MetaFrm.Razor.Group;
using MetaFrm.Service;
using MetaFrm.Web.Bootstrap;
using Microsoft.AspNetCore.Components.Web;

namespace MetaFrm.Management.Razor
{
    /// <summary>
    /// B003
    /// </summary>
    public partial class B003
    {
        #region Variable
        internal B003ViewModel B003ViewModel { get; set; } = Factory.CreateViewModel<B003ViewModel>();

        internal DataGridControl<MenuPermissionsModel>? DataGridControl;
        internal List<ColumnDefinitions>? ColumnDefinitions;

        internal List<ColumnDefinitions>? ColumnDefinitionsMenu;

        internal MenuPermissionsModel SelectItem = new();

        internal GroupWindowStatus GroupWindowStatus = GroupWindowStatus.Close;
        #endregion


        #region Init
        /// <summary>
        /// OnInitialized
        /// </summary>
        protected override void OnInitialized()
        {
            if (this.ColumnDefinitions == null)
            {
                this.ColumnDefinitions = new();
                this.ColumnDefinitions.AddRange(new ColumnDefinitions[] {
                    new ColumnDefinitions{ DataField = nameof(MenuPermissionsModel.NAME), Caption = "Permissions", DataType = DbType.NVarChar, Class = "text-break", SortDirection = SortDirection.Ascending },
                    new ColumnDefinitions{ DataField = nameof(MenuPermissionsModel.INACTIVE_DATE), Caption = "Inactive", DataType = DbType.DateTime, Class = "text-break", Alignment = Alignment.Center, SortDirection = SortDirection.Normal, Format = "yyyy-MM-dd HH:mm" }});
            }

            if (this.ColumnDefinitionsMenu == null)
            {
                this.ColumnDefinitionsMenu = new();
                this.ColumnDefinitionsMenu.AddRange(new ColumnDefinitions[] {
                    new ColumnDefinitions{ DataField = nameof(MenuModel.CHK), Caption = "", DataType = DbType.Bit, SortDirection = SortDirection.NotSet, Alignment = Alignment.Center, Editable = true },
                    new ColumnDefinitions{ DataField = nameof(MenuModel.NAME), Caption = "Menu", DataType = DbType.NVarChar, Class = "text-break", SortDirection = SortDirection.NotSet },
                    new ColumnDefinitions{ DataField = nameof(MenuModel.PARENT_NAME), Caption = "Parent", DataType = DbType.NVarChar, Class = "text-break", SortDirection = SortDirection.NotSet },
                    new ColumnDefinitions{ DataField = nameof(MenuModel.NAMESPACE), Caption = "Namespace", DataType = DbType.NVarChar, Class = "text-break", SortDirection = SortDirection.NotSet } });
            }
        }

        /// <summary>
        /// OnAfterRenderAsync
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!this.IsLogin())
                    this.Navigation?.NavigateTo("/", true);

                this.B003ViewModel = await this.GetSession<B003ViewModel>(nameof(this.B003ViewModel));

                this.Search();

                this.StateHasChanged();
            }
        }
        #endregion


        #region IO
        private void Init()
        {
            this.SearchMenu();
        }

        private void OnSearch()
        {
            if (this.DataGridControl != null)
                this.DataGridControl.CurrentPageNumber = 1;

            this.Search();
        }
        private void Search()
        {
            Response response;

            try
            {
                if (this.B003ViewModel.IsBusy) return;

                this.B003ViewModel.IsBusy = true;

                ServiceData serviceData = new()
                {
                    Token = this.UserClaim("Token")
                };
                serviceData["1"].CommandText = this.GetAttribute("Search");
                serviceData["1"].AddParameter(nameof(this.B003ViewModel.SearchModel.SEARCH_TEXT), DbType.NVarChar, 4000, this.B003ViewModel.SearchModel.SEARCH_TEXT);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.UserClaim("Account.USER_ID").ToInt());
                serviceData["1"].AddParameter("PAGE_NO", DbType.Int, 3, this.DataGridControl != null ? this.DataGridControl.CurrentPageNumber : 1);
                serviceData["1"].AddParameter("PAGE_SIZE", DbType.Int, 3, this.DataGridControl != null && this.DataGridControl.PagingEnabled ? this.DataGridControl.PagingSize : int.MaxValue);

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    this.B003ViewModel.SelectResultModel.Clear();

                    if (response.DataSet != null && response.DataSet.DataTables.Count > 0)
                    {
                        foreach (var datarow in response.DataSet.DataTables[0].DataRows)
                        {
                            this.B003ViewModel.SelectResultModel.Add(new MenuPermissionsModel
                            {
                                RESPONSIBILITY_ID = datarow.Int(nameof(MenuPermissionsModel.RESPONSIBILITY_ID)),
                                NAME = datarow.String(nameof(MenuPermissionsModel.NAME)),
                                INACTIVE_DATE = datarow.DateTime(nameof(MenuPermissionsModel.INACTIVE_DATE)),
                            });
                        }
                    }
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.B003ViewModel.IsBusy = false;
#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                this.SetSession(nameof(B003ViewModel), this.B003ViewModel);
#pragma warning restore CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
            }
        }
        private void SearchMenu()
        {
            Response response;

            try
            {
                if (this.SelectItem == null || this.SelectItem.RESPONSIBILITY_ID == null) return;
                if (this.B003ViewModel.IsBusy) return;

                this.B003ViewModel.IsBusy = true;

                ServiceData serviceData = new()
                {
                    Token = this.UserClaim("Token")
                };
                serviceData["1"].CommandText = this.GetAttribute("SearchMenu");
                serviceData["1"].AddParameter(nameof(this.SelectItem.RESPONSIBILITY_ID), DbType.Int, 3, this.SelectItem.RESPONSIBILITY_ID);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.UserClaim("Account.USER_ID").ToInt());

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    this.SelectItem.Menu.Clear();

                    if (response.DataSet != null && response.DataSet.DataTables.Count > 0)
                    {
                        foreach (var datarow in response.DataSet.DataTables[0].DataRows)
                        {
                            this.SelectItem.Menu.Add(new MenuModel
                            {
                                CHK = datarow.Int(nameof(MenuModel.CHK)) == 1,
                                MENU_ID = datarow.Int(nameof(MenuModel.MENU_ID)),
                                PARENT_NAME = datarow.String(nameof(MenuModel.PARENT_NAME)),
                                NAME = datarow.String(nameof(MenuModel.NAME)),
                                DESCRIPTION = datarow.String(nameof(MenuModel.DESCRIPTION)),
                                NAMESPACE = datarow.String(nameof(MenuModel.NAMESPACE)),
                            });
                        }
                    }
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.B003ViewModel.IsBusy = false;
#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                this.SetSession(nameof(B003ViewModel), this.B003ViewModel);
#pragma warning restore CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
            }
        }

        private void Save()
        {
            Response? response;
            string? value;

            response = null;

            try
            {
                if (this.B003ViewModel.IsBusy)
                    return;

                if (this.SelectItem == null)
                    return;

                this.B003ViewModel.IsBusy = true;

                ServiceData serviceData = new()
                {
                    TransactionScope = true,
                    Token = this.UserClaim("Token")
                };
                serviceData["1"].CommandText = this.GetAttribute("Delete");
                serviceData["1"].AddParameter(nameof(this.SelectItem.RESPONSIBILITY_ID), DbType.Int, 3, this.SelectItem.RESPONSIBILITY_ID);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.UserClaim("Account.USER_ID").ToInt());

                serviceData["2"].CommandText = this.GetAttribute("Save");
                serviceData["2"].AddParameter(nameof(this.SelectItem.RESPONSIBILITY_ID), DbType.Int, 3);
                serviceData["2"].AddParameter(nameof(MenuModel.MENU_ID), DbType.Int, 3);
                serviceData["2"].AddParameter("USER_ID", DbType.Int, 3);

                foreach (MenuModel menu in this.SelectItem.Menu)
                {
                    if (menu.CHK)
                    {
                        serviceData["2"].NewRow();
                        serviceData["2"].SetValue(nameof(this.SelectItem.RESPONSIBILITY_ID), this.SelectItem.RESPONSIBILITY_ID);
                        serviceData["2"].SetValue(nameof(menu.MENU_ID), menu.MENU_ID);
                        serviceData["2"].SetValue("USER_ID", this.UserClaim("Account.USER_ID").ToInt());
                    }
                }

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    if (response.DataSet != null && response.DataSet.DataTables.Count > 0 && response.DataSet.DataTables[0].DataRows.Count > 0 && this.SelectItem != null && this.SelectItem.RESPONSIBILITY_ID == null)
                    {
                        value = response.DataSet.DataTables[0].DataRows[0].String("Value");

                        if (value != null)
                            this.SelectItem.RESPONSIBILITY_ID = value.ToInt();
                    }

                    this.ToastShow("Completed", $"{this.GetAttribute("Title")} registered successfully.", Alert.ToastDuration.Long);
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.B003ViewModel.IsBusy = false;

                if (response != null && response.Status == Status.OK)
                {
                    this.Search();
                    this.StateHasChanged();
                }
            }
        }
        #endregion


        #region Event
        private void SearchKeydown(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                this.OnSearch();
            }
        }

        private void RowModify(MenuPermissionsModel item)
        {
            this.SelectItem = new()
            {
                RESPONSIBILITY_ID = item.RESPONSIBILITY_ID,
                NAME = item.NAME,
                INACTIVE_DATE = item.INACTIVE_DATE,
            };

            this.SearchMenu();

            this.GroupWindowStatus = GroupWindowStatus.Maximize;
        }

        private void Close()
        {
            this.GroupWindowStatus = GroupWindowStatus.Close;
        }
        #endregion
    }
}