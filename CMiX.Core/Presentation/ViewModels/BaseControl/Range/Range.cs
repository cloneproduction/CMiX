// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Range : ObservableObject, IControl, IRange
    {
        public Range(RangeModel rangeModel)
        {
            this.ID = rangeModel.ID;
            Minimum = rangeModel.Minimum;
            Maximum = rangeModel.Maximum;
        }


        public Guid ID { get; set; }

        private double _width;
        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        private double _minimum;
        public double Minimum
        {
            get => _minimum;
            set
            {
                SetProperty(ref _minimum, value);
                Width = Math.Abs(Maximum - Minimum);
            }
        }

        private double _maximum;
        public double Maximum
        {
            get => _maximum;
            set
            {
                SetProperty(ref _maximum, value);
                Width = Math.Abs(Maximum - Minimum);
            }
        }


        public void SetViewModel(IModel model)
        {
            RangeModel rangeModel = model as RangeModel;
            this.ID = rangeModel.ID;
            this.Minimum = rangeModel.Minimum;
            this.Maximum = rangeModel.Maximum;
        }

        public IModel GetModel()
        {
            IRangeModel model = new RangeModel();
            model.ID = this.ID;
            model.Minimum = this.Minimum;
            model.Maximum = this.Maximum;
            return model;
        }
    }
}
