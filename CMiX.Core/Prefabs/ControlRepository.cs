// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;

namespace CMiX.Core.Prefabs
{
    public class ControlRepository 
    {
        public ControlRepository()
        {
            Controls = new ObservableCollection<IControl>();
        }

        private int nameCount = 0;
        public ObservableCollection<IControl> Controls { get; set; }

        public void AddControl(IControl control)
        {
            Controls.Add(control);
            //control.Name.Value = control.GetType().Name + nameCount.ToString();
            nameCount++;
            Console.WriteLine("PrefabRepository Count is " + Controls.Count().ToString());
        }

        //public void RemovePrefab(IControl prefab)
        //{
        //    Controls.Remove(prefab);
        //}

        //public IControl GetPrefab(Guid id)
        //{
        //    return Controls.FirstOrDefault(x => x.ID == id);
        //}
    }
}
