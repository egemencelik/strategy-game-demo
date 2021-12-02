using Gameplay.Objects;
using Gameplay.Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Info_Panel
{
    public class UnitButton : MonoBehaviour
    {
        private Image image;
        private TextMeshProUGUI text;

        private UnitSO unitSo;
        private BuildingWithUnits unitSpawner;

        private void Awake()
        {
            image = GetComponentInChildren<Image>();
            text = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        /// <param name="unitSo">Scriptable object to get data from.</param>
        /// <param name="unitSpawnerObject">Unit spawner building to set.</param>
        public void Setup(UnitSO unitSo, BuildingWithUnits unitSpawnerObject)
        {
            if (!image) image = GetComponentInChildren<Image>();
            if (!text) text = GetComponentInChildren<TextMeshProUGUI>();
        
            this.unitSo = unitSo;
            unitSpawner = unitSpawnerObject;
            image.sprite = unitSo.sprite;
            text.text = unitSo.objectName;
        }

        /// <summary>
        /// Spawns unit from current unit spawner.
        /// </summary>
        public void Spawn()
        {
            unitSpawner.Spawn(unitSo);
        }
    }
}