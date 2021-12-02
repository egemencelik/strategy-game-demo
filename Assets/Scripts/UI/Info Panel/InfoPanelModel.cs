using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Objects;
using Gameplay.Scriptable_Objects;

namespace UI.Info_Panel
{
    [Serializable]
    public class InfoPanelModel
    {
        public delegate void UnitsChangedEvent(BuildingWithUnits spawner, List<UnitSO> units);
        // Event to fire when unit list changes.
        public event UnitsChangedEvent OnUnitsChanged;

        private List<UnitSO> Units
        {
            get => units;
            set
            {
                if (units != value)
                {
                    units = value;
                    OnUnitsChanged?.Invoke(unitSpawner, value);
                }
            }
        }

        private List<UnitSO> units;

        public PlacedObject selectedObject;
        private BuildingWithUnits unitSpawner;

        public InfoPanelModel()
        {
            Units = new List<UnitSO>();
        }

        public void ChangeSelectedObject(PlacedObject obj)
        {
            selectedObject = obj;
            unitSpawner = null;
            // if selected object has units, update list.
            if (obj is BuildingWithUnits uSpawner)
            {
                unitSpawner = uSpawner;
                var spawnerSO = uSpawner.UnitSoSpawnerSo;
                Units = spawnerSO.units.ToList();
            }
            else
            {
                Units = new List<UnitSO>();
            }
        }
    }
}