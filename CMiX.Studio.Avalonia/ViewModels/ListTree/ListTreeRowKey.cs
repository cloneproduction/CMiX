// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Runtime.CompilerServices;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The model is the item of an item row or the list of a group row. A copy can later get a marker model.
    public sealed record ListTreeRowKey(object Model, ListTreeRowKey? Parent)
    {
        public static ListTreeRowKey Of(TreeNode node, ListTreeRowKey? parent) => new(ModelOf(node), parent);

        public static object ModelOf(TreeNode node) => (object?)node.Item ?? (object?)node.ChildManager ?? node;

        public bool Equals(ListTreeRowKey? other) =>
            other is not null && ReferenceEquals(Model, other.Model) && Equals(Parent, other.Parent);

        public override int GetHashCode() => HashCode.Combine(RuntimeHelpers.GetHashCode(Model), Parent);
    }
}
