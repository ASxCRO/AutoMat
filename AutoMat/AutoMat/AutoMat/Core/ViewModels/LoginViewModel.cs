using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AutoMat.Core
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
