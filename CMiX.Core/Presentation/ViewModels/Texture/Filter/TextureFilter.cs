// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TextureFilter : ObservableObject, IPrefab, IControl
    {
        public TextureFilter()
        {

        }

        public bool IsSelected { get; set; }
        public bool IsRenaming { get; set; }
        public string Name { get; set; }
        public Guid ID { get; set; }

        public IModel GetModel()
        {
            throw new NotImplementedException();
        }

        public void SetViewModel(IModel model)
        {
            throw new NotImplementedException();
        }
    }
}
