using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Assets._Scripts.Manager.Popup.Modal.Base
{
    public class PopupBaseModalShow : UnityEvent<PopupBaseModal> { }
    public class PopupBaseModalHide : UnityEvent<PopupBaseModal> { }

    public class PopupBaseModal : MonoBehaviour
    {
        public PopupBaseModalShow popupBaseModalShow = new PopupBaseModalShow();
        public PopupBaseModalHide popupBaseModalHide = new PopupBaseModalHide();

        private bool showing;

        virtual protected void Awake()
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
        }

        virtual public void ShowAction()
        {
            transform.DOKill();
            transform.DOScale(1, 0.25f)
                .SetDelay(0.25f);
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
                });
        }
    }
}
