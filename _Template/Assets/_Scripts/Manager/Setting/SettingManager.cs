using Newtonsoft.Json;
using UnityEngine;
using Assets._Scripts.Manager.Setting.Model;
using System.IO;

namespace Assets._Scripts.Manager.Setting
{
    public class SettingManager : MonoBehaviour
    {
        #region Singleton
        private static SettingManager instance;
        public static SettingManager Instance { get { return instance; } }

        public static void InstanceNW(InitializerManager manager)
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SettingManager");

                instance = go.AddComponent<SettingManager>();
                instance.Initialize(manager);
            }
        }
        #endregion

        private SettingModel data;
        public SettingModel Data { get { return data; } }

        private void Initialize(InitializerManager manager)
        {
            transform.SetParent(manager.transform);

            SetProperties();
        }

        private void SetProperties()
        {

#if UNITY_STANDALONE
            data = JsonConvert.DeserializeObject<SettingModel>(File.ReadAllText($"{Application.streamingAssetsPath}/Manager/Setting/setting.json"));
#elif UNITY_ANDROID || UNITY_IOS
            Directory.CreateDirectory($"{Application.persistentDataPath}/Manager/Setting/");

            if (!File.Exists($"{Application.persistentDataPath}/Manager/Setting/setting.json"))
                File.WriteAllText($"{Application.persistentDataPath}/Manager/Setting/setting.json", Resources.Load<TextAsset>("Manager/Setting/setting").text);

            data = JsonConvert.DeserializeObject<SettingModel>(File.ReadAllText($"{Application.persistentDataPath}/Manager/Setting/setting.json"));
#endif
        }

        public void Save()
        {
#if UNITY_STANDALONE
            File.WriteAllText($"{Application.streamingAssetsPath}/Manager/Setting/setting.json", JsonConvert.SerializeObject(data, Formatting.Indented));
#elif UNITY_ANDROID || UNITY_IOS
            File.WriteAllText($"{Application.persistentDataPath}/Manager/Setting/setting.json", JsonConvert.SerializeObject(data, Formatting.Indented));
#endif
        }

        public void Reload()
        {
            SetProperties();
        }
    }
}
