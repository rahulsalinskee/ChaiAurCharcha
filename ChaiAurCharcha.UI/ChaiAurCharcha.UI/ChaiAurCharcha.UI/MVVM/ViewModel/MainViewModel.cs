using ChaiAurCharcha.UI.MVVM.Core;
using ChaiAurCharcha.UI.MVVM.Model;
using ChaiAurCharcha.UI.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace ChaiAurCharcha.UI.MVVM.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Server
        private Server? _server;

        public Server? Server
        {
            get
            {
                return _server;
            }
            set
            {
                this._server = value;
                OnPropertyChanged(nameof(Server));
            }
        }
        #endregion

        #region Observable Collections
        public ObservableCollection<UserModel> Users { get; set; }

        public ObservableCollection<string> Messages { get; set; }
        #endregion

        #region Message
        private string _message = string.Empty;

        public string Message
        {
            get
            {
                return this._message;
            }
            set
            {
                this._message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
        #endregion

        #region Relay Commands
        public RelayCommand ConnectToServerCommand { get; set; }

        public RelayCommand SendMessageCommand { get; set; }
        #endregion

        #region UserName
        private string _userName = string.Empty;

        public string UserName
        {
            get
            {
                return this._userName;
            }
            set
            {
                this._userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            _server = new();
            Users = []; /* Can also be initialized as - new ObservableCollection<UserModel>() OR new() */
            Messages = []; /* Can also be initialized as - new ObservableCollection<string>() OR new() */
            SubscribeUserConnectionEvent();
            SubscribeMessageEvent();
            SubscriveUserDisconnectEvent();
            ConnectToServerCommand = new RelayCommand(
                execute: server => _server?.ConnectToServer(userName: UserName),
                canExecute: server => !string.IsNullOrEmpty(UserName));
            SendMessageCommand = new RelayCommand(
                execute: server => SendMessageAsync(message: Message).GetAwaiter().GetResult(),
                canExecute: server => !string.IsNullOrEmpty(Message));
        }
        #endregion

        private async Task SendMessageAsync(string message)
        {
            if (!string.IsNullOrWhiteSpace(message) && _server != null)
            {
                await _server.SendMessageToServerAsync(message: message);
                Message = string.Empty;
            }
        }

        #region I Notify PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            /* This event is published or raised when a property is changed */
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Connected Event
        private void SubscribeUserConnectionEvent()
        {
            UnSubscribeUserConnectionEvent();
            if (_server != null)
            {
                _server.connectedEvent += UserConnected;
            }
        }

        private void UnSubscribeUserConnectionEvent()
        {
            if (_server != null)
            {
                _server.connectedEvent -= UserConnected;
            }
        }

        private void UserConnected()
        {
            if (_server?.PackageReader == null)
                return;

            /* This is User */
            string userName = _server.PackageReader.ReadMessage() ?? string.Empty;
            string uid = _server.PackageReader.ReadMessage() ?? string.Empty;

            UserModel userModel = new()
            {
                UserName = userName,
                UID = uid,
            };

            /* To append user to list of Users */
            if (!Users.Any(user => user.UID == userModel.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(userModel));
            }
        }
        #endregion

        #region Message Event
        private void SubscribeMessageEvent()
        {
            UnSubscribeMessageEvent();
            if (_server != null)
            {
                _server.messageReceivedEvent += MessageReceived;
            }
        }

        private void UnSubscribeMessageEvent()
        {
            if (_server != null)
            {
                _server.messageReceivedEvent -= MessageReceived;
            }
        }

        private void MessageReceived()
        {
            if (_server?.PackageReader == null)
                return;

            var message = _server.PackageReader.ReadMessage() ?? string.Empty;

            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));
        }
        #endregion

        #region User Disconnect Event
        private void SubscriveUserDisconnectEvent()
        {
            UnsubscribeUserDisconnectEvent();
            if (_server != null)
            {
                _server.userDisconnectedEvent += UserRemoved;
            }
        }

        private void UnsubscribeUserDisconnectEvent()
        {
            if (_server != null)
            {
                _server.userDisconnectedEvent -= UserRemoved;
            }
        }

        private void UserRemoved()
        {
            if (_server?.PackageReader == null)
            {
                return;
            }

            var uid = _server.PackageReader.ReadMessage() ?? string.Empty;
            var user = Users.FirstOrDefault(u => u.UID == uid);

            if (user != null)
            {
                Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
            }
        }
        #endregion
    }
}
