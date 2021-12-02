using System.Collections.Generic;
using Event_Manager;
using Event_Manager.Events;
using Gameplay.Objects;
using Gameplay.Scriptable_Objects;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Info_Panel
{
    public class InfoPanelView : MonoBehaviour
    {
        #region Events

        private void OnEnable()
        {
            controller = new InfoPanelController(new InfoPanelModel(), this);
            EventManager.AddListener<SelectedObjectChangedEvent>(controller.OnSelectedObjChanged);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<SelectedObjectChangedEvent>(controller.OnSelectedObjChanged);
        }

        #endregion

        [SerializeField]
        private GameObject container;

        [SerializeField]
        private TextMeshProUGUI selectedObjectNameText;

        [SerializeField]
        private Image selectedObjectImage;

        [SerializeField]
        private TextMeshProUGUI selectedObjectStatText;

        [Header("Products")]
        [SerializeField]
        private GameObject productionLayout;

        [SerializeField]
        private TextMeshProUGUI productionHeader;

        [SerializeField]
        private GameObject UnitButtonPrefab;

        [Header("Controller")]
        public InfoPanelController controller;
        
        private void Awake()
        {
            container.SetActive(false);
        }

        private void OnDestroy()
        {
            controller = null;
        }

        /// <summary>
        /// Creates unit buttons from <see cref="units"/> parameter.
        /// </summary>
        /// <param name="unitSpawner">Building with units to setup buttons and spawn units.</param>
        /// <param name="units">Units to create buttons.</param>
        public void UpdateUnitButtons(BuildingWithUnits unitSpawner, List<UnitSO> units)
        {
            SetProductionPanelActive(true);
            foreach (var unit in units)
            {
                var btn = Instantiate(UnitButtonPrefab, productionLayout.transform);
                btn.GetComponent<UnitButton>().Setup(unit, unitSpawner);
            }
        }
        
        public void DisableInfoPanel()
        {
            container.SetActive(false);
        }
        
        public void SetProductionPanelActive(bool value)
        {
            productionLayout.SetActive(value);
            productionHeader.text = value ? "Production" : string.Empty;
        }

        public void DeleteAllProductionButtons()
        {
            productionLayout.transform.DeleteAllChildren();
        }

        public void UpdateUI(string objectName, Sprite objectSprite, string objectStats)
        {
            container.SetActive(true);
            selectedObjectNameText.text = objectName;
            selectedObjectImage.sprite = objectSprite;
            selectedObjectStatText.text = objectStats;
        }

        public void UpdateStats(string objectStats)
        {
            selectedObjectStatText.text = objectStats;
        }
    }
}