// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Mapping;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core.Prefabs
{
    public class ControlFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Mapper _mapper;
        private readonly ControlMessenger _controlMessenger;
        private readonly MessageFactory _messageFactory;
        private readonly ControlActivationService _activationService;

        public ControlFactory(ControlMessenger controlMessenger,
                              MessageFactory messageFactory,
                              ControlActivationService activationService,
                              Mapper mapper,
                              IServiceProvider services)
        {
            _serviceProvider = services;
            _messageFactory = messageFactory;
            _controlMessenger = controlMessenger;
            _activationService = activationService;
            _mapper = mapper;
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
            control = _mapper.MapToViewModel(control, model);
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
    }
}
