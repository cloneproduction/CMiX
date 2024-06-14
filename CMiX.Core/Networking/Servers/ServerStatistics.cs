// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;
using WatsonTcp;

namespace CMiX.Core.Networking.Messenger
{
    public class ServerStatistics : ObservableObject, IControl
    {
        public ServerStatistics()
        {
            SentMessages = 0;
        }

        public Guid ID { get; set; }

        private long _sentMessages;
        public long SentMessages
        {
            get => _sentMessages;
            set => SetProperty(ref _sentMessages, value);
        }

        private long _sentBytes;
        public long SentBytes
        {
            get => _sentBytes;
            set => SetProperty(ref _sentBytes, value);
        }

        private DateTime _startTime;
        public DateTime StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        private TimeSpan _upTime;
        public TimeSpan UpTime
        {
            get => _upTime;
            set => SetProperty(ref _upTime, value);
        }

        private decimal _sentMessagesAverageSize;
        public decimal SentMessagesAverageSize
        {
            get => _sentMessagesAverageSize;
            set => SetProperty(ref _sentMessagesAverageSize, value);
        }


        public void Update(WatsonTcpServer watsonTcpServer)
        {
            SentMessages = watsonTcpServer.Statistics.SentMessages;
            SentMessagesAverageSize = watsonTcpServer.Statistics.SentMessageSizeAverage;
            StartTime = watsonTcpServer.Statistics.StartTime;
            UpTime = watsonTcpServer.Statistics.UpTime;
        }
    }
}
