﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FileManager.WPF.ViewModel
{
    /// <summary> Класс, описывающий базовую ViewModel с событием изменения свойств. </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
