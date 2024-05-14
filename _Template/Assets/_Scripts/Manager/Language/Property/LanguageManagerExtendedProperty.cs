#if UNITY_EDITOR
using Assets._Scripts.Manager.Language.Map;
using Assets._Scripts.Manager.Language.Token;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Assets._Scripts.Manager.Language.Attributes;

namespace Assets._Scripts.Manager.Language.Property
{
    [CustomEditor(typeof(LanguageManagerExtendedMap))]
    public class LanguageManagerExtendedProperty : Editor
    {
        private readonly Type[] _groups = typeof(LanguageManagerToken).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

        private int _languageIndex = -1;
        private int _groupIndex = -1;
        private int _keyIndex = -1;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            bool resetKey = false;
            bool resetDirty = false;

            LanguageManagerExtendedMap script = (LanguageManagerExtendedMap)target;

            List<string> groups = new List<string>();

            foreach (Type group in _groups)
                groups.Add(group.Name);

            string[] languageArr = Enum.GetNames(typeof(LanguageManager.CountryCode));
            string[] groupArr = groups.ToArray();

            if (_languageIndex < 0)
                _languageIndex = Mathf.Max(Array.FindIndex(languageArr, type => string.Equals(type, script.language)), 0);

            _languageIndex = EditorGUILayout.Popup("Language", _languageIndex, languageArr);

            if (_groupIndex < 0)
                _groupIndex = Mathf.Max(Array.FindIndex(groupArr, type => string.Equals(type, script.group)), 0);

            _groupIndex = EditorGUILayout.Popup("Group", _groupIndex, groupArr);

            resetKey = script.languageName != languageArr[_languageIndex] || script.groupName != groupArr[_groupIndex];

            if (script.languageName != languageArr[_languageIndex] || script.groupName != groupArr[_groupIndex])
                resetDirty = true;

            script.languageName = languageArr[_languageIndex];
            script.groupName = groupArr[_groupIndex];

            script.language = script.languageName;
            script.group = script.groupName;

            PropertyInfo[] _keys = _groups[_groupIndex].GetProperties(BindingFlags.Public | BindingFlags.Static);

            List<string> keys = new List<string>();

            foreach (PropertyInfo key in _keys)
                if (key.GetCustomAttribute(typeof(LanguageManagerIgnoreToken), true) == null)
                    keys.Add(key.Name);

            string[] keyArr = keys.ToArray();

            if (resetKey)
                _keyIndex = -1;

            if (_keyIndex < 0)
                _keyIndex = Mathf.Max(Array.FindIndex(keyArr, type => string.Equals(type, script.key)), 0);

            _keyIndex = EditorGUILayout.Popup("Key", _keyIndex, keyArr);

            if (script.keyName != keyArr[_keyIndex])
                resetDirty = true;

            script.keyName = keyArr[_keyIndex];
            script.key = script.keyName;

            if (resetDirty)
                EditorUtility.SetDirty(script);
        }
    }
}
#endif