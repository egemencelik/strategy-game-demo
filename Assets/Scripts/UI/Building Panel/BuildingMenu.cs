using Gameplay.Scriptable_Objects;
using UnityEngine;

namespace UI.Building_Panel
{
    public class BuildingMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject buttonPrefab;

        [SerializeField]
        private Transform content;

        private void Awake()
        {
            // get building scriptable objects from Resources/Buildings folder
            var buildings = Resources.LoadAll<PlacedObjectSO>("Buildings");
            foreach (var building in buildings)
            {
                // create and setup buttons
                var btn = Instantiate(buttonPrefab, content);
                btn.GetComponent<BuildingButton>().SetupButton(building);
            }
        }
    }
}
