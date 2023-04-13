// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Assets;
using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.ViewModels.Scheduling;
using CMiX.Core.Presentations.Scheduling;

namespace CMiX.Core.Presentations.Components
{
    public class ProjectModel : IModel
    {
        public ProjectModel()
        {
            ID = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00");
            ComponentModels = new ObservableCollection<IComponentModel>();
            AssetModels = new ObservableCollection<IAssetModel>();
            AssetModelsFlatten = new ObservableCollection<IAssetModel>();
            //CompositionSchedulerSelectorModel = new ComboBoxModel<CompositionScheduler>();
            SchedulerManagerModel = new SchedulerManagerModel();
        }



        public Guid ID { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }


        public SchedulerManagerModel SchedulerManagerModel { get; set; }
        public GenericValueModel<CompositionScheduler> CompositionSchedulerSelectorModel { get; set; }
        public ObservableCollection<IComponentModel> ComponentModels { get; set; }
        public ObservableCollection<IAssetModel> AssetModels { get; set; }
        public ObservableCollection<IAssetModel> AssetModelsFlatten { get; set; }
    }
}
