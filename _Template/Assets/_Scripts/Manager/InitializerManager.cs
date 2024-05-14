using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Assets._Scripts.Manager.Language;
using Assets._Scripts.Manager.Route;
using Assets._Scripts.Manager.Setting;
using Assets._Scripts.Manager.System;
using Assets._Scripts.Manager.Keyboard;
using Assets._Scripts.Manager.Timeout;
using Assets._Scripts.Manager.Popup;

namespace Assets._Scripts.Manager
{
    public class InitializerManagerComplete : UnityEvent { }

    public class InitializerManager : MonoBehaviour
    {
        #region Singleton
        private static bool initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitialize()
        {
            if (initialized)
                return;

            initialized = true;

            Instance.Initialize();
        }

        private static InitializerManager instance;
        public static InitializerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("InitializerManager");

                    instance = go.AddComponent<InitializerManager>();

                    DontDestroyOnLoad(go);
                }

                return instance;
            }
        }
        #endregion

        private static bool initializeComplete;
        public static bool InitializeComplete { get { return initializeComplete; } }

        public object ScreenManager { get; private set; }

        private void Initialize()
        {
            InstanceNW();
            StartCoroutine(InitializeCR());
        }

        private void InstanceNW()
        {
            SettingManager.InstanceNW(this);
            PopupManager.InstanceNW(this);
            LanguageManager.InstanceNW(this);
            SystemManager.InstanceNW(this);
            KeyboardManager.InstanceNW(this);
            TimeoutManager.InstanceNW(this);
        }

        private IEnumerator InitializeCR()
        {
            initializeComplete = false;

            yield return RouteManager.InstanceCR(this);

            initializeComplete = true;
        }
    }
}