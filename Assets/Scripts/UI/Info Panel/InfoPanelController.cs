using System;
using System.Collections.Generic;
using Event_Manager.Events;
using Gameplay.Interfaces;
using Gameplay.Objects;
using Gameplay.Scriptable_Objects;

namespace UI.Info_Panel
{
    [Serializable]
    public class InfoPanelController
    {
        public InfoPanelModel model;
        public InfoPanelView view;

        public InfoPanelController(InfoPanelModel model, InfoPanelView view)
        {
            this.model = model;
            this.view = view;
            // sub to event
            model.OnUnitsChanged += OnUnitsChanged;
        }

        private void OnUnitsChanged(BuildingWithUnits spawner, List<UnitSO> units)
        {
            // if selected object doesnt have units, disable production part.
            if (spawner == null)
            {
                view.SetProductionPanelActive(false);
            }
            else
            {
                view.UpdateUnitButtons(spawner, units);
            }
        }

        // update health text when health changes
        private void OnHealthChanged()
        {
            var statText = $"Health: {model.selectedObject.Health}";

            if (model.selectedObject is IAttack attacker)
            {
                statText += $"\nDamage: {attacker.Damage}";
            }

            view.UpdateStats(statText);
        }


        public void OnSelectedObjChanged(SelectedObjectChangedEvent eventParam)
        {
            // if still same object, return
            if (eventParam.selectedObject == model.selectedObject) return;

            view.DeleteAllProductionButtons();
            
            // unsub from old object's health event
            if (model.selectedObject != null) model.selectedObject.OnHealthChanged -= OnHealthChanged;
            
            model.ChangeSelectedObject(eventParam.selectedObject);

            // if object is null disable panel
            if (eventParam.selectedObject == null)
            {
                view.DisableInfoPanel();
            }
            else
            {
                // sub to new selected object's health event
                model.selectedObject.OnHealthChanged += OnHealthChanged;
                
                var placedObjectTypeSO = eventParam.selectedObject.PlacedObjectSo;
                
                // set texts
                var statText = $"Health: {eventParam.selectedObject.Health}";

                if (eventParam.selectedObject is IAttack attacker)
                {
                    statText += $"\nDamage: {attacker.Damage}";
                }

                view.UpdateUI(placedObjectTypeSO.objectName, placedObjectTypeSO.sprite, statText);
            }
        }
    }
}