
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
    }
}
