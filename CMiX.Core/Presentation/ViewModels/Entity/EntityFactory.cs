//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using System;
//using CMiX.Core.Models;

//namespace CMiX.Core.Presentation.ViewModels
//{
//    public class EntityFactory
//    {
//        public EntityFactory()
//        {

//        }

//        public IEntity Create(Type type)
//        {
//            if (type == typeof(MeshEntity))
//                return new MeshEntity(new MeshEntityModel());

//            if(type == typeof(LightEntity))
//                return new LightEntity(new LightEntityModel());

//            return null;
//        }

//        public IEntity Create(IEntityModel entityModel)
//        {
//            if(entityModel is MeshEntityModel meshEntityModel)
//                return new MeshEntity(meshEntityModel);

//            if(entityModel is LightEntityModel lightEntityModel)
//                return new LightEntity(lightEntityModel);

//            return null;
//        }
//    }
//}
