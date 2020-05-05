﻿using CMiX.MVVM.ViewModels;
using MvvmDialogs;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;

namespace CMiX.Studio.ViewModels.MessageService
{
    public class Settings : ViewModel, IModalDialogViewModel
    {
        public Settings()
        {

            OkCommand = new RelayCommand(p => Ok(p as Window));
            CancelCommand = new RelayCommand(p => Cancel(p as Window));
            ApplyCommand = new RelayCommand(p => Apply(), x => CanApplySettings());
            CloseWindowCommand = new RelayCommand(p => Cancel(p as Window));
        }

        private bool CanApplySettings()
        {
            if (CanApply)
                return true;
            else
                return false;
        }

        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ApplyCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }

        public bool? DialogResult { get; set; }

        private bool _canApply;
        public bool CanApply
        {
            get => _canApply;
            set => SetAndNotify(ref _canApply, value);
        }

        private string _topic;
        public string Topic
        {
            get => _topic;
            set
            {
                SetAndNotify(ref _topic, value);
                CommandManager.InvalidateRequerySuggested();
                CanApply = true;
            }
        }

        private string _ip;
        public string IP
        {
            get => _ip;
            set
            {
                SetAndNotify(ref _ip, value);
                CanApply = true;
            }
        }

        private int _port;
        public int Port
        {
            get => _port;
            set
            {
                SetAndNotify(ref _port, value);
                CanApply = true;
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetAndNotify(ref _errorMessage, value);
        }

        public void Ok(Window window)
        {
            window.Close();
        }

        public void Cancel(Window window)
        {
            DialogResult = false;
            window.Close();
        }

        public void Apply()
        {
            CanApply = true;
            if (ValidateIPv4(IP) && ValidatePort(IP, Port))
            {
                ErrorMessage = "Settings applied succefully !";
                CanApply = false;
                DialogResult = true;
            }
        }


        public bool ValidatePort(string host, int port)
        {
            IPAddress ipa = Dns.GetHostAddresses(host)[0];
            try
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
                    ErrorMessage = String.Empty;
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
            ErrorMessage = String.Empty;
            if (String.IsNullOrWhiteSpace(ipString))
            {
                ErrorMessage = "IP Address is not valid";
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                ErrorMessage = "IP Address is not valid";
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }
    }
}