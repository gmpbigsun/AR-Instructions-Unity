using UnityEngine;
using UnityEngine.Events;

    public class MaterialGameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public MaterialGameEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public ColorSelected Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(Material material)
        {
            Response?.Invoke(material);
        }
    }

[System.Serializable]
public class ColorSelected : UnityEvent<Material>
{
}