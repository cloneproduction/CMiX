// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Presentation.ViewModels.Assets
{
    public class AssetPathSelector : ObservableObject, IControl, IDropTarget
    {
        public AssetPathSelector(Asset defaultAsset, AssetPathSelectorModel assetPathSelectorModel)
        {
            this.ID = assetPathSelectorModel.ID;
            SelectedAsset = defaultAsset;
        }


        public Guid ID { get; set; }
        public Communicator AssetPathSelectorCommunicator { get; set; }


        private Asset _selectedAsset;
        public Asset SelectedAsset
        {
            get => _selectedAsset;
            set
            {
                SetProperty(ref _selectedAsset, value);

                AssetPathSelectorCommunicator?.SendMessage(new MessageAsset(SelectedAsset));
                if (value != null)
                    System.Console.WriteLine("SelectedAsset Name is " + SelectedAsset.Name);
            }
        }


        public void SetCommunicator(Communicator communicator)
        {
            AssetPathSelectorCommunicator = new Communicator(this);
            AssetPathSelectorCommunicator.SetCommunicator(communicator);
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            AssetPathSelectorCommunicator.UnsetCommunicator(communicator);
        }


        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.DragInfo != null && dropInfo.DragInfo.SourceItem != null)
                dropInfo.Effects = DragDropEffects.Copy;
        }

        public void Drop(IDropInfo dropInfo)
        {
            //SelectedPath = ((IAssets)dropInfo.DragInfo.SourceItem).Path;
        }

        public void SetViewModel(IModel model)
        {
            AssetPathSelectorModel assetPathSelectorModel = model as AssetPathSelectorModel;
            assetPathSelectorModel.ID = this.ID;

            if (this.SelectedAsset == null)
            {
                if (model is AssetTextureModel)
                    this.SelectedAsset = new AssetTexture();
                else if (model is AssetGeometryModel)
                    this.SelectedAsset = new AssetGeometry();
            }

            if (assetPathSelectorModel.SelectedAsset != null)
                this.SelectedAsset.SetViewModel(assetPathSelectorModel.SelectedAsset);
        }

        public IModel GetModel()
        {
            AssetPathSelectorModel model = new AssetPathSelectorModel();
            model.ID = this.ID;
            if (this.SelectedAsset != null)
                model.SelectedAsset = (IAssetModel)this.SelectedAsset.GetModel();
            return model;
        }
    }
}
