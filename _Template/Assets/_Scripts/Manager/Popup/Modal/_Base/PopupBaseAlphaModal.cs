using DG.Tweening;
using UnityEngine;

namespace Assets._Scripts.Manager.Popup.Modal.Base
{
    public class PopupBaseAlphaModal : PopupBaseModal
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        override protected void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;       
        }

        override public void ShowAction()
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(1, 0.25f)
                .SetDelay(0.25f);
        }

        override public void HideAction()
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(0, 0.25f)
                .OnComplete(() => {
                    popupBaseModalHide?.Invoke(this);

                    Destroy(gameObject);
                });
        }
    }
}
