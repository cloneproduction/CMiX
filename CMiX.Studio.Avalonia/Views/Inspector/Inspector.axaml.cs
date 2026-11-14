// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Controls.Presenters;

namespace CMiX.Studio.Avalonia.Views
{
    // The editor of the node the project tree sends to the inspector (ProjectTree.InspectedNode). The DataContext is the ProjectTree.
    public partial class Inspector : UserControl
    {
        public Inspector()
        {
            InitializeComponent();
            editor.TemplateApplied += (_, _) =>
            {
                if (editor.Presenter is { } presenter)
                    presenter.PropertyChanged += (_, e) =>
                    {
                        if (e.Property == ContentPresenter.ChildProperty && e.NewValue is not null)
                            EditorViewsBuilt++;
                    };
            };
        }

        // The number of editor views that the data templates built. A budget test reads it.
        internal int EditorViewsBuilt { get; private set; }
    }
}
