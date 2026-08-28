// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class BeatModifier : UserControl
    {
        public BeatModifier()
        {
            InitializeComponent();

            // The WPF view fed ItemsControl.AlternationIndex into the CurrentStepConverter.
            // Avalonia has no alternation index, so the container index is published
            // through the container's Tag, which the multi binding in the item template reads.
            stepsItemsControl.ContainerPrepared += (sender, e) => e.Container.Tag = e.Index;
            stepsItemsControl.ContainerIndexChanged += (sender, e) => e.Container.Tag = e.NewIndex;
        }
    }
}
