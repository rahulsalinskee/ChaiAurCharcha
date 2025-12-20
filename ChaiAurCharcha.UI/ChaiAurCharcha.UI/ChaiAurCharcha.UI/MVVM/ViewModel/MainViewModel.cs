using ChaiAurCharcha.UI.MVVM.Core;
using ChaiAurCharcha.UI.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChaiAurCharcha.UI.MVVM.ViewModel
{
    class MainViewModel
    {
        private Server? _server;

        public RelayCommand ConnectToServerCommand { get; set; }

        public MainViewModel()
        {
            _server = new();
            ConnectToServerCommand = new RelayCommand(server => _server.ConnectToServer());
        }
    }
}
