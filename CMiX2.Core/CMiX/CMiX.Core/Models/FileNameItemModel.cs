// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    [Serializable]
    public class FileNameItemModel
    {
        public FileNameItemModel()
        {

        }

        public string FileName { get; set; }
        public bool FileIsSelected { get; set; }
        public FileNameItemModel SelectedFileNameItem { get; set; }
    }
}
