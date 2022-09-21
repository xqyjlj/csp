﻿using CSP.Modules.Pages.MCU.Tools;
using CSP.Utils;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace CSP.Modules.Pages.MCU.ViewModels
{
    public class ClockTreeViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private Canvas _canvasControl;
        private double _canvasHeight = 900;
        private double _canvasWidth = 1600;
        private Uri _clockTreeImage;

        public ClockTreeViewModel(IRegionManager regionManager, IEventAggregator eventAggregator) {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            var path =
                $"{DescriptionHelper.RepositoryPath}/description/{DescriptionHelper.Name.ToLower()}/clock/{DescriptionHelper.MCU.Name}.svg";
            DebugUtil.Assert(File.Exists(path), new FileNotFoundException(nameof(path)), $"{path}: 不存在");
            if (File.Exists(path))
                ClockTreeImage = new Uri(path, UriKind.Relative);

            CanvasHeight = DescriptionHelper.Clock.Height;
            CanvasWidth = DescriptionHelper.Clock.Width;
        }

        public Canvas CanvasControl {
            get => _canvasControl;
            private set {
                if (SetProperty(ref _canvasControl, value)) {
                    foreach (var control in DescriptionHelper.Clock.ControlMap) {
                        UIElement obj = null;
                        switch (control.Value.Type) {
                            case "TextBox": {
                                    TextBox box = new() {
                                        Width = control.Value.Width,
                                        Height = control.Value.Height,
                                        Text = control.Value.ID.ToString(),
                                        TextAlignment = TextAlignment.Center,
                                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")!)
                                    };
                                    obj = box;
                                    break;
                                }
                            case "ComboBox": {
                                    ComboBox box = new() {
                                        Width = control.Value.Width,
                                        Height = control.Value.Height,
                                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")!)
                                    };
                                    obj = box;
                                    break;
                                }
                            case "Label": {
                                    TextBox box = new() {
                                        Width = control.Value.Width,
                                        Height = control.Value.Height,
                                        Text = control.Value.ID.ToString(),
                                        TextAlignment = TextAlignment.Center,
                                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")!),
                                        IsReadOnly = true
                                    };
                                    obj = box;
                                    break;
                                }
                            case "RadioButton": {
                                    Viewbox box = new() {
                                        Width = control.Value.Width,
                                        Height = control.Value.Height
                                    };
                                    RadioButton button = new() {
                                        GroupName = control.Value.GroupName,
                                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")!),
                                    };
                                    box.Child = button;
                                    obj = box;
                                    break;
                                }
                            default: {
                                    Log.Warning($"不存在的CanvasControl: {control.Value.Type}");
                                    break;
                                }
                        }
                        if (obj != null) {
#if DEBUG
                            var binding = new Binding("Name") {
                                Mode = BindingMode.TwoWay,
                                Source = control.Value,
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                            };
                            BindingOperations.SetBinding(obj, FrameworkElement.ToolTipProperty, binding);
#endif
                            Canvas.SetLeft(obj, control.Value.X);
                            Canvas.SetTop(obj, control.Value.Y);
                            CanvasControl.Children.Add(obj);
                        }
                    }
                }
            }
        }

        public double CanvasHeight {
            get => _canvasHeight;
            set => SetProperty(ref _canvasHeight, value);
        }

        public double CanvasWidth {
            get => _canvasWidth;
            set => SetProperty(ref _canvasWidth, value);
        }

        public Uri ClockTreeImage {
            get => _clockTreeImage;
            set => SetProperty(ref _clockTreeImage, value);
        }

        public DelegateCommand<object> OnLoaded {
            get {
                return new DelegateCommand<object>((obj) => {
                    if (obj is not Canvas canvas)
                        return;

                    CanvasControl = canvas;
                });
            }
        }
    }
}