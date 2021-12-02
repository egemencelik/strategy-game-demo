using Event_Manager;
using Event_Manager.Events;
using Gameplay.Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Building_Panel
{
    public class BuildingButton : MonoBehaviour
    {
        private Button button;
        private TextMeshProUGUI text;
        private Image image;

        private PlacedObjectSO objectSO;

        private void Awake()
        {
            button = GetComponentInChildren<Button>();
            text = GetComponentInChildren<TextMeshProUGUI>();
            image = GetComponentInChildren<Image>();
        }

        /// <summary>
        /// Setups button from scriptable object and adds onclick listener.
        /// </summary>
        /// <param name="placed">Scriptable object to get data from.</param>
        public void SetupButton(PlacedObjectSO placed)
        {
            objectSO = placed;
            text.text = placed.objectName;
            image.sprite = placed.sprite;
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            EventManager.Trigger(new ItemToBuildSelectedEvent { PlacedObjectSo = objectSO });
        }
    }
}