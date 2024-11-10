using Assets._Scripts.Util;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Assets._Scripts.Manager.Popup.Modal.Base
{
    public class PopupBaseModalShow : UnityEvent<PopupBaseModal> { }
    public class PopupBaseModalHide : UnityEvent<PopupBaseModal> { }

    public class PopupBaseModal : MonoBehaviour
    {
        protected CanvasGroup canvasGroup;
        public CanvasGroup CanvasGroup { get { return canvasGroup; } }

        public GameObject firstSelected;

        public PopupBaseModalShow popupBaseModalShow = new PopupBaseModalShow();
        public PopupBaseModalHide popupBaseModalHide = new PopupBaseModalHide();

        private bool showing;
        private bool interactive;

        private GameObject lastSelected;

        virtual protected void Update()
        {
            if (!canvasGroup.interactable)
            {
                interactive = false;

                return;
            }

            if (!interactive && canvasGroup.interactable)
            {
                interactive = true;

                if (lastSelected != null)
                    EventUtil.Selected(lastSelected);
            }

            lastSelected = EventUtil.CurrentSelected;
        }

        virtual public void Initialize()
        {
            SetCanvasGroup();
            SetScale();
        }

        virtual public void SetCanvasGroup()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        virtual public void SetScale()
        {
            transform.localScale = Vector3.zero;
        }

        public void Show()
        {
            if (!showing)
                popupBaseModalShow?.Invoke(this);

            if (showing)
                return;

            showing = true;

            ShowAction();

            EventUtil.Selected(firstSelected);
        }

        virtual public void ShowAction()
        {
            transform.DOKill();
            transform.DOScale(1, 0.25f)
                .SetUpdate(true);
        }

        public void Hide()
        {
            if (!showing)
                return;

            showing = false;

            HideAction();
        }

        virtual public void HideAction()
        {
            transform.DOKill();
            transform.DOScale(0, 0.25f)
                .OnComplete(() =>
                {
                    popupBaseModalHide?.Invoke(this);

                    Destroy(gameObject);
                })
                .SetUpdate(true);
        }
    }
}
