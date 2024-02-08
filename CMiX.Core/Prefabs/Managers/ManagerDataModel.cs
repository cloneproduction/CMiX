// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;

namespace CMiX.Core.Prefabs.Managers
{
    public class ManagerDataModel : IControlModel
    {
        public ManagerDataModel()
        {
            ID = Guid.NewGuid();
            //Items = new ObservableCollection<IControlModel>();
        }

        public Guid ID { get; set; }
        //public ObservableCollection<IControlModel> Items { get; set; }
        //public IControlModel SelectedItem { get; set; }
        //public int SelectedIndex { get; set; }
    }
}
