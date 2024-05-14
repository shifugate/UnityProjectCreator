using UnityEngine;

namespace Assets._Scripts.Manager.Language.Map
{
    public class LanguageManagerExtendedMap : MonoBehaviour
    {
        [HideInInspector]
        public string languageName;
        [HideInInspector]
        public string language;

        [HideInInspector]
        public string groupName;
        [HideInInspector]
        public string group = "common";

        [HideInInspector] 
        public string keyName;
        [HideInInspector] 
        public string key;

        public bool upper;

        private void Awake()
        {
            if (LanguageManager.Instance != null)
                LanguageManager.Instance.AddLanguageExtendedMap(this);
        }

        private void OnDestroy()
        {
            if (LanguageManager.Instance != null)
                LanguageManager.Instance.RemoveLanguageExtendedMap(this);
        }
    }
}
