// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Net;
using System.Net.Sockets;
using System.Windows.Input;
using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Networking.Servers
{
    public class ServerSettings : ObservableObject, IControl
    {
        public ServerSettings(GenericValue<string> ip,
                              GenericValue<int> port)
        {
            IP = ip;
            Port = port;
            ApplyCommand = new RelayCommand(Apply);
        }

        public ICommand ApplyCommand { get; set; }

        public Guid ID { get; set; }
        public GenericValue<string> IP { get; set; }
        public GenericValue<int> Port { get; set; }
        public GenericValue<string> ErrorMessage { get; set; }

        public void Apply()
        {
            if (ValidateIPv4(IP.Value) && ValidatePort(IP.Value, Port.Value))
            {
                ErrorMessage.Value = "Settings applied succefully !";
            }
        }

        public bool ValidatePort(string host, int port)
        {
            var ipa = Dns.GetHostAddresses(host)[0];
            try
            {
                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(ipa, port);
                if (sock.Connected == true)  // Port is in use and connection is successful
                {
                    ErrorMessage.Value = "Port already in use";
                    return false;
                }
                sock.Close();

            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10061)  // Port is unused and could not establish connection 
                {
                    ErrorMessage.Value = string.Empty;
                    return true;
                }
                else
                    ErrorMessage.Value = ex.Message;
            }
            if (port == 0)
                return false;

            return false;
        }

        public bool ValidateIPv4(string ipString)
        {
            ErrorMessage.Value = string.Empty;
            if (string.IsNullOrWhiteSpace(ipString))
            {
                ErrorMessage.Value = "IP Address is not valid";
                return false;
            }

            var splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                ErrorMessage.Value = "IP Address is not valid";
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }
    }
}
