﻿
using FileManager.WPF.View;
using FileManager.WPF.ViewModel;
using NLog;
using System.Windows;

namespace FileManager.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        FileManagerViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new FileManagerViewModel(_logger);
            DataContext = _viewModel;
        }

        private void OpenFile_ButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ListBoxItemEnterCommand.Execute(null);
        }

        private void OpenDialogWindowForRename_ButtonClick(object sender, RoutedEventArgs e)
        {
            DialogWindow dialogWindow = new DialogWindow(_viewModel);
            dialogWindow.DataContext = _viewModel;
            dialogWindow.ShowDialog();
        }

        private void CutFile_ButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ListBoxItem_Ctrl_X_Command.Execute(null);
        }

        private void CopyFile_ButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ListBoxItem_Ctrl_C_Command.Execute(null);
        }

        private void PastFile_ButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ListBoxItem_Ctrl_V_Command.Execute(null);
        }

        private void GetFileInfo_ButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.GetFileInfoCommand.Execute(null);
            FileInfoWindow infoWindow = new FileInfoWindow(_viewModel);
            infoWindow.DataContext = _viewModel;
            infoWindow.ShowDialog();
        }
        
    }
}
