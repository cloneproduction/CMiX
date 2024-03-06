// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Networking.Messages
{
    internal class MessageOpenProject : IMessage
    {
        public MessageOpenProject()
        {
            
        }
        public MessageOpenProject(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; set; }
        public Guid ID { get; set; }
    }
}
