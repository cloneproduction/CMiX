// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefabs
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ModifierPanelAttribute : Attribute
    {
        public Type PanelOwner { get; }
        public ModifierPanelAttribute(Type panelOwner) => PanelOwner = panelOwner;
    }
}
