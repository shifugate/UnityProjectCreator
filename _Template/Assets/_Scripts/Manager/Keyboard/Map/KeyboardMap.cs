using Assets._Scripts.Manager.Keyboard.Data;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets._Scripts.Manager.Keyboard.Map
{
    public class KeyboardMap : MonoBehaviour
    {
        [SerializeField]
        private KeyboardManager.Type type;
        [SerializeField]
        private int tabIndex;

        private TMP_InputField inputField;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => InitializerManager.InitializeComplete);

            SetProperties();
        }

        private void SetProperties()
        {
            inputField = GetComponentInChildren<TMP_InputField>();

            KeyboardManager.Instance.AddInputField(new KeyboardData 
            {
                type = type,
                inputField = inputField,
                tabIndex = tabIndex
            });
        }
    }
}
