﻿using CMiX.MVVM.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CMiX.MVVM.Controls
{
    public class BeatDisplay : ViewModel
    {
        public BeatDisplay()
        {
            //AnimationCollection = new ObservableCollection<AnimatedDouble>();
            //Storyboard = new Storyboard();
            //MakeCollection(Storyboard);

            AddIndexCommand = new RelayCommand(p => AddIndex());
            SubIndexCommand = new RelayCommand(p => SubIndex());
        }

        public ICommand AddIndexCommand { get; set; }
        public ICommand SubIndexCommand { get; set; }

        public void Resync()
        {
            //if(Storyboard != null)
            //{
            //    Storyboard.Stop();
            //    Storyboard.Begin();
            //}
        }

        //private void MakeCollection(Storyboard storyboard)
        //{
        //    storyboard.Children.Clear();
        //    AnimationCollection.Clear();
        //    double Multiplier = 1.0;

        //    for (int i = 0; i <= 3; i++)
        //    {
        //        Multiplier *= 2;
                
        //        CreateAnimation(this.Period, Multiplier, storyboard);
        //    }

        //    CreateAnimation(this.Period, 1, storyboard);

        //    Multiplier = 1.0;
        //    for (int i = 0; i <= 3; i++)
        //    {
        //        Multiplier /= 2;
        //        CreateAnimation(this.Period, Multiplier, storyboard);
        //    }
        //    storyboard.RepeatBehavior = RepeatBehavior.Forever;
        //    storyboard.Begin();
        //}

        //private void CreateAnimation(double period, double multiplier, Storyboard storyboard)
        //{
        //    if(period > 0)
        //    {
        //        var dummyDO = new AnimatedDouble();
        //        this.AnimationCollection.Add(dummyDO);

        //        var newda = new DoubleAnimation(0, 100, new Duration(TimeSpan.FromMilliseconds(period / multiplier)));
        //        newda.RepeatBehavior = RepeatBehavior.Forever;
        //        Storyboard.SetTarget(newda, dummyDO);
        //        Storyboard.SetTargetProperty(newda, new PropertyPath(AnimatedDouble.AnimationPositionProperty));
        //        storyboard.Children.Add(newda);
        //    }
        //}

        //private Storyboard Storyboard { get; set; }

        private BeatAnimations _beatAnimations;
        public BeatAnimations BeatAnimations
        {
            get => _beatAnimations;
            set => SetAndNotify(ref _beatAnimations, value);
        }


        private double _period;
        public double Period
        {
            get => _period;
            set
            {
                SetAndNotify(ref _period, value);
                //MakeCollection(Storyboard);
            }
        }

        public void AddIndex()
        {
            CurrentIndex++;
            if (CurrentIndex > MaxIndex)
                CurrentIndex = MaxIndex;
        }

        public void SubIndex()
        {
            CurrentIndex--;
            if (CurrentIndex < MinIndex)
                CurrentIndex = MinIndex;
        }

        private int MaxIndex = 7;
        private int MinIndex = 0;
        private int CurrentIndex = 4;

        public AnimatedDouble SelectedAnimation
        {
            get => AnimationCollection[CurrentIndex];
            //set => SetAndNotify(ref _selectedAnimation, value);
        }


        private ObservableCollection<AnimatedDouble> _animationCollection;
        public ObservableCollection<AnimatedDouble> AnimationCollection
        {
            get => _animationCollection;
            set => SetAndNotify(ref _animationCollection, value);
        }
    }
}
