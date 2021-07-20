//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using System.Windows.Input;
//using Ceras;
//using CMiX.Core.Network.Messages;
//using CMiX.Core.Presentation.ViewModels.Components;

//namespace CMiX.Core.Presentation.ViewModels.Network
//{
//    public class Messenger : ObservableObject
//    {
//        public Messenger(int id)
//        {
//            Server = new Server(id);
//            Serializer = new CerasSerializer();
//            Name = $"Messenger ({id})";

//            StartServerCommand = new RelayCommand(p => StartServer());
//            RequestProjectReSyncCommand = new RelayCommand(p => RequestProjectResync(p as Project));
//            StopServerCommand = new RelayCommand(p => StopServer());
//            RestartServerCommand = new RelayCommand(p => RestartServer());
//            RenameCommand = new RelayCommand(p => Rename());
//        }

//        internal void SendMessage(Message message)
//        {
//            var data = Serializer.Serialize(message);
//            this.Server.Send(data);
//        }

//        public ICommand RenameCommand { get; }
//        public ICommand RequestProjectReSyncCommand { get; }
//        public ICommand StartServerCommand { get; }
//        public ICommand StopServerCommand { get; }
//        public ICommand RestartServerCommand { get; }


//        private CerasSerializer Serializer { get; set; }
//        public Server Server { get; set; }


//        private string _name;
//        public string Name
//        {
//            get => _name;
//            set => SetProperty(ref _name, value);
//        }

//        private bool _isRenaming;
//        public bool IsRenaming
//        {
//            get => _isRenaming;
//            set => SetProperty(ref _isRenaming, value);
//        }


//        public void Rename()
//        {
//            IsRenaming = true;
//            System.Console.WriteLine("DOUBLE CLICK !!");
//        }

//        //public void SendData(byte[] data)
//        //{
//        //    this.Server.Send(data);
//        //}


//        public Settings GetSettings()
//        {
//            return new Settings(Name, Server.Topic, Server.IP, Server.Port);
//        }


//        public void StartServer()
//        {
//            Server.Start();
//        }

//        public void StopServer()
//        {
//            Server.Stop();
//        }

//        public void RestartServer()
//        {
//            Server.Restart();
//        }


//        public void RequestProjectResync(Project project)
//        {
//            byte[] data = Serializer.Serialize(project.GetModel());

//            System.Console.WriteLine("Messenger send project to Instance");
//            Server.SendRequestProjectSync(data);
//        }


//        public void SetSettings(Settings settings)
//        {
//            Name = settings.Name;
//            Server.Topic = settings.Topic;
//            Server.IP = settings.IP;
//            Server.Port = settings.Port;
//            Server.Start();
//        }
//    }
//}
