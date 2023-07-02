// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Prefab.Managers;
using CMiX.Core.Transformation;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Compositing
{
    public class EntityModel : IPrefabModel
    {
        public EntityModel()
        {
            ID = Guid.NewGuid();

            TransformSRT = new TransformSRTModel();
            ModifierManager = new ModifierManagerModel();
            Mesh = new MeshModel();
            MaterialSelector = new PrefabSelectorModel();
            Name = new StringValueModel("Entity " + ID.ToString());
            IsSelected = new BooleanValueModel(false);
            IsRenaming = new BooleanValueModel(false);
            Visibility = new BooleanValueModel(false);
        }

        public EntityModel(Guid id) : this()
        {
            ID = id;
        }

        public Guid ID { get; set; }
        public StringValueModel Name { get; set; }
        public TransformSRTModel TransformSRT { get; set; }
        public MeshModel Mesh { get; set; }
        public ModifierManagerModel ModifierManager { get; set; }
        public BooleanValueModel IsSelected { get; set; }
        public BooleanValueModel IsRenaming { get; set; }
        public PrefabSelectorModel MaterialSelector { get; set; }
        public BooleanValueModel Visibility { get; internal set; }
    }
}
