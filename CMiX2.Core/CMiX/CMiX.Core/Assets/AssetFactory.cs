// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.ViewModels.Assets
{
    public class AssetFactory
    {
        public AssetFactory()
        {

        }

        public AssetDirectory CreateRootDirectory(string name)
        {
            return new AssetDirectory(name) { IsRoot = true };
        }

        public AssetDirectory CreateDirectory(string name)
        {
            return new AssetDirectory(name);
        }


        public IAsset CreateAsset(string fileType, string fileName, string filePath)
        {
            IAsset asset = null;

            if (Enum.IsDefined(typeof(TextureFileType), fileType))
            {
                return new AssetImage(fileName, filePath);
            }

            if (Enum.IsDefined(typeof(GeometryFileType), fileType))
            {
                return new AssetGeometry(fileName, filePath);
            }

            if(Enum.IsDefined(typeof(VideoFileType), fileType))
            {
                return new AssetVideo(fileName, filePath);
            }

            return asset;
        }
    }
}
