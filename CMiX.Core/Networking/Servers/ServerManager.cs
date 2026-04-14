// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Networking.Servers
{
    public partial class ServerManager : ObservableObject, IControl
    {
        public ServerManager(ManagerData managerData,
                             ControlRepository controlRepository,
                             ControlFactory controlFactory,
                             ControlMessenger controlMessenger)
        {
            ControlMessenger = controlMessenger;
            ControlFactory = controlFactory;
            ManagerData = managerData;
            ControlRepository = controlRepository;

            IP = "127.0.0.1";
            Port = 8080;

            AddServerCommand = new RelayCommand<Window>(AddServer);
            AddItemCommand = new RelayCommand<Type>(AddItem);
            DeleteItemCommand = new RelayCommand<IControl>(DeleteItem);
            ReplaceSelectedItemCommand = new RelayCommand<IControl>(ReplaceItem);
            ResyncProjectCommand = new RelayCommand<IControl>(ResyncProject);
        }

        public Guid ID { get; set; }
        public ICommand AddServerCommand { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand ReplaceSelectedItemCommand { get; set; }
        public ICommand ResyncProjectCommand { get; set; }

        public ControlMessenger ControlMessenger { get; set; }
        public ControlFactory ControlFactory { get; set; }
        public ControlRepository ControlRepository { get; set; }
        public ManagerData ManagerData { get; set; }
        public ManagerReorderService? ManagerReorderService { get; } = null;


        private IControl _selectedItem;
        public IControl SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private int _port;
        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        private string _iP;
        public string IP
        {
            get => _iP;
            set => SetProperty(ref _iP, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private void ResyncProject(IControl control)
        {
            //ControlMessenger.Sen
        }

        private void AddServer(Window window)
        {
            if (ValidateIPv4(IP) && ValidatePort(IP, Port))
            {

                var server = (Server)ControlFactory.Create(typeof(Server));
                server.IP.Value = IP;
                server.Port.Value = Port;

                ErrorMessage = "Settings applied succefully !";
                ControlRepository.AddControl(server);

                var items = ManagerData.Items;

                if (SelectedItem is EmptyPrefab emptyPrefab)
                {
                    items[items.IndexOf(emptyPrefab)] = server;
                }
                else
                {
                    items.Add(server);
                }

                SelectedItem = server;
                ManagerData.SelectedIndex = items.IndexOf(server);

                window.Close();
            }
        }

        private void AddItem(Type type)
        {
            var prefab = ControlFactory.Create(type);
            ControlRepository.AddControl(prefab);

            var items = ManagerData.Items;

            if (SelectedItem is EmptyPrefab emptyPrefab && prefab is IPrefabModel pre)
            {
                items[items.IndexOf(emptyPrefab)] = prefab;
            }
            else
            {
                items.Add(prefab);
            }

            SelectedItem = prefab;
            ManagerData.SelectedIndex = items.IndexOf(prefab);
        }

        public void DeleteItem(IControl control)
        {
            var index = ManagerData.Items.IndexOf(control);

            if (control == null)
                return;

            ManagerData.Items.RemoveAt(index);

            if (ManagerData.Items.Count == 0)
            {
                SelectedItem = null;
                ManagerData.SelectedIndex = -1;
                return;
            }

            if (index == 0)
            {
                SelectedItem = ManagerData.Items[0];
                ManagerData.SelectedIndex = 0;
                return;
            }

            if (index > 0)
            {
                SelectedItem = ManagerData.Items[index - 1];
                ManagerData.SelectedIndex = index - 1;
                return;
            }
        }

        public void ReplaceItem(IControl control)
        {
            var prefab = (IPrefab)control;
            var items = ManagerData.Items;
            var index = -1;

            if (ManagerData.Items.Count == 0)
            {
                index = 0;
                items.Add(prefab);
            }
            else
            {
                index = ManagerData.SelectedIndex;
                items[index] = prefab;
            }

            SelectedItem = prefab;
            ManagerData.SelectedIndex = index;
        }


        //public void Apply()
        //{
        //    if (ValidateIPv4(IP.Value) && ValidatePort(IP.Value, Port.Value))
        //    {
        //        ErrorMessage.Value = "Settings applied succefully !";
        //    }
        //}

        public bool ValidatePort(string host, int port)
        {
            var ipa = Dns.GetHostAddresses(host)[0];
            try
            {
                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(ipa, port);
                if (sock.Connected == true)  // Port is in use and connection is successful
                {
                    ErrorMessage = "Port already in use";
                    return false;
                }
                sock.Close();

            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10061)  // Port is unused and could not establish connection 
                {
                    ErrorMessage = string.Empty;
                    return true;
                }
                else
                    ErrorMessage = ex.Message;
            }
            if (port == 0)
                return false;

            return false;
        }

        public bool ValidateIPv4(string ipString)
        {
            ErrorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(ipString))
            {
                ErrorMessage = "IP Address is not valid";
                return false;
            }

            var splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                ErrorMessage = "IP Address is not valid";
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        public IControlModel ToModel() => new ServerManagerModel
        {
            ID = ID,
            ManagerData = (ManagerDataModel)ManagerData.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ServerManagerModel)model;
            ID = m.ID;
        }
    }
}
