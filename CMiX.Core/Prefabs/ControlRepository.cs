// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs
{
    public class ControlRepository :ObservableObject
    {
        public ControlRepository()
        {
            Controls = new ObservableCollection<IControl>();
            Textures = CollectionViewSource.GetDefaultView(Controls);
            Textures.Filter = new Predicate<object>(this.FilterTexture);

            BindingOperations.EnableCollectionSynchronization(Controls, this);
        }



        private int nameCount = 0;
        public ObservableCollection<IControl> Controls { get; set; }


        public bool FilterTexture(object item)
        {
            if(item is Texture)
                return true;

            return false;
        }

        public void AddControl(IControl control)
        {
            Controls.Add(control);
            //control.Name.Value = control.GetType().Name + nameCount.ToString();
            nameCount++;
            Console.WriteLine("PrefabRepository Count is " + Controls.Count().ToString());
        }

        private ICollectionView _textures;
        public ICollectionView Textures
        {
            get => _textures;
            set => SetProperty(ref _textures, value);
        }

        public IControl GetPrefab(Guid id)
        {
            return Controls.FirstOrDefault(x => x.ID == id);
        }
    }
}
