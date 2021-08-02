// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using System.Windows.Media;
using CMiX.Core.Mathematics;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using ColorMine.ColorSpaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ColorPicker : ObservableRecipient, IRecipient<IMessage>, IControl
    {
        public ColorPicker(ColorPickerModel colorPickerModel)
        {
            SelectedColor = ColorExtensions.HexStringToColor(colorPickerModel.SelectedColor);
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);

            this.ID = colorPickerModel.ID;
            Red = SelectedColor.R;
            Green = SelectedColor.G;
            Blue = SelectedColor.B;
            MouseDown = false;

            PreviewMouseDownCommand = new RelayCommand(PreviewMouseDown);
            PreviewMouseUpCommand = new RelayCommand(PreviewMouseUp);
            PreviewMouseLeaveCommand = new RelayCommand(PreviewMouseLeave);
        }


        public Guid ID { get; set; }
        public ICommand PreviewMouseDownCommand { get; set; }
        public ICommand PreviewMouseUpCommand { get; set; }
        public ICommand PreviewMouseLeaveCommand { get; set; }
        public bool MouseDown { get; set; }


        private Color _selectedColor;
        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                SetProperty(ref _selectedColor, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
            }
        }


        private byte _red;
        public byte Red
        {
            get => _red;
            set
            {
                if (_red != value)
                {
                    SetProperty(ref _red, value);

                    var hsv = new Rgb() { R = _selectedColor.R, G = _selectedColor.G, B = _selectedColor.B }.To<Hsv>();

                    _hue = hsv.H;
                    OnPropertyChanged(nameof(Hue));
                    _sat = hsv.S;
                    OnPropertyChanged(nameof(Sat));
                    _val = hsv.V;
                    OnPropertyChanged(nameof(Val));

                    SelectedColor = Color.FromRgb(_red, _green, _blue);
                }
            }
        }


        private byte _green;
        public byte Green
        {
            get => _green;
            set
            {
                if (_green != value)
                {
                    SetProperty(ref _green, value);

                    var hsv = new Rgb() { R = _selectedColor.R, G = _selectedColor.G, B = _selectedColor.B }.To<Hsv>();
                    _hue = hsv.H;
                    OnPropertyChanged(nameof(Hue));
                    _sat = hsv.S;
                    OnPropertyChanged(nameof(Sat));
                    _val = hsv.V;
                    OnPropertyChanged(nameof(Val));

                    SelectedColor = Color.FromRgb(_red, _green, _blue);
                }
            }
        }


        private byte _blue;
        public byte Blue
        {
            get => _blue;
            set
            {
                if (_blue != value)
                {
                    SetProperty(ref _blue, value);

                    var hsv = new Rgb() { R = _selectedColor.R, G = _selectedColor.G, B = _selectedColor.B }.To<Hsv>();

                    _hue = hsv.H;
                    OnPropertyChanged(nameof(Hue));
                    _sat = hsv.S;
                    OnPropertyChanged(nameof(Sat));
                    _val = hsv.V;
                    OnPropertyChanged(nameof(Val));

                    SelectedColor = Color.FromRgb(_red, _green, _blue);
                }
            }
        }


        private double _hue;
        public double Hue
        {
            get => _hue;
            set
            {
                if (_hue != value)
                {
                    SetProperty(ref _hue, value);

                    var hsv = new Rgb() { R = SelectedColor.R, G = SelectedColor.G, B = SelectedColor.B }.To<Hsv>();
                    hsv.H = value;

                    var rgb = hsv.To<Rgb>();
                    _red = (byte)rgb.R;
                    OnPropertyChanged(nameof(Red));
                    _green = (byte)rgb.G;
                    OnPropertyChanged(nameof(Green));
                    _blue = (byte)rgb.B;
                    OnPropertyChanged(nameof(Blue));

                    SelectedColor = Color.FromRgb(_red, _green, _blue);
                }
            }
        }


        private double _sat;
        public double Sat
        {
            get => _sat;
            set
            {
                if (_sat != value)
                {
                    SetProperty(ref _sat, value);

                    var hsv = new Rgb() { R = SelectedColor.R, G = SelectedColor.G, B = SelectedColor.B }.To<Hsv>();
                    hsv.V = _val;
                    hsv.S = value;
                    hsv.H = _hue;

                    var rgb = hsv.To<Rgb>();
                    _red = (byte)rgb.R;
                    OnPropertyChanged(nameof(Red));
                    _green = (byte)rgb.G;
                    OnPropertyChanged(nameof(Green));
                    _blue = (byte)rgb.B;
                    OnPropertyChanged(nameof(Blue));

                    SelectedColor = Color.FromRgb(_red, _green, _blue);
                }
            }
        }


        private double _val;
        public double Val
        {
            get => _val;
            set
            {
                if (_val != value)
                {
                    SetProperty(ref _val, value);

                    var hsv = new Rgb() { R = SelectedColor.R, G = SelectedColor.G, B = SelectedColor.B }.To<Hsv>();
                    hsv.V = value;
                    hsv.S = _sat;
                    hsv.H = _hue;

                    if (value > 0)
                    {
                        var rgb = hsv.To<Rgb>();
                        _red = (byte)rgb.R;
                        OnPropertyChanged(nameof(Red));
                        _green = (byte)rgb.G;
                        OnPropertyChanged(nameof(Green));
                        _blue = (byte)rgb.B;
                        OnPropertyChanged(nameof(Blue));

                        SelectedColor = Color.FromRgb(_red, _green, _blue);
                    }
                    else
                    {
                        SelectedColor = Color.FromRgb(0, 0, 0);
                    }
                }
            }
        }


        public void PreviewMouseDown()
        {
            MouseDown = true;
        }

        public void PreviewMouseUp()
        {
            MouseDown = false;
        }

        public void PreviewMouseLeave()
        {
            MouseDown = false;
        }


        public void SetViewModel(IModel model)
        {
            ColorPickerModel colorPickerModel = model as ColorPickerModel;
            this.ID = colorPickerModel.ID;
            this.SelectedColor = colorPickerModel.SelectedColor.HexStringToColor();
            Console.WriteLine("ColorPicker SetViewModel Color " + SelectedColor);
        }

        public IModel GetModel()
        {
            ColorPickerModel model = new ColorPickerModel();
            model.ID = this.ID;
            model.SelectedColor = this.SelectedColor.ColorToHexString();
            return model;
        }

        public void Receive(IMessage message)
        {
            if (message is MessageUpdateViewModel msg)
            {
                if (msg.ID == this.ID)
                    this.SetViewModel(msg.Model);
            }
        }
    }
}
