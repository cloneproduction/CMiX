// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Animations;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Compositing
{
    // Builds the ProjectModel shape shared by the normal save path and the crash emergency save,
    // so the two cannot drift when the project model grows a field.
    public static class ProjectModelBuilder
    {
        public static ProjectModel Build(Composition composition, MasterBeat masterBeat)
        {
            return new ProjectModel
            {
                MasterBeat = (MasterBeatModel)masterBeat.ToModel(),
                CompositionManager = new PrefabManagerModel
                {
                    ManagerData = new ManagerDataModel
                    {
                        Items = new Collection<IControlModel> { (CompositionModel)composition.ToModel() },
                        SelectedIndex = 0
                    }
                }
            };
        }
    }
}
