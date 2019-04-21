﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SharpOSC;
using CMiX.ViewModels;

namespace CMiX.Services
{
    public class OSCMessenger : ViewModel, IMessenger
    {
        public OSCMessenger()
        {
            Sender = new UDPSender(Address, Port);
            messages = new List<OscMessage>();
        }

        public UDPSender Sender { get; set; }

        private readonly List<OscMessage> messages;

        private bool _sendenabled = true;
        public bool SendEnabled
        {
            get { return _sendenabled; }
            set
            {
                _sendenabled = value;
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set => SetAndNotify(ref _name, value);
        }


        private string _address = "127.0.0.1";
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                UpdateUDPSender();
            }
        }

        private int _port = 55555;
        public int Port
        {
            get { return _port; }
            set
            {
                _port = value;
                UpdateUDPSender();
            }
        }

        private void UpdateUDPSender()
        {
            Sender = new UDPSender(Address, Port);
        }

        private OscMessage CreateOscMessage(string address, params object[] args) => new OscMessage(address, args.Select(a => a?.ToString()).ToArray());

        public void QueueMessage(string address, params object[] args)
        {
            var message = CreateOscMessage(address, args);
            messages.Add(message);
        }

        public void SendMessage(string address, params object[] args)
        {
            if (SendEnabled)
            {
                var message = CreateOscMessage(address, args);
                Sender.Send(message);
            }
        }

        public void SendQueue()
        {
            if (SendEnabled)
            {
                var bundle = new OscBundle(0, messages.ToArray());
                Sender.Send(bundle);
                messages.Clear();
            }
        }

        public void QueueObject(object obj)
        {
            string address = string.Empty;
            string propdata = string.Empty;
            string propertyname = string.Empty;

            if (obj == null) return;

            if (obj.GetType().GetProperty("MessageAddress") != null)
            {
                address = obj.GetType().GetProperty("MessageAddress").GetValue(obj, null).ToString();
            }

            Type objType = obj.GetType();
            var properties = objType.GetProperties().Where(p => !p.GetIndexParameters().Any());

            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj, null);

                object[] attrs = property.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    OSCAttribute oscdata = attr as OSCAttribute;
                    if (oscdata != null)
                    {
                        propertyname = property.Name;

                        if (propValue is ObservableCollection<FileNameItem>)
                        {
                            List<string> filenames = new List<string>();
                            foreach (FileNameItem lbfn in (ObservableCollection<FileNameItem>)propValue)
                            {
                                if (lbfn.FileIsSelected == true)
                                {
                                    filenames.Add(lbfn.FileName);
                                }
                            }
                            QueueMessage(address + propertyname, filenames.ToArray());
                        }
                        else
                        {
                            propdata = property.GetValue(obj, null).ToString();
                            QueueMessage(address + propertyname, propdata);
                        }
                    }
                }

                var elems = propValue as IList;
                if ((elems != null) && !(elems is string[]))
                {
                    foreach (var item in elems)
                    {
                        QueueObject(item);
                    }
                }
                else
                {
                    // This will not cut-off System.Collections because of the first check
                    if (property.PropertyType.Assembly == objType.Assembly)
                    {
                        QueueObject(propValue);
                    }
                    /*else
                    {
                        if (propValue is string[])
                        {
                            var str = new StringBuilder();
                            foreach (string item in (string[])propValue)
                            {
                                str.AppendFormat("{0}; ", item);
                            }
                            propValue = str.ToString();
                            str.Clear();
                        }
                    }*/
                }
            }
        }
    }
}