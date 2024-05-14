using Assets._Scripts.Manager.Keyboard.Key;
using Assets._Scripts.Manager.Keyboard.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.Manager.Keyboard.Row
{
    public class KeyboardRow : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rowHolder;
        [SerializeField]
        private KeyboardKey keyboardKey;
        [SerializeField]
        private HorizontalLayoutGroup horizontalLayoutGroup;

        private KeyboardRowModel keyboardRowModel;

        private void Awake()
        {
            AddListener();
        }

        private void OnDestroy()
        {
            RemoveListener();
        }

        private void AddListener()
        {
            KeyboardManager.Instance.onKeyboardManagerUpdateChange.AddListener(OnKeyboardManagerUpdateChange);
            KeyboardManager.Instance.onKeyboardManagerUpdateLevel.AddListener(OnKeyboardManagerUpdateLevel);
        }

        private void RemoveListener()
        {
            KeyboardManager.Instance.onKeyboardManagerUpdateChange.RemoveListener(OnKeyboardManagerUpdateChange);
            KeyboardManager.Instance.onKeyboardManagerUpdateLevel.RemoveListener(OnKeyboardManagerUpdateLevel);
        }

        private void OnKeyboardManagerUpdateChange()
        {
            SetContent();
        }

        private void OnKeyboardManagerUpdateLevel()
        {
            SetContent();
        }

        private void SetContent() 
        {
            SetSpace();
            SetMargin();
            SetKeys();
        }

        private void SetSpace()
        {
            horizontalLayoutGroup.spacing = keyboardRowModel.space_row;
        }

        private void SetMargin()
        {
            horizontalLayoutGroup.padding = new RectOffset(keyboardRowModel.margin_x, keyboardRowModel.margin_x, 0, 0);
        }

        private void SetKeys()
        {
            foreach (Transform transform in rowHolder)
                Destroy(transform.gameObject);

            foreach (KeyboardKeyModel keyboardKeyModel in keyboardRowModel.keys)
                Instantiate(keyboardKey, rowHolder).Setup(KeyboardManager.Instance.GetLevel(keyboardKeyModel));
        }

        public KeyboardRow Setup(KeyboardRowModel keyboardRowModel)
        {
            this.keyboardRowModel = keyboardRowModel;

            SetContent();

            return this;
        }
    }
}
