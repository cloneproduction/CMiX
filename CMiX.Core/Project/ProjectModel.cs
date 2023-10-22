// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Assets;
using CMiX.Core.BaseControls;
using CMiX.Core.Scheduling;
using CMiX.Core.ViewModels.Scheduling;

namespace CMiX.Core.Compositing
{
    public class ProjectModel : IControlModel
    {
        public ProjectModel()
        {
            ID = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00");
            AssetModels = new ObservableCollection<IAssetModel>();
            AssetModelsFlatten = new ObservableCollection<IAssetModel>();
            SchedulerManagerModel = new SchedulerManagerModel();
        }



        public Guid ID { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }


        public SchedulerManagerModel SchedulerManagerModel { get; set; }
        public GenericValueModel<CompositionScheduler> CompositionSchedulerSelectorModel { get; set; }
        public ObservableCollection<IAssetModel> AssetModels { get; set; }
        public ObservableCollection<IAssetModel> AssetModelsFlatten { get; set; }
    }
}
