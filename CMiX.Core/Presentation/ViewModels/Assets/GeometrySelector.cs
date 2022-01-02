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
    public class GeometrySelector : ObservableRecipient, IRecipient<IMessage>, IControl, IDropTarget
    {
        public GeometrySelector(AssetGeometry defaultAsset, GeometrySelectorModel geometrySelectorModel)
        {
            this.ID = geometrySelectorModel.ID;
            SelectedAsset = defaultAsset;
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
        }


        public Guid ID { get; set; }


        private AssetGeometry _selectedAsset;
        public AssetGeometry SelectedAsset
        {
            get => _selectedAsset;
            set
            {
                SetProperty(ref _selectedAsset, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAsset(this, value), MessageType.Out);
                if (value != null)
                    Console.WriteLine("SelectedAsset Name is " + SelectedAsset.Name);
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
            GeometrySelectorModel assetPathSelectorModel = model as GeometrySelectorModel;
            this.ID = assetPathSelectorModel.ID;

            if (this.SelectedAsset == null)
            {
                this.SelectedAsset = new AssetGeometry();
            }

            if (assetPathSelectorModel.SelectedAsset != null)
                this.SelectedAsset.SetViewModel(assetPathSelectorModel.SelectedAsset);
        }

        public IModel GetModel()
        {
            GeometrySelectorModel model = new GeometrySelectorModel();
            model.ID = this.ID;
            if (this.SelectedAsset != null)
                model.SelectedAsset = (IAssetModel)this.SelectedAsset.GetModel();
            return model;
        }

        public void Receive(IMessage message)
        {
            if (message is MessageAsset messageAsset && message.ID == this.ID)
            {
                var assetModel = messageAsset.AssetModel;

                if (assetModel is AssetGeometryModel model)
                {
                    var asset = new AssetGeometry();
                    asset.SetViewModel(model);
                    this.SelectedAsset = asset;
                    Console.WriteLine("GeometrySelector Receive Asset with address " + SelectedAsset.Path);
                    return;

                }
            }
        }

        public void DragEnter(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        public void DragLeave(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }
    }
}
