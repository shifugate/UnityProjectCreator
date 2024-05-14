using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Assets._Scripts.Util
{
    public static class ScreenUtil
    {
        public static bool PointerOverUI()
        {
            GameObject go = PointerOverUIElement(GetEventSystemRaycastResults());

            return go != null;
        }

        public static bool PointerOverUIName(string name)
        {
            List<RaycastResult> raycastResults = GetEventSystemRaycastResults();

            foreach (RaycastResult raycastResult in raycastResults)
                if (raycastResult.gameObject.name == name)
                    return true;

            return false;
        }

        public static GameObject GetUIOverPointerByName(string name)
        {
            List<RaycastResult> raycastResults = GetEventSystemRaycastResults();

            foreach (RaycastResult raycastResult in raycastResults)
                if (raycastResult.gameObject.name == name)
                    return raycastResult.gameObject;

            return null;
        }

        public static List<GameObject> GetUIsOverPointer()
        {
            List<GameObject> list = new List<GameObject>();
            List<RaycastResult> raycastResults = GetEventSystemRaycastResults();

            foreach (RaycastResult raycastResult in raycastResults)
                if (!list.Contains(raycastResult.gameObject))
                    list.Add(raycastResult.gameObject);

            return list;
        }

        private static GameObject PointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            if (eventSystemRaysastResults.Count > 0)
                return eventSystemRaysastResults[0].gameObject;

            return null;
        }

        private static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
}
