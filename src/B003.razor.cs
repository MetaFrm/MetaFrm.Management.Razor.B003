﻿using MetaFrm.Database;
using MetaFrm.Extensions;
using MetaFrm.Management.Razor.Models;
using MetaFrm.Management.Razor.ViewModels;
using MetaFrm.Razor.Essentials;
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

        internal MenuPermissionsModel SelectItem = new();
        #endregion


        #region Init
        /// <summary>
        /// OnAfterRenderAsync
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!this.AuthState.IsLogin())
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
                    Token = this.AuthState.Token()
                };
                serviceData["1"].CommandText = this.GetAttribute("Search");
                serviceData["1"].AddParameter(nameof(this.B003ViewModel.SearchModel.SEARCH_TEXT), DbType.NVarChar, 4000, this.B003ViewModel.SearchModel.SEARCH_TEXT);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.AuthState.UserID());
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
                        this.ModalShow("경고", response.Message, new() { { "확인", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("예외가 발생했습니다.", e.Message, new() { { "확인", Btn.Danger } }, null);
            }
            finally
            {
                this.B003ViewModel.IsBusy = false;
                this.SetSession(nameof(B003ViewModel), this.B003ViewModel);
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
                    Token = this.AuthState.Token()
                };
                serviceData["1"].CommandText = this.GetAttribute("SearchMenu");
                serviceData["1"].AddParameter(nameof(this.SelectItem.RESPONSIBILITY_ID), DbType.Int, 3, this.SelectItem.RESPONSIBILITY_ID);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.AuthState.UserID());

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
                        this.ModalShow("경고", response.Message, new() { { "확인", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("예외가 발생했습니다.", e.Message, new() { { "확인", Btn.Danger } }, null);
            }
            finally
            {
                this.B003ViewModel.IsBusy = false;
                this.SetSession(nameof(B003ViewModel), this.B003ViewModel);
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
                    Token = this.AuthState.Token()
                };
                serviceData["1"].CommandText = this.GetAttribute("Delete");
                serviceData["1"].AddParameter(nameof(this.SelectItem.RESPONSIBILITY_ID), DbType.Int, 3, this.SelectItem.RESPONSIBILITY_ID);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.AuthState.UserID());

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
                        serviceData["2"].SetValue("USER_ID", this.AuthState.UserID());
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

                    this.ToastShow("완료", this.Localization["{0}이(가) 등록되었습니다.", this.GetAttribute("Title")], Alert.ToastDuration.Long);
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("경고", response.Message, new() { { "확인", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("예외가 발생했습니다.", e.Message, new() { { "확인", Btn.Danger } }, null);
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
        }
        #endregion
    }
}