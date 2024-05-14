using UnityEngine;
using Assets._Scripts.Manager.Setting;
using Assets._Scripts.Manager.Timeout.Event;

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
                GameObject go = new GameObject("TimeoutManager");

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
            if (Input.anyKey || Input.touchCount > 0)
                time = 0;
        }

        private void UpdateTime()
        {
            time += Time.deltaTime;

            if (time > SettingManager.Instance.Data.timeout)
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
