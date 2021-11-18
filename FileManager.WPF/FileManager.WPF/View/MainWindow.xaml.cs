
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
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new FileManagerViewModel(_logger);
        }
    }
}
