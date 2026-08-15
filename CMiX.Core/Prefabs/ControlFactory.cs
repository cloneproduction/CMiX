// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.Specialized;
using System.ComponentModel;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
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
            _controlRepository.Controls.CollectionChanged += OnControlsChanged;
        }

        // Names of the prefabs currently in the repository, kept so Create does not rebuild the
        // whole set on every call. Adds fill it directly; anything that could invalidate an entry,
        // a removal or a rename through the editable text box, only marks it stale so the next
        // Create rebuilds once. Correctness therefore never depends on tracking a rename precisely.
        private readonly HashSet<string> _names = new();
        private readonly HashSet<IPrefab> _tracked = new();
        private bool _namesAreStale;
        private bool _applyingName;

        private void OnControlsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                    if (item is IPrefab prefab) Untrack(prefab);

            if (e.NewItems != null)
                foreach (var item in e.NewItems)
                    if (item is IPrefab prefab) Track(prefab);

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var prefab in _tracked.ToList())
                    Untrack(prefab);
                _namesAreStale = true;
            }
        }

        private void Track(IPrefab prefab)
        {
            if (!_tracked.Add(prefab)) return;
            prefab.PrefabService.Name.PropertyChanged += OnTrackedNameChanged;
            var name = prefab.PrefabService.Name.Value;
            if (name != null) _names.Add(name);
        }

        private void Untrack(IPrefab prefab)
        {
            if (!_tracked.Remove(prefab)) return;
            prefab.PrefabService.Name.PropertyChanged -= OnTrackedNameChanged;
            _namesAreStale = true;
        }

        private void OnTrackedNameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_applyingName) return;
            if (e.PropertyName == nameof(GenericValue<string>.Value))
                _namesAreStale = true;
        }

        private void RebuildNames()
        {
            _names.Clear();
            foreach (var control in _controlRepository.Controls)
            {
                if (control is not IPrefab prefab) continue;
                var name = prefab.PrefabService.Name.Value;
                if (name != null) _names.Add(name);
            }
            _namesAreStale = false;
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

            var typeName = StringHelper.PascalCaseToDisplay(control.GetType().Name);

            prefab.PrefabService.Name.IsActive = false;
            _applyingName = true;

            if (control is IModifier)
            {
                prefab.PrefabService.Name.Value = typeName;
            }
            else
            {
                if (_namesAreStale) RebuildNames();

                // The search restarts at the bare type name on every call so a name freed by a
                // deletion is reused exactly as it was before the set was cached.
                string newName = typeName;
                var count = 1;
                while (_names.Contains(newName))
                {
                    newName = $"{typeName}.{count:000}";
                    count++;
                }

                prefab.PrefabService.Name.Value = newName;
            }

            _applyingName = false;
            prefab.PrefabService.Name.IsActive = true;
        }

        public static class StringHelper
        {
            public static string PascalCaseToDisplay(string name) =>
                System.Text.RegularExpressions.Regex.Replace(
                    System.Text.RegularExpressions.Regex.Replace(name, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"),
                    @"(\p{Ll})(\P{Ll})", "$1 $2");
        }
    }
}
