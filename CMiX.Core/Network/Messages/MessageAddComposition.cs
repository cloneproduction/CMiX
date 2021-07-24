// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Components;

namespace CMiX.Core.Network.Messages
{
    public class MessageAddComposition : IComponentMessage
    {
        public MessageAddComposition()
        {

        }

        public MessageAddComposition(IProject project)
        {
            Project = project;
        }

        public IProject Project { get; set; }

        public void Process<T>(T receiver)
        {
            System.Console.WriteLine("MessageAddComposition Process");
            ComponentManager componentManager = receiver as ComponentManager;
            if (componentManager != null)
            {
                System.Console.WriteLine("componentManager != null");
                componentManager.CreateComponent(Project);
            }
        }
    }
}
