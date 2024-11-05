using Assets._Scripts.Util;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.Manager.Event
{
    public class EventManager : MonoBehaviour
    {
        #region Singleton
        private static EventManager instance;
        public static EventManager Instance { get { return instance; } }

        public static void InstanceNW(InitializerManager manager)
        {
            if (instance == null)
            {
                GameObject go = new GameObject("EventManager");

                instance = go.AddComponent<EventManager>();
            }

            instance.Initialize(manager);
        }
        #endregion

        private void Initialize(InitializerManager manager)
        {
            transform.SetParent(manager.transform);

            Verify();
        }

        private void Verify()
        {
            StartCoroutine(VerifyCR());
        }

        private IEnumerator VerifyCR()
        {
            while (true)
            {
                if (EventUtil.CurrentSelected == null)
                {
                    Selectable selectable = FindObjectsByType<Selectable>(FindObjectsSortMode.None)?.FirstOrDefault(x => x.interactable);

                    if (selectable != null)
                        EventUtil.Selected(selectable.gameObject);
                }

                yield return new WaitForSecondsRealtime(1);
            }
        }
    }
}
