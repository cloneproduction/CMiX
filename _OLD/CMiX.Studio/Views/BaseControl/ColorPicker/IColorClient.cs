// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.



using System.Windows.Media;

namespace CMiX.Studio.Views.BaseControl
{
    public interface IColorClient
    {
        //void ColorUpdated(Color color, IColorClient client);

        void Init(IColorManager colorManager);
    }
}
