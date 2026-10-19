// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Prefabs.Managers
{
    public class CollectionManager
    {
        public CollectionManager(ManagerData managerData,
                                 ControlRepository controlRepository,
                                 ControlFactory controlFactory)
        {
            ManagerData = managerData;
            ControlRepository = controlRepository;
            _controlFactory = controlFactory;
        }

        public ManagerData ManagerData { get; }
        public ControlRepository ControlRepository { get; }
        private readonly ControlFactory _controlFactory;

        public void ClearAll()
        {
            foreach (var item in ManagerData.Items.ToList())
                RemoveAndDispose(item);
        }

        private void AddControlToCollection(IControl prefab)
        {
            ControlRepository.AddControl(prefab, this);
            ManagerData.Insert(ManagerData.Items.Count, prefab);
        }

        public void AddItem(IControlModel controlModel)
        {
            var prefab = CreateOrReuse(controlModel);
            AddControlToCollection(prefab);
        }

        // Managers can share one control. A model with the ID of a known control gives back that control.
        private IControl CreateOrReuse(IControlModel controlModel)
        {
            var existing = controlModel.ID != Guid.Empty ? ControlRepository.GetControl(controlModel.ID) : null;
            return existing ?? _controlFactory.Create(controlModel);
        }

        public void InsertItem(IControl control, int index)
        {
            ControlRepository.AddControl(control, this);
            ManagerData.Insert(index, control);
        }

        // The index is kept inside the list, so a stale index from a peer cannot throw.
        public void InsertItem(IControlModel controlModel, int index)
        {
            var prefab = CreateOrReuse(controlModel);
            InsertItem(prefab, Math.Clamp(index, 0, ManagerData.Items.Count));
        }

        public void Select(Guid controlID) => ManagerData.Select(ControlRepository.GetControl(controlID));

        public void LoadItem(IControlModel controlModel)
        {
            var prefab = CreateOrReuse(controlModel);
            ControlRepository.AddControl(prefab, this);
            ManagerData.Items.Add(prefab);
        }

        private int RemoveFromList(IControl control)
        {
            var index = ManagerData.Remove(control);
            if (index >= 0)
                ControlRepository.RemoveControl(control, this);
            return index;
        }

        private void RemoveAndDispose(IControl control)
        {
            if (RemoveFromList(control) >= 0)
                OwnedControl.DisposeIfOrphaned(ControlRepository, control);
        }

        // Not disposed on the undoable delete path: the RemoveItemCommand owns the removed control until it is dropped.
        public int DeleteItem(IControl control) => control == null ? -1 : RemoveFromList(control);

        public void DeleteItem(Guid id)
        {
            var control = ManagerData.Items.FirstOrDefault(x => x.ID == id);
            if (control != null)
                RemoveAndDispose(control);
        }
    }
}
