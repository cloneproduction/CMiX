﻿using System.Windows;
using System.Windows.Input;
using CMiX.MVVM.ViewModels;
using CMiX.MVVM.Models;

namespace CMiX.MVVM.ViewModels
{
    public class Geometry : Sendable
    {
        #region CONSTRUCTORS
        public Geometry(Beat beat) 
        {
            Instancer = new Instancer(beat);
            Transform = new Transform();
            Transform.SendChangeEvent += this.OnChildPropertyToSendChange;

            GeometryFX = new GeometryFX();

            AssetPathSelector = new AssetPathSelector<AssetGeometry>();

            CopyGeometryCommand = new RelayCommand(p => CopyGeometry());
            PasteGeometryCommand = new RelayCommand(p => PasteGeometry());
            ResetGeometryCommand = new RelayCommand(p => ResetGeometry());
        }
        #endregion

        #region PROPERTIES
        public ICommand CopyGeometryCommand { get; }
        public ICommand PasteGeometryCommand { get; }
        public ICommand ResetGeometryCommand { get; }

        public AssetPathSelector<AssetGeometry> AssetPathSelector { get; set; }

        public Transform Transform { get; set; }
        public Instancer Instancer { get; set; }
        public GeometryFX GeometryFX { get; set; }

        #endregion

        #region COPY/PASTE/RESET
        public void CopyGeometry()
        {
            IDataObject data = new DataObject();
            data.SetData("GeometryModel", this.GetModel(), false);
            Clipboard.SetDataObject(data);
        }

        public void PasteGeometry()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent("GeometryModel"))
            {
                //Mementor.BeginBatch();
                var geometrymodel = data.GetData("GeometryModel") as GeometryModel;
                this.SetViewModel(geometrymodel);
                //Mementor.EndBatch();
                //SendMessages(MessageAddress, geometrymodel);
            }
        }

        public void ResetGeometry()
        {
            GeometryModel geometrymodel = this.GetModel();
            this.Reset();
            //SendMessages(MessageAddress, geometrymodel);
        }


        public void Reset()
        {
            //Mementor.BeginBatch();
            Transform.Reset();
            GeometryFX.Reset();
            Instancer.Reset();
            //Mementor.EndBatch();
            //SendMessages(MessageAddress, GetModel());
        }
        #endregion
    }
}