// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Presentation.ViewModels.Assets
{
    public class AssetPathSelector : ObservableRecipient, IRecipient<IMessage>, IControl, IDropTarget
    {
        public AssetPathSelector(Asset defaultAsset, AssetPathSelectorModel assetPathSelectorModel)
        {
            this.ID = assetPathSelectorModel.ID;
            SelectedAsset = defaultAsset;
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
        }


        public Guid ID { get; set; }


        private Asset _selectedAsset;
        public Asset SelectedAsset
        {
            get => _selectedAsset;
            set
            {
                SetProperty(ref _selectedAsset, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAsset(SelectedAsset), MessageType.Out);

                if (value != null)
                    System.Console.WriteLine("SelectedAsset Name is " + SelectedAsset.Name);
            }
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

        public void Receive(IMessage message)
        {
            if (message is MessageAsset messageAsset)
            {
                var assetModel = messageAsset.AssetModel;

                if (assetModel is AssetGeometryModel assetGeometryModel)
                {
                    var asset = new AssetGeometry();
                    asset.SetViewModel(assetGeometryModel);
                    this.SelectedAsset = asset;
                    return;

                }

                if (assetModel is AssetTextureModel assetTextureModel)
                {
                    var asset = new AssetTexture();
                    asset.SetViewModel(assetTextureModel);
                    this.SelectedAsset = asset;
                    return;
                }
                Console.WriteLine(this.SelectedAsset.Path);
            }
        }
    }
}
