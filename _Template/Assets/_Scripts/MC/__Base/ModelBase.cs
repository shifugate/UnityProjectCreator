using Assets._Scripts.Util;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets._Scripts.MC.__Base
{
    public class ModelBase : MonoBehaviour
    {
        public object[] args;

        [SerializeField]
        private List<Selectable> firstSelectables = new List<Selectable>();

        private CanvasGroup canvasGroup;

        private bool interactable;

        private IEnumerator Start()
        {
            yield return ContentUtil.WaitInitializerManager();
            yield return new WaitForEndOfFrame();

            SetGroup();
        }

        private void Update()
        {
            UpdateInteractive();
        }

        private void SetGroup()
        {
            canvasGroup = gameObject.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        private void UpdateInteractive()
        {
            if (canvasGroup == null)
                return;

            canvasGroup.interactable = Time.timeScale > 0;

            if (interactable != canvasGroup.interactable)
            {
                interactable = canvasGroup.interactable;

                if (!interactable)
                    return;

                foreach (Selectable selectable in firstSelectables)
                {
                    if (selectable.interactable)
                    {
                        EventUtil.Selected(selectable.gameObject);

                        return;
                    }
                }
            }
        }
    }
}
