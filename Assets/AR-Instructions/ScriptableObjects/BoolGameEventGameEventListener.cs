using UnityEngine;
using UnityEngine.Events;

    public class BoolGameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public BoolGameEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public BooleanUnityEvent Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(bool boolean)
        {
            Response?.Invoke(boolean);
        }
    }

[System.Serializable]
public class BooleanUnityEvent : UnityEvent<bool>
{
}
