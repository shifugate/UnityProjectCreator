using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets._Scripts.Util
{
    public static class EventUtil
    {
        public static GameObject CurrentSelected { get { return EventSystem.current?.currentSelectedGameObject; } }

        public static void Selected(GameObject gameObject)
        {
            EventSystem.current?.SetSelectedGameObject(gameObject);
        }
    }
}
