using UnityEngine;
using UnityEngine.Events;

    public class GameObjectGameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public GameObjectGameEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public ItemSelected Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(GameObject gameObject)
        {
            Response?.Invoke(gameObject);
        }
    }

[System.Serializable]
public class ItemSelected : UnityEvent<GameObject>
{
}