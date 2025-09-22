using Assets._Scripts.Manager.Setting.Model;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

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

        private SettingModel model;
        public SettingModel Model { get { return model; } }

        private void Initialize(InitializerManager manager)
        {
            transform.SetParent(manager.transform);

            SetProperties();
        }

        private void SetProperties()
        {

#if UNITY_STANDALONE
            model = JsonConvert.DeserializeObject<SettingModel>(File.ReadAllText($"{Application.streamingAssetsPath}/Manager/Setting/setting.json"));
#elif UNITY_ANDROID || UNITY_IOS
            model = JsonConvert.DeserializeObject<SettingModel>(Resources.Load<TextAsset>("Manager/Setting/setting").text);
#endif
        }
    }
}
