using Assets._Scripts.Manager.Popup.Modal.Base;
using Assets._Scripts.Util;
using DG.Tweening;
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

        private readonly Dictionary<string, Transform[]> oldOverTransform = new();
        private readonly Dictionary<string, Transform[]> oldUnderTransform = new();

        private readonly List<PopupBaseModal> modals = new();
        private bool overlay;

        private void Initialize(InitializerManager manager)
        {
            transform.SetParent(manager.transform);

            gameObject.SetActive(false);
        }

        public T ShowModal<T>(bool overlay = true, bool block = true)
        {
            this.overlay = overlay;

            overlayCanvasGroup.blocksRaycasts = block;

            PopupBaseModal modal = Instantiate(Resources.Load<PopupBaseModal>($"Manager/Popup/Modal/{typeof(T).Name}"), popupHolder);
            modal.popupBaseModalShow.AddListener(AddModal);
            modal.popupBaseModalHide.AddListener(RemoveModal);
            modal.Initialize();
            modal.Show();

            return (T)Convert.ChangeType(modal, typeof(T));
        }

        public T GetOverHolder<T>(string name)
        {
            if (oldOverTransform.ContainsKey(name))
                return oldOverTransform[name][0].GetComponent<T>();

            return default;
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
#pragma warning disable UNT0014 // Invalid type for call to GetComponent
                return oldUnderTransform[name][0].GetComponent<T>();
#pragma warning restore UNT0014 // Invalid type for call to GetComponent

            return default;
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

            if (modals.Count > 0)
                modals[^1].CanvasGroup.interactable = false;

            modals.Add(modal);
        }

        private void RemoveModal(PopupBaseModal modal)
        {
            modals.Remove(modal);

            if (modals.Count == 0)
                Hide();
            else
                modals[^1].CanvasGroup.interactable = true;
        }

        private void Show()
        {
            overlayCanvasGroup.DOKill();
            overlayCanvasGroup.DOFade(overlay ? 1 : 0, 0.25f)
                .OnStart(() =>
                {
                    SystemUtil.PauseGame();

                    gameObject.SetActive(true);
                })
                .SetUpdate(true);
        }

        private void Hide()
        {
            overlayCanvasGroup.DOKill();
            overlayCanvasGroup.DOFade(0, 0.25f)
                .SetDelay(0.25f)
                .OnComplete(() =>
                {
                    SystemUtil.ResumeGame();

                    gameObject.SetActive(false);
                })
                .SetUpdate(true);
        }
    }
}
