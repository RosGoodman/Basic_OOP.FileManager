using FileManager.WPF.ViewModel;
using System.Windows;

namespace FileManager.WPF.View
{
    /// <summary>
    /// Логика взаимодействия для FileInfoWindow.xaml
    /// </summary>
    public partial class FileInfoWindow : Window
    {
        private FileManagerViewModel _viewModel;
        public FileInfoWindow(FileManagerViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
