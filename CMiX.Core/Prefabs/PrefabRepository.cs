// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefabs
{
    public class PrefabRepository<T> : ObservableObject, IRecipient<MessageRequestPrefab> where T : IPrefab
    {
        public PrefabRepository(IPrefabDataBase prefabDataBase)
        {
            PrefabDataBase = prefabDataBase;

            Prefabs = new ObservableCollection<IPrefab>();
            WeakReferenceMessenger.Default.Register(this, MessageType.Internal);
        }


        private IPrefabDataBase PrefabDataBase { get; set; }


        private ObservableCollection<IPrefab> _prefabs;
        public ObservableCollection<IPrefab> Prefabs
        {
            get => _prefabs;
            set => SetProperty(ref _prefabs, value);
        }


        public void AddPrefab(T prefab)
        {
            PrefabDataBase.Prefabs.Add(prefab);
            Prefabs.Add(prefab);
        }

        public void RemovePrefab(T prefab)
        {
            PrefabDataBase.Prefabs.Remove(prefab);
            Prefabs.Remove(prefab);
        }

        public IPrefab GetPrefab(Guid id)
        {
            return PrefabDataBase.Prefabs.FirstOrDefault(x => x.ID == id);
        }

        public void Receive(MessageRequestPrefab message)
        {
            if (!message.HasReceivedResponse)
                message.Reply(Prefabs.FirstOrDefault(x => x.ID == message.ID));
        }
    }
}
