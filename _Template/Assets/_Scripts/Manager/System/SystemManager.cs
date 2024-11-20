using Assets._Scripts.Manager.Setting;
using Assets._Scripts.Manager.System.Support;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Assets._Scripts.Manager.System
{
    public class SystemManager : MonoBehaviour
    {
        #region Singleton
        private static SystemManager instance;
        public static SystemManager Instance { get { return instance; } }

        public static void InstanceNW(InitializerManager manager)
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SystemManager");

                instance = go.AddComponent<SystemManager>();
            }

            instance.Initialize(manager);
        }
        #endregion

        private void Initialize(InitializerManager manager)
        {
            transform.SetParent(manager.transform);

            SetProperties();
        }

        private void SetProperties()
        {
            if (SettingManager.Instance.Model.max_fps > 0)
                Application.targetFrameRate = SettingManager.Instance.Model.max_fps;

            EnhancedTouchSupport.Enable();

            if (SettingManager.Instance.Model.fps_show)
                gameObject.AddComponent<SystemManagerFPSDisplaySupport>();

            UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (op, ex) => throw ex;
        }
    }
}
