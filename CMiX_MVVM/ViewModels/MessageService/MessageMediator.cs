﻿using System.Collections.Generic;
using System;
using CMiX.MVVM.ViewModels.MessageService;
using CMiX.MVVM.Services;

namespace CMiX.MVVM.ViewModels.Mediator
{
    public class MessageMediator : IDisposable
    {
        public MessageMediator(MessengerTerminal messageTerminal)
        {
            MessengerTerminal = messageTerminal;
            MessengerTerminal.MessageReceived += MessengerTerminal_MessageReceived;
            Colleagues = new Dictionary<string, IMessageProcessor>();
        }

        private void MessengerTerminal_MessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            this.Notify(MessageDirection.IN, message);
        }

        private MessengerTerminal MessengerTerminal { get; set; }
        private Dictionary<string, IMessageProcessor>  Colleagues { get; set; }

        public void Notify(MessageDirection messageDirection, Message message)
        {
            if(messageDirection == MessageDirection.IN)
            {
                IMessageProcessor col;
                if (Colleagues.TryGetValue(message.Address, out col))
                {
                    col.Receive(message);
                }
            }
            else if(messageDirection == MessageDirection.OUT)
            {
                MessengerTerminal.SendMessage(message.Address, message);
            }
        }

        public void RegisterColleague(IMessageProcessor colleague)
        {
            
            if (Colleagues.ContainsKey(colleague.GetAddress()))
                Colleagues[colleague.GetAddress()] = colleague;
            else
                Colleagues.Add(colleague.GetAddress(), colleague);
        }

        public void UnregisterColleague(string address)
        {
            Colleagues.Remove(address);
        }

        public void Dispose()
        {
            MessengerTerminal.MessageReceived -= MessengerTerminal_MessageReceived;
        }
    }
}