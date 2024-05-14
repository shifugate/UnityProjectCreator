using DG.Tweening;
using Assets._Scripts.Manager.Popup.Modal.Base;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Manager.Popup
{
    public class PopupManager : MonoBehaviour
    {
        #region Singleton
        private static PopupManager instance;
        public static PopupManager Instance { get { return instance; } }

        public static void InstanceNW(InitializerManager manager)
        {
            if (instance == null)
            {
                instance = GameObject.Instantiate<PopupManager>(Resources.Load<PopupManager>("Manager/Popup/PopupManager"));
                instance.name = "PopupManager";
            }

            instance.Initialize(manager);
        }
        #endregion 

        [SerializeField] private CanvasGroup overlayCanvasGroup;
        [SerializeField] private RectTransform popupHolder;
        [SerializeField] private RectTransform overHolder;
        [SerializeField] private RectTransform underHolder;

        private Dictionary<string, Transform[]> oldOverTransform = new Dictionary<string, Transform[]>();
        private Dictionary<string, Transform[]> oldUnderTransform = new Dictionary<string, Transform[]>();

        private List<PopupBaseModal> modals = new List<PopupBaseModal>();
        private bool overlay;
        private bool block;

        private void Initialize(InitializerManager manager)
        {
            transform.SetParent(manager.transform);

            gameObject.SetActive(false);
        }

        public T ShowModal<T>(bool overlay = true, bool block = true)
        {
            this.overlay = overlay;
            this.block = block;

            overlayCanvasGroup.blocksRaycasts = block;

            PopupBaseModal modal = GameObject.Instantiate<PopupBaseModal>(Resources.Load<PopupBaseModal>($"Manager/Popup/Modal/{typeof(T).Name}"), popupHolder);
            modal.popupBaseModalShow.AddListener(AddModal);
            modal.popupBaseModalHide.AddListener(RemoveModal);
            modal.Show();

            return (T)Convert.ChangeType(modal, typeof(T));
        }

        public T GetOverHolder<T>(string name)
        {
            if (oldOverTransform.ContainsKey(name))
                return oldOverTransform[name][0].GetComponent<T>();

            return default(T);
        }

        public void SetOverHolder(string name, Transform transform = null)
        {
            if (oldOverTransform.ContainsKey(name))
            {
                if (oldOverTransform[name][0] != null
                    && oldOverTransform[name][1] != null)
                    oldOverTransform[name][0].SetParent(oldOverTransform[name][1], false);

                oldOverTransform.Remove(name);
            }

            if (transform == null)
                return;

            oldOverTransform.Add(name, new Transform[] { transform, transform.parent });

            transform.SetParent(overHolder, false);
        }

        public T GetUnderHolder<T>(string name)
        {
            if (oldUnderTransform.ContainsKey(name))
                return oldUnderTransform[name][0].GetComponent<T>();

            return default(T);
        }

        public void SetUnderHolder(string name, Transform transform = null)
        {
            if (oldUnderTransform.ContainsKey(name))
            {
                if (oldUnderTransform[name][0] != null
                    && oldUnderTransform[name][1] != null)
                    oldUnderTransform[name][0].SetParent(oldUnderTransform[name][1], false);

                oldUnderTransform.Remove(name);
            }

            if (transform == null)
                return;

            oldUnderTransform.Add(name, new Transform[] { transform, transform.parent });

            transform.SetParent(underHolder, false);
        }

        private void AddModal(PopupBaseModal modal)
        {
            if (modals.Count == 0)
                Show();

            modals.Add(modal);
        }

        private void RemoveModal(PopupBaseModal modal)
        {
            modals.Remove(modal);

            if (modals.Count == 0)
                Hide();
        }

        private void Show()
        {
            overlayCanvasGroup.DOKill();
            overlayCanvasGroup.DOFade(overlay ? 1 : 0, 0.25f)
                .OnStart(() => gameObject.SetActive(true));
        }

        private void Hide()
        {
            overlayCanvasGroup.DOKill();
            overlayCanvasGroup.DOFade(0, 0.25f)
                .SetDelay(0.25f)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}
