// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentation.Extensions;
using CMiX.Core.Presentation.ViewModels.Components;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CMiX.Core.Presentation.ViewModels
{
    public class OutlinerDragDropManager : IDropTarget, IDragSource
    {
        private TreeViewItem ParentVisualSource;
        private TreeViewItem VisualTarget;
        private TreeViewItem VisualSource;
        private TreeViewItem ParentVisualTarget;

        private Component SourceComponent;
        private Component SourceComponentParent;
        private Component TargetComponent;
        private Component TargetComponentParent;

        private int TargetIndex = -1;
        private int SourceIndex = -1;

        private bool InsertPositionTargetItemCenter;

        public Action<TreeViewItem, TreeViewItem> DoDropInParent { get; set; }


        public void GetInfo(IDropInfo dropInfo)
        {
            TargetIndex = dropInfo.InsertIndex;
            SourceIndex = dropInfo.DragInfo.SourceIndex;

            VisualTarget = dropInfo.VisualTargetItem as TreeViewItem;
            VisualSource = dropInfo.DragInfo.VisualSourceItem as TreeViewItem;

            SourceComponent = dropInfo.DragInfo.SourceItem as Component;
            TargetComponent = dropInfo.TargetItem as Component;

            ParentVisualSource = VisualSource.FindParent<TreeViewItem>();// Utils.FindParent<TreeViewItem>(VisualSource);
            ParentVisualTarget = VisualTarget.FindParent<TreeViewItem>();// Utils.FindParent<TreeViewItem>(VisualTarget);

            InsertPositionTargetItemCenter = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter);

            if (ParentVisualTarget != null)
            {
                TargetComponentParent = ParentVisualTarget.DataContext as Component;
                SourceComponentParent = ParentVisualSource.DataContext as Component;
            }
        }

        public bool CheckTargetIsSameType(IDropInfo dropInfo)
        {
            if (SourceComponent.GetType() == TargetComponent.GetType())
            {
                if (dropInfo.InsertIndex == dropInfo.DragInfo.SourceIndex && dropInfo.InsertPosition == RelativeInsertPosition.AfterTargetItem
                    && SourceComponentParent != TargetComponentParent)
                {
                    return true;
                }
                else if (dropInfo.InsertIndex - 1 == dropInfo.DragInfo.SourceIndex && dropInfo.InsertPosition == RelativeInsertPosition.BeforeTargetItem
                    && SourceComponentParent != TargetComponentParent)
                {
                    return true;
                }
                else if (dropInfo.InsertIndex - 1 == dropInfo.DragInfo.SourceIndex && dropInfo.InsertPosition == RelativeInsertPosition.BeforeTargetItem)
                {
                    return false;
                }
                else if (dropInfo.InsertIndex == dropInfo.DragInfo.SourceIndex && dropInfo.InsertPosition == RelativeInsertPosition.AfterTargetItem)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
                return false;
        }

        public bool CheckParentIsSameType(TreeViewItem target, TreeViewItem source, RelativeInsertPosition relativeInsertPosition)
        {
            var parent = target.FindParent<TreeViewItem>();// Utils.FindParent<TreeViewItem>(target);
            if (parent == null)
            {
                return false;
            }
            else
            {
                var parentComponent = parent.DataContext as Component;
                var parentCollection = parentComponent.Components;
                var sourceComponent = source.DataContext as Component;
                var targetComponent = target.DataContext as Component;


                if (parentComponent.GetType() != sourceComponent.GetType())
                {
                    return false;
                }

                if (
                    parentCollection.Last() == targetComponent
                    && relativeInsertPosition == RelativeInsertPosition.AfterTargetItem
                    && parentComponent != sourceComponent)
                {
                    return true;
                }
                else
                {
                    return CheckParentIsSameType(parent, source, relativeInsertPosition);
                }
            }
        }

        public bool CheckItemIsLast(TreeViewItem target, TreeViewItem source, RelativeInsertPosition relativeInsertPosition)
        {
            var parent = target.FindParent<TreeViewItem>();// Utils.FindParent<TreeViewItem>(target);
            if (parent == null)
            {
                return true;
            }
            else
            {
                var collection = (ObservableCollection<Component>)parent.ItemsSource;
                var parentComponent = parent.DataContext as Component;
                var targetComponent = target.DataContext as Component;
                var sourceComponent = source.DataContext as Component;
                if (collection != null
                    && collection.Last() == targetComponent
                    && parentComponent != sourceComponent
                    && relativeInsertPosition == RelativeInsertPosition.AfterTargetItem)
                {
                    return CheckItemIsLast(parent, source, relativeInsertPosition);
                }
                else
                {
                    return false;
                }
            }
        }




        public void DropFromLastItem(TreeViewItem target, TreeViewItem source)
        {
            if (target != null && source != null)
            {
                TreeViewItem targetParent = target.FindParent<TreeViewItem>();// Utils.FindParent<TreeViewItem>(target);
                TreeViewItem sourceParent = source.FindParent<TreeViewItem>();// Utils.FindParent<TreeViewItem>(source);

                Component sourceComponent = source.DataContext as Component;
                Component targetParentComponent = targetParent.DataContext as Component;
                Component sourceParentComponent = sourceParent.DataContext as Component;

                if (targetParentComponent.GetType() == sourceComponent.GetType())
                {
                    Component targetGrandParentComponent = targetParent.FindParent<TreeViewItem>().DataContext as Component;// Utils.FindParent<TreeViewItem>(targetParent).DataContext as Component;

                    sourceParentComponent.RemoveComponent(sourceComponent);
                    targetGrandParentComponent.AddComponent(sourceComponent);
                    return;
                }

                DropFromLastItem(targetParent, source);
            }
        }

        public void DropOnSameTargetType(TreeViewItem target, TreeViewItem source)
        {
            if (target != null && source != null)
            {
                TreeViewItem targetParent = target.FindParent<TreeViewItem>();// Utils.FindParent<TreeViewItem>(target);
                TreeViewItem sourceParent = source.FindParent<TreeViewItem>();// Utils.FindParent<TreeViewItem>(source);

                Component sourceComponent = source.DataContext as Component;
                Component targetParentComponent = targetParent.DataContext as Component;
                Component sourceParentComponent = sourceParent.DataContext as Component;

                if (SourceIndex < TargetIndex && targetParentComponent == sourceParentComponent)
                {
                    sourceParentComponent.MoveComponent(SourceIndex, TargetIndex - 1);
                    return;
                }

                sourceParentComponent.RemoveComponent(sourceComponent);
                targetParentComponent.InsertComponent(TargetIndex, sourceComponent);
            }
        }

        public void DropInSameParentType(TreeViewItem target, TreeViewItem source)
        {
            if (target != null && source != null)
            {
                TreeViewItem targetParent = target.FindParent<TreeViewItem>();// Utils.FindParent<TreeViewItem>(target);
                TreeViewItem sourceParent = source.FindParent<TreeViewItem>(); //Utils.FindParent<TreeViewItem>(source);
                TreeViewItem targetGrandParent = targetParent.FindParent<TreeViewItem>();// Utils.FindParent<TreeViewItem>(targetParent);

                Component sourceComponent = source.DataContext as Component;
                Component targetParentComponent = targetParent.DataContext as Component;
                Component targetGrandParentComponent = targetGrandParent.DataContext as Component;
                Component sourceParentComponent = sourceParent.DataContext as Component;

                var targetParentIndex = targetGrandParentComponent.Components.IndexOf(targetParentComponent);

                if (targetParentComponent.GetType() == sourceComponent.GetType())
                {
                    if (sourceParentComponent == targetGrandParentComponent)
                    {
                        if (SourceIndex > targetParentIndex)
                            targetParentIndex += 1;

                        targetGrandParentComponent.MoveComponent(SourceIndex, targetParentIndex);
                    }
                    else
                    {
                        sourceParentComponent.RemoveComponent(sourceComponent);
                        targetGrandParentComponent.InsertComponent(targetParentIndex + 1, sourceComponent);
                    }
                }
                else
                {
                    DropInSameParentType(targetParent, source);
                }
            }
        }

        public void DropInDifferentParent(TreeViewItem target, TreeViewItem source)
        {
            if (target != null && source != null)
            {
                TreeViewItem sourceParent = source.FindParent<TreeViewItem>();// Utils.FindParent<TreeViewItem>(source);

                Component sourceComponent = source.DataContext as Component;
                Component targetComponent = target.DataContext as Component;
                Component sourceParentComponent = sourceParent.DataContext as Component;

                sourceParentComponent.RemoveComponent(sourceComponent);
                targetComponent.InsertComponent(0, sourceComponent);
            }
        }




        public void DragOver(IDropInfo dropInfo)
        {
            if (!dropInfo.Data.GetType().IsSubclassOf(typeof(Component)))
                return;

            GetInfo(dropInfo);

            if (TargetComponent == null || TargetComponentParent == null)
                return;

            if (VisualTarget == VisualSource)
                return;

            if (ParentVisualTarget == null)
                return;

            if (SourceComponentParent != TargetComponent && SourceComponentParent.GetType() != typeof(Project)) // can't over it's own parent
            {
                if (SourceComponentParent.GetType() == TargetComponent.GetType())
                {
                    DoDropInParent = DropInDifferentParent;
                    dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                }
                else if (!InsertPositionTargetItemCenter)
                {
                    if (CheckTargetIsSameType(dropInfo))
                    {
                        DoDropInParent = DropOnSameTargetType;
                        dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    }

                    else if (CheckParentIsSameType(VisualTarget, VisualSource, dropInfo.InsertPosition))
                    {
                        DoDropInParent = DropInSameParentType;
                        dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    }
                    else if (CheckItemIsLast(VisualTarget, VisualSource, dropInfo.InsertPosition))
                    {
                        DoDropInParent = DropFromLastItem;
                        dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    }
                }
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            DoDropInParent.Invoke(VisualTarget, VisualSource);
        }

        public void StartDrag(IDragInfo dragInfo)
        {
            dragInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
            dragInfo.Data = dragInfo.SourceItem;
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            var dragged = dragInfo.SourceItem;
            if (dragged is IEntity || dragged is Layer || dragged is Composition)
                return true;
            else
                return false;
        }

        public void Dropped(IDropInfo dropInfo)
        {

        }

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {

        }

        public void DragCancelled()
        {

        }

        public bool TryCatchOccurredException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void DragEnter(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        public void DragLeave(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }
    }
}
