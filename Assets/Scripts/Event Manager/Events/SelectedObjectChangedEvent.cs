using Gameplay.Objects;

namespace Event_Manager.Events
{
    public class SelectedObjectChangedEvent : GameEvent
    {
        public PlacedObject selectedObject;
    }
}
