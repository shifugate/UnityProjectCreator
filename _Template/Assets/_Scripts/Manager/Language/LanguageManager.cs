using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Assets._Scripts.Manager.Language.Map;
using Assets._Scripts.Util;
using Assets._Scripts.Manager.Language.Event;

namespace Assets._Scripts.Manager.Language
{
    public class LanguageManager : MonoBehaviour
    {
        #region Singleton
        private static LanguageManager instance;
        public static LanguageManager Instance { get { return instance; } }

        public static void InstanceNW(InitializerManager manager)
        {
            if (instance == null)
            {
                GameObject go = new GameObject("LanguageManager");

                instance = go.AddComponent<LanguageManager>();
            }

            instance.Initialize(manager);
        }
        #endregion

        public enum CountryCode { en_US, pt_BR }

        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> contents = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        private string language;
        public string Language { get { return language; } }

        private List<LanguageManagerMap> maps = new List<LanguageManagerMap>();

        private List<LanguageManagerExtendedMap> mapsExtended = new List<LanguageManagerExtendedMap>();

        private void Initialize(InitializerManager manager)
        {
            transform.SetParent(manager.transform);

            SetProperties();
        }

        private void SetProperties()
        {
#if UNITY_STANDALONE
            string[] files = Directory.GetFiles($"{Application.streamingAssetsPath}/Manager/Language", "*.json");

            contents.Clear();

            foreach (string file in files)
                contents.Add(Path.GetFileNameWithoutExtension(file), JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(file)));

            language = PlayerPrefs.HasKey("language") ? PlayerPrefs.GetString("language") : Enum.GetName(typeof(CountryCode), 0);
#elif UNITY_ANDROID || UNITY_IOS
            TextAsset[] assets = Resources.LoadAll<TextAsset>("Manager/Language");

            contents.Clear();

            foreach (TextAsset asset in assets)
                contents.Add(asset.name, JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(asset.text));

            language = PlayerPrefs.HasKey("language") ? PlayerPrefs.GetString("language") : Enum.GetName(typeof(CountryCode), 0);
#endif
        }

        public void SetLanguage(CountryCode language)
        {
            this.language = Enum.GetName(typeof(CountryCode), language);

            PlayerPrefs.SetString("language", this.language);
            PlayerPrefs.Save();

            UpdateMaps();

            LanguageManagerEvent.LanguageManagerEvent_Updated?.Invoke();
        }

        public bool HasLanguage()
        {
            return PlayerPrefs.HasKey("language");
        }

        public bool HasTranslation(string group, string key)
        {
            return contents.ContainsKey(language) && contents[language].ContainsKey(group) && contents[language][group].ContainsKey(key);
        }

        public bool HasTranslation(string language, string group, string key)
        {
            return contents.ContainsKey(language) && contents[language].ContainsKey(group) && contents[language][group].ContainsKey(key);
        }

        public string GetTranslation(string group, string key)
        {
            try
            {
                return contents[language][group][key];
            }
            catch (Exception ex)
            {
                SystemUtil.Log(GetType(), $"{language}: {group}: {key} - {ex}", SystemUtil.LogType.Warning);
            }

            return $"{language}: {group}: {key}";
        }

        public string GetTranslation(string language, string group, string key)
        {
            try
            {
                return contents[language][group][key];
            }
            catch (Exception ex)
            {
                SystemUtil.Log(GetType(), $"{language}: {group}: {key} - {ex}", SystemUtil.LogType.Warning);
            }

            return $"{language}: {group}: {key}";
        }

        public Dictionary<string, string> GetGroup(string group)
		{
			try
			{
				return contents[language][group];
			}
			catch (Exception ex)
			{
				SystemUtil.Log(GetType(), $"{language}: {group} - {ex}", SystemUtil.LogType.Warning);
			}

			return new Dictionary<string, string>();
		}

		public Dictionary<string, string> GetGroup(string language, string group)
		{
			try
			{
				return contents[language][group];
			}
			catch (Exception ex)
			{
				SystemUtil.Log(GetType(), $"{language}: {group} - {ex}", SystemUtil.LogType.Warning);
			}

			return new Dictionary<string, string>();
		}

        public void AddLanguageMap(LanguageManagerMap map)
        {
            if (!maps.Contains(map))
                maps.Add(map);

            UpdateMaps(map);
        }

        public void RemoveLanguageMap(LanguageManagerMap map)
        {
            maps.Remove(map);
        }

        private void UpdateMaps(LanguageManagerMap map = null)
        {
            if (map != null)
            {
                SetMapTranslation(map);

                return;
            }

            foreach(LanguageManagerMap obj in maps)
                SetMapTranslation(obj);
        }

        private void SetMapTranslation(LanguageManagerMap map)
        {
            try
            {
                Component[] components = map.GetComponents<Component>().Where(x => x.GetType().GetProperty("text") != null).ToArray();

                foreach (Component component in components)
                {
                    try
                    {
                        component.GetType().GetProperty("text").SetValue(component, map.upper 
                            ? GetTranslation(map.group, map.key).ToUpper() 
                            : GetTranslation(map.group, map.key));
                    }
                    catch(Exception ex)
                    {
                        SystemUtil.Log(GetType(), $"{language}: {component.gameObject.name}: {map.group}: {map.key} - {ex}", SystemUtil.LogType.Warning);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public void AddLanguageExtendedMap(LanguageManagerExtendedMap map)
        {
            if (!mapsExtended.Contains(map))
                mapsExtended.Add(map);

            UpdateExtendedMaps(map);
        }

        public void RemoveLanguageExtendedMap(LanguageManagerExtendedMap map)
        {
            mapsExtended.Remove(map);
        }

        private void UpdateExtendedMaps(LanguageManagerExtendedMap map = null)
        {
            if (map != null)
            {
                SetExtendedMapTranslation(map);

                return;
            }

            foreach (LanguageManagerExtendedMap obj in mapsExtended)
                SetExtendedMapTranslation(obj);
        }

        private void SetExtendedMapTranslation(LanguageManagerExtendedMap map)
        {
            try
            {
                Component[] components = map.GetComponents<Component>().Where(x => x.GetType().GetProperty("text") != null).ToArray();

                foreach (Component component in components)
                {
                    try
                    {
                        component.GetType().GetProperty("text").SetValue(component, map.upper 
                            ? GetTranslation(map.language, map.group, map.key).ToUpper()
                            : GetTranslation(map.language, map.group, map.key));
                    }
                    catch (Exception ex)
                    {
                        SystemUtil.Log(GetType(), $"{map.language}: {component.gameObject.name}: {map.group}: {map.key} - {ex}", SystemUtil.LogType.Warning);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
