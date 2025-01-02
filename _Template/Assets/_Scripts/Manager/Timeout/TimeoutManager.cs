using Assets._Scripts.Manager.Setting;
using Assets._Scripts.Manager.Timeout.Event;
using UnityEngine;

namespace Assets._Scripts.Manager.Timeout
{
    public class TimeoutManager : MonoBehaviour
    {
        #region Singleton
        private static TimeoutManager instance;
        public static TimeoutManager Instance { get { return instance; } }

        public static void InstanceNW(InitializerManager manager)
        {
            if (instance == null)
            {
                GameObject go = new("TimeoutManager");

                instance = go.AddComponent<TimeoutManager>();
            }

            instance.Initialize(manager);
        }
        #endregion

        private float time;

        private void Update()
        {
            VerifyInput();
            UpdateTime();
        }

        private void Initialize(InitializerManager manager)
        {
            transform.SetParent(manager.transform);

            SetProperties();
        }

        private void SetProperties()
        {
            ResetTime();
        }

        private void VerifyInput()
        {
           if ((UnityEngine.InputSystem.Keyboard.current?.anyKey != null && UnityEngine.InputSystem.Keyboard.current.anyKey.wasPressedThisFrame) 
			|| (UnityEngine.InputSystem.Mouse.current?.leftButton != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
			|| UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
			time = 0;
        }

        private void UpdateTime()
        {
            time += Time.deltaTime;

            if (time > SettingManager.Instance.Model.timeout)
            {
                ResetTime();

                TimeoutManagerEvent.OnTimeout?.Invoke();
            }
        }

        public void ResetTime()
        {
            time = 0;
        }
    }
}
