// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using CMiX.Core.Mathematics;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class RandomXYZ : ObservableObject, IControl, ITransformModifier
    {
        public RandomXYZ(RandomXYZModel randomXYZModel, Guid componentID)
        {
            this.ID = randomXYZModel.ID;
            this.Name = randomXYZModel.Name;

            Counter = new Counter(randomXYZModel.CounterModel);
            Counter.CounterChangeEvent += Counter_CounterChangeEvent;

            Easing = new Easing(randomXYZModel.EasingModel);
            BeatModifier = new BeatModifier(randomXYZModel.BeatModifierModel, componentID);
            Transforms = new ObservableCollection<Transform>();

            SelectedModifierType = ModifierType.OBJECT;

            RandomizeLocation = new ToggleButton(randomXYZModel.RandomizeLocation);
            RandomizeLocation.IsChecked = true;
            LocationX = new Slider(nameof(LocationX), randomXYZModel.LocationX);
            LocationY = new Slider(nameof(LocationY), randomXYZModel.LocationY);
            LocationZ = new Slider(nameof(LocationZ), randomXYZModel.LocationZ);

            RandomizeScale = new ToggleButton(randomXYZModel.RandomizeScale);
            RandomizeScale.IsChecked = true;
            ScaleX = new Slider(nameof(ScaleX), randomXYZModel.ScaleX);
            ScaleY = new Slider(nameof(ScaleY), randomXYZModel.ScaleY);
            ScaleZ = new Slider(nameof(ScaleZ), randomXYZModel.ScaleZ);

            RandomizeRotation = new ToggleButton(randomXYZModel.RandomizeScale);
            RandomizeRotation.IsChecked = true;
            RotationX = new Slider(nameof(RotationX), randomXYZModel.RotationX);
            RotationY = new Slider(nameof(RotationY), randomXYZModel.RotationY);
            RotationZ = new Slider(nameof(RotationZ), randomXYZModel.RotationZ);
        }


        public Guid ID { get; set; }
        public TransformModifierNames Name { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public Counter Counter { get; set; }
        public ObservableCollection<Transform> Transforms { get; set; }

        public ToggleButton RandomizeLocation { get; set; }
        public Slider LocationX { get; set; }
        public Slider LocationY { get; set; }
        public Slider LocationZ { get; set; }

        public ToggleButton RandomizeScale { get; set; }
        public Slider ScaleX { get; set; }
        public Slider ScaleY { get; set; }
        public Slider ScaleZ { get; set; }

        public ToggleButton RandomizeRotation { get; set; }
        public Slider RotationX { get; set; }
        public Slider RotationY { get; set; }
        public Slider RotationZ { get; set; }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private bool _randomizeLocationIsExpanded;
        public bool RandomizeLocationIsExpanded
        {
            get => _randomizeLocationIsExpanded;
            set => SetProperty(ref _randomizeLocationIsExpanded, value);
        }

        private bool _randomizeScaleIsExpanded;
        public bool RandomizeScaleIsExpanded
        {
            get => _randomizeScaleIsExpanded;
            set => SetProperty(ref _randomizeScaleIsExpanded, value);
        }

        private bool _randomizeRotationIsExpanded;
        public bool RandomizeRotationIsExpanded
        {
            get => _randomizeRotationIsExpanded;
            set => SetProperty(ref _randomizeRotationIsExpanded, value);
        }

        private ModifierType _selectedModifierType;
        public ModifierType SelectedModifierType
        {
            get => _selectedModifierType;
            set => SetProperty(ref _selectedModifierType, value);
        }


        private void Counter_CounterChangeEvent(object sender, CounterEventArgs e)
        {
            Transforms.Clear();
            for (int i = 0; i < e.Value; i++)
            {
                //Transforms.Add(new Transform("Transform" + i, this));
            }
        }


        private Vector3D[] PreviousLocation { get; set; }
        private Vector3D[] PreviousScale { get; set; }
        private Vector3D[] PreviousRotation { get; set; }

        private Vector3D[] NextLocation { get; set; }
        private Vector3D[] NextScale { get; set; }
        private Vector3D[] NextRotation { get; set; }


        public void UpdateOnBeatTick(double period)
        {
            PreviousLocation = NextLocation;
            PreviousScale = NextScale;
            PreviousRotation = NextRotation;

            foreach (var transform in Transforms)
            {
                var index = Transforms.IndexOf(transform);

                if (BeatModifier.CheckHitOnBeatTick())
                {
                    NextLocation[index] = new Vector3D(0.0, 0.0, 0.0);
                    NextScale[index] = new Vector3D(1.0, 1.0, 1.0);
                    NextRotation[index] = new Vector3D(0.0, 0.0, 0.0);

                    if (RandomizeLocation.IsChecked)
                    {
                        var org_X = transform.Translate.X.Amount;
                        var org_Y = transform.Translate.Y.Amount;
                        var org_Z = transform.Translate.Z.Amount;

                        var new_X = MathUtils.RandomDouble(-LocationX.Amount + 2.0, LocationX.Amount);
                        var new_Y = MathUtils.RandomDouble(-LocationY.Amount + 2.0, LocationY.Amount);
                        var new_Z = MathUtils.RandomDouble(-LocationZ.Amount + 2.0, LocationZ.Amount);

                        var aX = new_X * org_X;
                        var aY = new_Y * org_Y;
                        var aZ = new_Z * org_Z;

                        NextLocation[index] = new Vector3D(aX, aY, aZ);
                    }

                    if (RandomizeScale.IsChecked)
                    {
                        var org_X = transform.Scale.X.Amount;
                        var org_Y = transform.Scale.Y.Amount;
                        var org_Z = transform.Scale.Z.Amount;

                        var new_X = MathUtils.RandomDouble(-ScaleX.Amount + 2.0, ScaleX.Amount);
                        var new_Y = MathUtils.RandomDouble(-ScaleY.Amount + 2.0, ScaleY.Amount);
                        var new_Z = MathUtils.RandomDouble(-ScaleZ.Amount + 2.0, ScaleZ.Amount);

                        var aX = new_X * org_X;
                        var aY = new_Y * org_Y;
                        var aZ = new_Z * org_Z;

                        NextScale[index] = new Vector3D(aX, aY, aZ);
                    }

                    if (RandomizeRotation.IsChecked)
                    {
                        var org_X = transform.Rotation.X.Amount;
                        var org_Y = transform.Rotation.Y.Amount;
                        var org_Z = transform.Rotation.Z.Amount;

                        var new_X = MathUtils.RandomDouble(-RotationX.Amount + 2.0, RotationX.Amount);
                        var new_Y = MathUtils.RandomDouble(-RotationY.Amount + 2.0, RotationY.Amount);
                        var new_Z = MathUtils.RandomDouble(-RotationZ.Amount + 2.0, RotationZ.Amount);

                        var aX = new_X * org_X;
                        var aY = new_Y * org_Y;
                        var aZ = new_Z * org_Z;

                        NextRotation[index] = new Vector3D(aX, aY, aZ);
                    }
                }
            }
        }

        public void UpdateOnGameLoop(double period)
        {
            if (Transforms.Count != PreviousLocation.Length)
            {

            }


            foreach (var transform in Transforms)
            {
                var index = Transforms.IndexOf(transform);

                if (Easing.IsEnabled)
                {
                    double eased = Easings.Interpolate((float)period, Easing.SelectedEasing);

                    Vector3D lerpedLocation = MathUtils.Lerp(PreviousLocation[index], NextLocation[index], eased);
                    Vector3D lerpedScale = MathUtils.Lerp(PreviousScale[index], NextScale[index], eased);
                    Vector3D lerpedRotation = MathUtils.Lerp(PreviousRotation[index], NextRotation[index], eased);

                    if (RandomizeLocation.IsChecked)
                    {
                        transform.Translate.X.Amount = lerpedLocation.X;
                        transform.Translate.Y.Amount = lerpedLocation.Y;
                        transform.Translate.Z.Amount = lerpedLocation.Z;
                    }

                    if (RandomizeScale.IsChecked)
                    {
                        transform.Scale.X.Amount = lerpedScale.X;
                        transform.Scale.Y.Amount = lerpedScale.Y;
                        transform.Scale.Z.Amount = lerpedScale.Z;
                    }

                    if (RandomizeScale.IsChecked)
                    {
                        transform.Rotation.X.Amount = lerpedRotation.X;
                        transform.Rotation.Y.Amount = lerpedRotation.Y;
                        transform.Rotation.Z.Amount = lerpedRotation.Z;
                    }
                }
                else
                {
                    if (RandomizeLocation.IsChecked)
                    {
                        transform.Translate.X.Amount = NextLocation[index].X;
                        transform.Translate.Y.Amount = NextLocation[index].Y;
                        transform.Translate.Z.Amount = NextLocation[index].Z;
                    }

                    if (RandomizeScale.IsChecked)
                    {
                        transform.Scale.X.Amount = NextScale[index].X;
                        transform.Scale.Y.Amount = NextScale[index].Y;
                        transform.Scale.Z.Amount = NextScale[index].Z;
                    }

                    if (RandomizeScale.IsChecked)
                    {
                        transform.Rotation.X.Amount = NextRotation[index].X;
                        transform.Rotation.Y.Amount = NextRotation[index].Y;
                        transform.Rotation.Z.Amount = NextRotation[index].Z;
                    }
                }
            }
        }


        public void SetViewModel(IModel model)
        {
            RandomXYZModel randomXYZModel = model as RandomXYZModel;
            this.ID = randomXYZModel.ID;
            this.Name = randomXYZModel.Name;
            this.BeatModifier.SetViewModel(randomXYZModel.BeatModifierModel);
            this.Counter.SetViewModel(randomXYZModel.CounterModel);

            this.LocationX.SetViewModel(randomXYZModel.LocationX);
            this.LocationY.SetViewModel(randomXYZModel.LocationY);
            this.LocationZ.SetViewModel(randomXYZModel.LocationZ);
            this.RandomizeLocation.SetViewModel(randomXYZModel.RandomizeLocation);

            this.ScaleX.SetViewModel(randomXYZModel.LocationX);
            this.ScaleY.SetViewModel(randomXYZModel.LocationY);
            this.ScaleZ.SetViewModel(randomXYZModel.LocationZ);
            this.RandomizeScale.SetViewModel(randomXYZModel.RandomizeLocation);

            this.RotationX.SetViewModel(randomXYZModel.LocationX);
            this.RotationY.SetViewModel(randomXYZModel.LocationY);
            this.RotationZ.SetViewModel(randomXYZModel.LocationZ);
            this.RandomizeRotation.SetViewModel(randomXYZModel.RandomizeLocation);
        }

        public IModel GetModel()
        {
            RandomXYZModel model = new RandomXYZModel();
            model.ID = this.ID;
            model.Name = this.Name;
            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            model.CounterModel = (CounterModel)this.Counter.GetModel();

            model.LocationX = (SliderModel)this.LocationX.GetModel();
            model.LocationY = (SliderModel)this.LocationY.GetModel();
            model.LocationZ = (SliderModel)this.LocationZ.GetModel();
            model.RandomizeLocation = (ToggleButtonModel)this.RandomizeLocation.GetModel();

            model.ScaleX = (SliderModel)this.ScaleX.GetModel();
            model.ScaleY = (SliderModel)this.ScaleY.GetModel();
            model.ScaleZ = (SliderModel)this.ScaleZ.GetModel();
            model.RandomizeScale = (ToggleButtonModel)this.RandomizeLocation.GetModel();

            model.RotationX = (SliderModel)this.RotationX.GetModel();
            model.RotationY = (SliderModel)this.RotationY.GetModel();
            model.RotationZ = (SliderModel)this.RotationZ.GetModel();
            model.RandomizeRotation = (ToggleButtonModel)this.RandomizeLocation.GetModel();
            return model;
        }
    }
}
