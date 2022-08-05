// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Entity : ObservableObject, IPrefab, IBeatable
    {
        public Entity(EntityModel entityModel)
        {
            ID = entityModel.ID;
            Name = this.GetType().Name + ID.ToString();
        }

        public Guid ID { get; set; }


        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }


        private Mesh _mesh;
        public Mesh Mesh
        {
            get => _mesh;
            set => SetProperty(ref _mesh, value);
        }

        private Material _material;
        public Material Material
        {
            get => _material;
            set => SetProperty(ref _material, value);
        }

        private Coloration _coloration;
        public Coloration Coloration
        {
            get => _coloration;
            set => SetProperty(ref _coloration, value);
        }

        private Transform _transform;
        public Transform Transform
        {
            get => _transform;
            set => SetProperty(ref _transform, value);
        }

        public MasterBeat MasterBeat { get; set; }

        public void SetViewModel(IModel model)
        {
            EntityModel entityModel = model as EntityModel;
            this.ID = entityModel.ID;
        }

        public IModel GetModel()
        {
            EntityModel entityModel = new EntityModel() ;
            entityModel.ID = ID;

            return entityModel;
        }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            this.MasterBeat = masterBeat;
        }
    }
}
