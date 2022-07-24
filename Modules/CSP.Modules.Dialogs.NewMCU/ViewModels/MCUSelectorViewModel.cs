﻿using CSP.Database;
using CSP.Database.Models.MCU;
using CSP.Models.Interfaces;
using CSP.Services;
using CSP.Services.Models;
using CSP.Utils;
using CSP.Utils.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows;

namespace CSP.Modules.Dialogs.NewMCU.ViewModels
{
    public class MCUSelectorViewModel : BindableBase, IDialogWindowParameters
    {
        #region IDialogWindowParameters

        private WindowState _windowState = WindowState.Maximized;

        public event Action<IDialogResult> RequestClose;

        public double Height { get => 300; }
        public double MinHeight { get => 600; }
        public double MinWidth { get => 900; }
        public bool ShowInTaskbar { get => false; }
        public SizeToContent SizeToContent { get => SizeToContent.Manual; }
        public string Title { get => "创建项目"; }
        public double Width { get => 900; }

        public WindowState WindowState
        {
            get => _windowState;
            set => SetProperty(ref _windowState, value);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        #endregion IDialogWindowParameters

        private MCUModel _mcu;
        private RepositoryModel _repository;
        private RepositoryModel.CompanyModel.SeriesModel.LineModel.MCUModel _selectedMCU;

        public MCUSelectorViewModel()
        {
            _repository = MCUHelper.Repository;
        }

        public MCUModel MCU
        {
            get => _mcu;
            set => SetProperty(ref _mcu, value);
        }

        public DelegateCommand OnNew
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (MCU == null)
                    {
                        MessageBoxUtil.Error("未选择MCU或无MCU支持包");
                    }
                    else
                    {
                        if (SelectedMCU.Name.IsNullOrEmpty() ||
                            SelectedMCU.Company.IsNullOrEmpty())
                        {
                            MessageBoxUtil.Error("数据库发生错误");
                        }
                        else
                        {
                            var project = new ProjectModel
                            {
                                Header = "### csp, (C) csp",
                                Version = "0.0.0.0",
                                Target = new ProjectModel.TargetModel
                                {
                                    Type = "MCU",
                                    MCU = new ProjectModel.TargetModel.MCUModel
                                    {
                                        Company = SelectedMCU.Company,
                                        Name = SelectedMCU.Name
                                    }
                                }
                            };

                            ProjectHelper.Project = project;

                            var result = new DialogResult(ButtonResult.OK);
                            RequestClose?.Invoke(result);
                        }
                    }
                });
            }
        }

        public RepositoryModel Repository
        {
            get => _repository;
            set => SetProperty(ref _repository, value);
        }

        public RepositoryModel.CompanyModel.SeriesModel.LineModel.MCUModel SelectedMCU
        {
            get => _selectedMCU;

            set
            {
                if (!SetProperty(ref _selectedMCU, value))
                    return;

                if (value == null)
                {
                    MCU = null;
                    return;
                }

                MCUHelper.LoadMcu(value.Company.ToLower(), value.Name.ToLower());

                MCU = MCUHelper.MCU;
            }
        }
    }
}