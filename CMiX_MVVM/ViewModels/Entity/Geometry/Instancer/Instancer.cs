﻿using CMiX.MVVM.Interfaces;
using CMiX.MVVM.Models;
using CMiX.MVVM.Services;
using CMiX.MVVM.ViewModels.Mediator;
using CMiX.MVVM.ViewModels.Observer;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CMiX.MVVM.ViewModels
{
    public class Instancer : Sender, ITransform, ISubject
    {
        public Instancer(string name, IColleague parentSender, MasterBeat beat) :base(name, parentSender)
        {
            Observers = new List<IObserver>();
            Counter = new Counter(nameof(Counter), this);
            Counter.CounterChangeEvent += Counter_CounterChangeEvent;

            Transform = new Transform(nameof(Transform), this);

            TranslateModifier = new XYZModifier(nameof(TranslateModifier), this, new Vector3D(0.0, 0.0, 0.0), beat);
            ScaleModifier = new XYZModifier(nameof(ScaleModifier), this, new Vector3D(1.0, 1.0, 1.0), beat);
            RotationModifier = new XYZModifier(nameof(RotationModifier), this, new Vector3D(0.0, 0.0, 0.0), beat);

            UniformScale = new AnimParameter(nameof(UniformScale), this, 1.0, beat);

            Attach(TranslateModifier);
            Attach(ScaleModifier);
            Attach(RotationModifier);
            Attach(UniformScale);

            NoAspectRatio = false;
        }

        private void Counter_CounterChangeEvent(object sender, CounterEventArgs e)
        {
            this.Notify(e.Value);
        }

        public override void Receive(Message message)
        {
            this.SetViewModel(message.Obj as InstancerModel);
        }

        public void Attach(IObserver observer)
        {
            Observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            Observers.Remove(observer);
        }


        public void Notify(int count)
        {
            foreach (var observer in Observers)
            {
                observer.Update(this.Counter.Count);
            }
        }

        private bool _noAspectRatio;
        public bool NoAspectRatio
        {
            get => _noAspectRatio;
            set => SetAndNotify(ref _noAspectRatio, value);
        }

        private List<IObserver> Observers { get; set; }

        public Transform Transform { get; set; }
        public Counter Counter { get; set; }
        public XYZModifier TranslateModifier { get; set; }
        public XYZModifier ScaleModifier { get; set; }
        public XYZModifier RotationModifier { get; set; }
        public AnimParameter UniformScale { get; set; }
    }
}