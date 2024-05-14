using Assets._Scripts.Manager.Popup.Modal.Base;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Assets._Scripts.Manager.Popup.Modal
{
    public class PopupAcceptRefuse1Modal : PopupBaseModal
    {
        [SerializeField]
        private TextMeshProUGUI titleText;
        [SerializeField]
        private TextMeshProUGUI messageText;
        [SerializeField]
        private TextMeshProUGUI acceptText;
        [SerializeField]
        private TextMeshProUGUI refuseText;
        [SerializeField]
        private ScrollRect scrollView;

        private Action acceptCallback;
        private Action refuseCallback;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);

            scrollView.verticalNormalizedPosition = 1;
        }

        public PopupAcceptRefuse1Modal Setup(string title, string message, string accept, string refuse, Action acceptCallback = null, Action refuseCallback = null)
        {
            titleText.text = title != null ? title : "";
            messageText.text = message != null ? message : "";
            acceptText.text = accept != null ? accept : "";
            refuseText.text = refuse != null ? refuse : "";

            if (title == null)
                titleText.gameObject.SetActive(false);

            if (message == null)
                messageText.gameObject.SetActive(false);

            this.acceptCallback = acceptCallback;
            this.refuseCallback = refuseCallback;

            return this;
        }

        public void AcceptAction()
        {
            Hide();

            acceptCallback?.Invoke();
        }

        public void RefuseAction()
        {
            Hide();

            refuseCallback?.Invoke();
        }
    }
}
