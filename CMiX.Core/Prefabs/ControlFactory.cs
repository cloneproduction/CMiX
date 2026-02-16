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

        public ControlFactory(
                ControlMessenger controlMessenger,
                MessageFactory messageFactory,
                Mapper mapper,
                IServiceProvider services)
        {
            _serviceProvider = services ;
            _messageFactory = messageFactory;
            _controlMessenger = controlMessenger;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a control from a ViewModel type using convention: ViewModelType => ViewModelType + "Model"
        /// </summary>
        public IControl Create(Type viewModelType)
        {
            if (viewModelType == null) throw new ArgumentNullException(nameof(viewModelType));

            // Resolve the model type by convention
            var modelTypeName = viewModelType.FullName + "Model";
            var modelType = viewModelType.Assembly.GetType(modelTypeName)
                            ?? throw new InvalidOperationException($"No model type found for {viewModelType.Name}");

            // Create an empty model instance
            var controlModel = (IControlModel)Activator.CreateInstance(modelType)!;

            // Delegate to the main Create(IControlModel) method
            return Create(controlModel);
        }

        /// <summary>
        /// Creates a control from a model instance and maps its properties
        /// </summary>
        public IControl Create(IControlModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            // Create the control instance
            var control = CreateControlInstance(model);
            control = _mapper.MapToViewModel(control, model);
            return control;
        }

        /// <summary>
        /// Resolves the control type for a given model and retrieves it from the service provider
        /// </summary>
        private IControl CreateControlInstance(IControlModel model)
        {
            var modelType = model.GetType();

            // Resolve the ViewModel type by removing the "Model" suffix
            var viewModelTypeName = modelType.FullName!.Replace("Model", "");
            var viewModelType = modelType.Assembly.GetType(viewModelTypeName)
                ?? throw new NotSupportedException($"No control type found for {modelType.Name}");

            // Retrieve the control instance from DI
            return (IControl)_serviceProvider.GetRequiredService(viewModelType);
        }
    }
}
