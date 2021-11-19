﻿
using FileManager.WPF.ViewModel;
using NLog;
using System.Windows;
using FileManager.WPF.ViewModel;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dirList.ItemsSource = _viewModel.Directoryes;
        }
    }
}
