using Assets._Scripts.Manager.Popup.Modal.Base;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Assets._Scripts.Manager.Popup.Modal
{
    public class PopupMessage1Modal : PopupBaseModal
    {
        [SerializeField]
        private TextMeshProUGUI titleText;
        [SerializeField]
        private TextMeshProUGUI messageText;
        [SerializeField]
        private TextMeshProUGUI okText;
        [SerializeField]
        private ScrollRect scrollView;

        private Action closeCallback;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);

            scrollView.verticalNormalizedPosition = 1;
        }

        public PopupMessage1Modal Setup(string title, string message, string ok, Action closeCallback = null)
        {
            titleText.text = title != null ? title : "";
            messageText.text = message != null ? message : "";
            okText.text = ok != null ? ok : "";

            if (title == null)
                titleText.gameObject.SetActive(false);

            if (message == null)
                messageText.gameObject.SetActive(false);

            this.closeCallback = closeCallback;

            return this;
        }

        public void CloseAction()
        {
            Hide();

            closeCallback?.Invoke();
        }
    }
}
