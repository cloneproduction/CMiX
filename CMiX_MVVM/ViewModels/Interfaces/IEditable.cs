﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMiX.MVVM.ViewModels
{
    public interface IEditable
    {
        bool IsRenaming { get; set; }
        string Name { get; set; }
    }
}
