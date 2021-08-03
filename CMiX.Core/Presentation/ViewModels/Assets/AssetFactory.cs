// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Presentation.ViewModels.Assets
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

        private AssetTexture CreateAssetTexture(string name, string path)
        {
            return new AssetTexture(name, path);
        }

        private AssetGeometry CreateAssetGeometry(string name, string path)
        {
            return new AssetGeometry(name, path);
        }


        public IAsset CreateAsset(string fileType, string fileName, string filePath)
        {
            IAsset asset = null;

            if (Enum.IsDefined(typeof(TextureFileType), fileType))
            {
                return CreateAssetTexture(fileName, filePath);
            }

            if (Enum.IsDefined(typeof(GeometryFileType), fileType))
            {
                return CreateAssetGeometry(fileName, filePath);
            }

            return asset;
        }
    }
}
