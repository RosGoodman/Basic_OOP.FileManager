
using FileManager.WPF.View;
using FileManager.WPF.ViewModel;
using NLog;
using System.Windows;
using System.Windows.Input;

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

        #region events

        /// <summary> Открыть файл. </summary>
        /// <param name="sender"> Источник. </param>
        /// <param name="e"> Аргумент. </param>
        private void OpenFile_ButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ListBoxItemEnterCommand.Execute(null);
        }

        /// <summary> Открыть диалоговое окно переименования файла. </summary>
        /// <param name="sender"> Источник. </param>
        /// <param name="e"> Аргумент. </param>
        private void OpenDialogWindowForRename_ButtonClick(object sender, RoutedEventArgs e)
        {
            DialogWindow dialogWindow = new DialogWindow(_viewModel);
            dialogWindow.DataContext = _viewModel;
            dialogWindow.ShowDialog();
        }

        /// <summary> Вырезать файл. </summary>
        /// <param name="sender"> Источник. </param>
        /// <param name="e"> Аргумент. </param>
        private void CutFile_ButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ListBoxItem_Ctrl_X_Command.Execute(null);
        }

        /// <summary> Копировать файл. </summary>
        //// <param name="sender"> Источник. </param>
        /// <param name="e"> Аргумент. </param>
        private void CopyFile_ButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ListBoxItem_Ctrl_C_Command.Execute(null);
        }

        /// <summary> Вставить файл. </summary>
        /// <param name="sender"> Источник. </param>
        /// <param name="e"> Аргумент. </param>
        private void PastFile_ButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ListBoxItem_Ctrl_V_Command.Execute(null);
        }

        /// <summary> Открыть окно с информацией о файле. </summary>
        //// <param name="sender"> Источник. </param>
        /// <param name="e"> Аргумент. </param>
        private void GetFileInfo_ButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.GetFileInfoCommand.Execute(null);
            FileInfoWindow infoWindow = new FileInfoWindow(_viewModel);
            infoWindow.DataContext = _viewModel;
            infoWindow.ShowDialog();
        }

        /// <summary> Отлавливание нажатия клавиши Enter в окне поиска. </summary>
        /// <param name="sender"> Источник. </param>
        /// <param name="e"> Аргумент. </param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) _viewModel.FindCommand.Execute(null);
        }

        /// <summary> Удалить  выбранный файл. </summary>
        /// <param name="sender"> Источник. </param>
        /// <param name="e"> Аргумент. </param>
        private void Delete_ButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteCommand.Execute(null);
        }

        #endregion
    }
}
