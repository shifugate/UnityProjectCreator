using Assets._Scripts.Manager.Keyboard.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.Manager.Keyboard.Key
{
    public class KeyboardHoldKey : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private Image releaseImage;
        [SerializeField]
        private Image pressImage;
        [SerializeField]
        private TextMeshProUGUI keyText;

        public string Key { get { return keyText.text; } }

        private KeyboardKeyLevelModel keyboardKeyLevelModel;

        private string key;

        private Color fontReleaseColor;
        private Color fontPressColor;

        private bool press;

        private void SetSize()
        {
            rectTransform.sizeDelta = new Vector2(keyboardKeyLevelModel.width_key, keyboardKeyLevelModel.height_key);
        }

        private void SetTextures()
        {
            releaseImage.sprite = KeyboardManager.Instance.GetSprite(keyboardKeyLevelModel.release_hold_key);
            releaseImage.type = KeyboardManager.Instance.HasSpriteBorder(releaseImage.sprite) ? Image.Type.Sliced : Image.Type.Simple;
            releaseImage.gameObject.SetActive(releaseImage.sprite != null);

            pressImage.sprite = KeyboardManager.Instance.GetSprite(keyboardKeyLevelModel.press_hold_key);
            pressImage.type = KeyboardManager.Instance.HasSpriteBorder(pressImage.sprite) ? Image.Type.Sliced : Image.Type.Simple;
            pressImage.gameObject.SetActive(pressImage.sprite != null);
            pressImage.color = new Color(1, 1, 1, 0);
        }

        private void SetColors()
        {
            ColorUtility.TryParseHtmlString(keyboardKeyLevelModel.font_release_hold_color, out fontReleaseColor);
            ColorUtility.TryParseHtmlString(keyboardKeyLevelModel.font_press_hold_color, out fontPressColor);
        }

        private void SetText()
        {
            keyText.text = key;
            keyText.color = fontReleaseColor;
        }

        public KeyboardHoldKey Setup(KeyboardKeyLevelModel keyboardKeyLevelModel, string key)
        {
            this.keyboardKeyLevelModel = keyboardKeyLevelModel;
            this.key = key;

            name = "KEYBOARDMANAGER_KEY";

            SetSize();
            SetTextures();
            SetColors();
            SetText();

            return this;
        }

        public void OverKey()
        {
            if (press)
                return;

            press = true;

            keyText.color = fontPressColor;
            pressImage.color = new Color(1, 1, 1, 1);
        }

        public void OutKey()
        {
            if (!press)
                return;

            press = false;

            keyText.color = fontReleaseColor;
            pressImage.color = new Color(1, 1, 1, 0);
        }
    }
}
