// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Presentation.Controls;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public interface IBeat
    {
        ICommand ResetCommand { get; set; }
        ICommand MultiplyCommand { get; set; }
        ICommand DivideCommand { get; set; }
        double Period { get; set; }
        AnimatedDouble AnimatedDouble { get; set; }
        double Multiplier { get; set; }

        void Reset();
        void Multiply();
        void Divide();
    }
}
