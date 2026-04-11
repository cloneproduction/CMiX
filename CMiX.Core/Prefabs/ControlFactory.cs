// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core.Prefabs
{
    public class ControlFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ControlActivationService _activationService;
        private readonly ControlRepository _controlRepository;

        public ControlFactory(
            ControlActivationService activationService,
            ControlRepository controlRepository,
            IServiceProvider services)
        {
            _serviceProvider = services;
            _activationService = activationService;
            _controlRepository = controlRepository;
        }

        public IControl Create(Type viewModelType)
        {
            if (viewModelType == null) throw new ArgumentNullException(nameof(viewModelType));
            var modelTypeName = viewModelType.FullName + "Model";
            var modelType = viewModelType.Assembly.GetType(modelTypeName)
                            ?? throw new InvalidOperationException($"No model type found for {viewModelType.Name}");
            var controlModel = (IControlModel)Activator.CreateInstance(modelType)!;
            return Create(controlModel);
        }

        public IControl Create(IControlModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var control = CreateControlInstance(model);
            control.FromModel(model);
            NameControl(control);
            _activationService.ActivateAll();
            return control;
        }

        private IControl CreateControlInstance(IControlModel model)
        {
            var modelType = model.GetType();
            var viewModelTypeName = modelType.FullName!.Replace("Model", "");
            var viewModelType = modelType.Assembly.GetType(viewModelTypeName)
                ?? throw new NotSupportedException($"No control type found for {modelType.Name}");
            return (IControl)_serviceProvider.GetRequiredService(viewModelType);
        }

        private void NameControl(IControl control)
        {
            if (control is not IPrefab prefab) return;

            var typeName = control.GetType().Name;
            var existingNames = _controlRepository.Controls
                .OfType<IPrefab>()
                .Select(x => x.PrefabService.Name.Value)
                .ToHashSet();

            string newName = typeName;
            var count = 1;
            while (existingNames.Contains(newName))
            {
                newName = $"{typeName}.{count:000}";
                count++;
            }

            prefab.PrefabService.Name.IsActive = false;
            prefab.PrefabService.Name.Value = newName;
            prefab.PrefabService.Name.IsActive = true;
        }
    }
}
