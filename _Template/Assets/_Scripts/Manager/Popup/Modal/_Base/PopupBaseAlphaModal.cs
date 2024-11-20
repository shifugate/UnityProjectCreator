using DG.Tweening;

namespace Assets._Scripts.Manager.Popup.Modal.Base
{
    public class PopupBaseAlphaModal : PopupBaseModal
    {
        override public void Initialize()
        {
            SetCanvasGroup();

            canvasGroup.alpha = 0;
        }

        override public void ShowAction()
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(1, 0.25f)
                .SetUpdate(true);
        }

        override public void HideAction()
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(0, 0.25f)
                .OnComplete(() =>
                {
                    popupBaseModalHide?.Invoke(this);

                    Destroy(gameObject);
                })
                .SetUpdate(true);
        }
    }
}
