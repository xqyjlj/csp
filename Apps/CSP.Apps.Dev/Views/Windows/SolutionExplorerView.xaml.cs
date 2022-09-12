﻿using CSP.Events;
using Syncfusion.UI.Xaml.TreeView;

namespace CSP.Apps.Dev.Views.Windows
{
    public partial class SolutionExplorerView
    {
        public SolutionExplorerView() {
            InitializeComponent();
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e) {
            var item = TreeView.SelectedItem;

            if (item is not SolutionExplorerEvent.Model value)
                return;

            value.CallBack?.Invoke(value.Name);
        }
    }
}