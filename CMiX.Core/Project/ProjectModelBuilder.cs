// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Compositing
{
    // Builds the ProjectModel shape shared by the normal save path and the crash emergency save,
    // so the two cannot drift when the project model grows a field.
    public static class ProjectModelBuilder
    {
        public static ProjectModel Build(Project project) => (ProjectModel)project.ToModel();
    }
}
