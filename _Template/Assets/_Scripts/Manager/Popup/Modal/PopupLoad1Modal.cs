using TMPro;
using UnityEngine;
using Assets._Scripts.Manager.Popup.Modal.Base;

namespace Assets._Scripts.Manager.Popup.Modal
{
    public class PopupLoad1Modal : PopupBaseAlphaModal
    {
        [SerializeField]
        private TextMeshProUGUI loadText;


        public PopupLoad1Modal Setup(string message = null)
        {
            loadText.text = message != null ? message : "";

            return this;
        }
    }
}
