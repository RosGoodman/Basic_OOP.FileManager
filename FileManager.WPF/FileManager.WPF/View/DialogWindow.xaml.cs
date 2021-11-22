using FileManager.WPF.ViewModel;
using System.Windows;

namespace FileManager.WPF.View
{
    /// <summary>
    /// Логика взаимодействия для DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        private FileManagerViewModel _viewModel;
        public DialogWindow(FileManagerViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        /// <summary> Обработка нажатия кнопки подтверждения. </summary>
        /// <param name="sender"> Источник. </param>
        /// <param name="e"> Аргумент. </param>
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RenameFile_Command.Execute(null);
            this.Close();
        }

        /// <summary> Обработка события нажатия кнопки отмены. </summary>
        /// <param name="sender"> Источник. </param>
        /// <param name="e"> Аргумент. </param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult= false;
        }
    }
}
