﻿using System;
using CMiX.MVVM.Services;
using CMiX.MVVM.ViewModels;

namespace CMiX.Studio.ViewModels.MessageService
{
    public class MessageReceiver : ViewModel
    {
        public MessageReceiver()
        {
            Client = new Client();
            Client.MessageReceived += Client_MessageReceived;
        }

        public event EventHandler<MessageEventArgs> MessageReceived;
        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            MessageReceived?.Invoke(sender, e);
        }

        private void Client_MessageReceived(object sender, DataEventArgs e)
        {
            Message message = MessageSerializer.Serializer.Deserialize<Message>(e.Data);
            OnMessageReceived(sender, new MessageEventArgs(message));
        }

        public string Address
        {
            get { return String.Format("tcp://{0}:{1}", IP, Port); }
        }

        private string _ip;
        public string IP
        {
            get => _ip;
            set => SetAndNotify(ref _ip, value);
        }

        private string _topic;
        public string Topic
        {
            get => _topic;
            set => SetAndNotify(ref _topic, value);
        }

        private int _port;
        public int Port
        {
            get => _port;
            set => SetAndNotify(ref _port, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetAndNotify(ref _name, value);
        }

        private Client _client;
        public Client Client
        {
            get => _client;
            set => SetAndNotify(ref _client, value);
        }

        public Settings GetSettings()
        {
            return new Settings(Name, Client.Topic, Client.IP, Client.Port);
        }

        public void SetSettings(Settings settings)
        {
            if (Client.IsRunning)
                Client.Stop();

            Name = settings.Name;
            Client.Topic = settings.Topic;
            Client.IP = settings.IP;
            Client.Port = settings.Port;
        }

        public void Start(Settings settings)
        {
            if (Client.IsRunning)
                return;

            Name = settings.Name;
            Client.Topic = settings.Topic;
            Client.IP = settings.IP;
            Client.Port = settings.Port;
            Client.Start();
        }

        public void Stop()
        {
            Client.Stop();
        }
    }
}