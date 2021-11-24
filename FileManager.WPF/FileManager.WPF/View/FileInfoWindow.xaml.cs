using FileManager.WPF.ViewModel;
using System.Windows;

namespace FileManager.WPF.View
{
    /// <summary>
    /// Логика взаимодействия для FileInfoWindow.xaml
    /// </summary>
    public partial class FileInfoWindow : Window
    {
        public FileInfoWindow(FileManagerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
