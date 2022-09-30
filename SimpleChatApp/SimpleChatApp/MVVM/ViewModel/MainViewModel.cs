using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient.MVVM.ViewModel
{
    public class MainViewModel
    {
        private Server server;

        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            server = new Server();
            server.connectedEvent += UserConnected;
            server.msgReceivedEvent += MessageReceived;
            server.userDisconnectEvent += RemoveUser;

            ConnectToServerCommand = new RelayCommand(o => server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));

            SendMessageCommnad = new RelayCommand(o => server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));
        }

        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommnad { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }        
        
        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = server.PacketReader.ReadMessage(),
                UID = server.PacketReader.ReadMessage(),
            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }

        private void MessageReceived()
        {
            var msg = server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

        private void RemoveUser()
        {
            var uid = server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }
    }
}
