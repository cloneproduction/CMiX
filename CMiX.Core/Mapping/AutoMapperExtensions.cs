//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using System.Reflection;
//using AutoMapper;
//using CMiX.Core.BaseControls;
//using CMiX.Core.Prefabs.Managers;

//namespace CMiX.Core.Mapping
//{
//    public static class AutoMapperExtensions
//    {
//        public static void MapGenericValueByConvention(this Profile profile)
//        {
//            // Open generic mapping
//            profile.CreateMap(typeof(GenericValue<>), typeof(GenericValueModel<>))
//                   .ReverseMap()
//                   .ConstructUsingServiceLocator(); // optional: uses DI if needed
//        }
//        public static void MapControlByConvention(this Profile profile, params Type[] sourceTypes)
//        {
//            if (sourceTypes == null || sourceTypes.Length == 0) return;

//            // All IControlModel types in the assembly
//            var controlModelInterface = typeof(IControlModel);
//            var models = controlModelInterface.Assembly.GetTypes()
//                .Where(t => controlModelInterface.IsAssignableFrom(t) &&
//                            !t.IsAbstract &&
//                            !t.IsInterface)
//                .ToList();

//            // 1️⃣ Concrete maps
//            foreach (var source in sourceTypes)
//            {
//                var modelName = source.Name + "Model";
//                var dest = models.FirstOrDefault(m => m.Name == modelName);
//                if (dest == null) continue;

//                profile.CreateMap(source, dest)
//                       .ReverseMap()
//                       .ConstructUsingServiceLocator();
//            }

//            // 2️⃣ Polymorphic map
//            var controlMap = profile.CreateMap<IControl, IControlModel>();
//            foreach (var source in sourceTypes)
//            {
//                var modelName = source.Name + "Model";
//                var dest = models.FirstOrDefault(m => m.Name == modelName);
//                if (dest == null) continue;

//                controlMap.Include(source, dest);
//            }

//            controlMap.ReverseMap()
//                      .ConstructUsingServiceLocator();
//        }

//        public static void MapControlByInterface(this Profile profile, List<Type> registeredTypes, Type filterInterface)
//        {
//            // Base interface for all filters
//            var controlModelInterface = typeof(IControlModel);

//            // 1Discover all concrete filters
//            var filters = filterInterface.Assembly.GetTypes()
//                .Where(t =>
//                    filterInterface.IsAssignableFrom(t) &&
//                    !t.IsAbstract &&
//                    !t.IsInterface)
//                .ToList();

//            // 2Discover all concrete models (IControlModel implementations)
//            var models = controlModelInterface.Assembly.GetTypes()
//                .Where(t =>
//                    controlModelInterface.IsAssignableFrom(t) &&
//                    !t.IsAbstract &&
//                    !t.IsInterface)
//                .ToList();

//            // 3Create concrete maps for each filter ↔ model
//            foreach (var filter in filters)
//            {
//                // Register the type for later use (DataTemplate generation)
//                registeredTypes.Add(filter);

//                var modelName = filter.Name + "Model";
//                var model = models.FirstOrDefault(m => m.Name == modelName);
//                if (model == null) continue;

//                // Concrete mapping
//                profile.CreateMap(filter, model)
//                    .ReverseMap()
//                    .ConstructUsingServiceLocator();
//            }

//            // Polymorphic mapping for IControl ↔ IControlModel
//            var polymorphicMap = profile.CreateMap<IControl, IControlModel>();

//            foreach (var filter in filters)
//            {
//                var modelName = filter.Name + "Model";
//                var model = models.FirstOrDefault(m => m.Name == modelName);
//                if (model == null) continue;

//                polymorphicMap.Include(filter, model);
//            }

//            polymorphicMap.ReverseMap()
//                          .ConstructUsingServiceLocator();
//        }
//    }
//}
