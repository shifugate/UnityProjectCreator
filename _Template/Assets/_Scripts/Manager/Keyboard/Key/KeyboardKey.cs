using Assets._Scripts.Manager.Keyboard.Model;
using Assets._Scripts.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.Manager.Keyboard.Key
{
    public class KeyboardKey : MonoBehaviour
    {
        [SerializeField]
        private KeyboardHoldKey keyboardHoldKey;
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private RectTransform holdHolder;
        [SerializeField]
        private Image releaseImage;
        [SerializeField]
        private Image pressImage;
        [SerializeField]
        private Image lockImage;
        [SerializeField]
        private TextMeshProUGUI keyText;

        private RectTransform keyboardTransform;

        public string Key { get { return keyText.text; } }

        private KeyboardKeyModel KeyboardKeyModel;

        private List<string> holdList;

        private KeyboardHoldKey holdKey;

        private Color fontReleaseColor;
        private Color fontPressColor;
        private Color fontLockColor;

        private bool press;

        private float time;
        private float shiftTime;

        private int shitClicks;

        private Vector2 position;

        private void Awake()
        {
            SetProperties();
            AddListener();
        }

        private void OnDestroy()
        {
            RemoveListener();
        }

        private void Update()
        {
            UpdateInput();
            UpdateHolder();
        }

        private void SetProperties()
        {
            keyboardTransform = (RectTransform)transform.parent.parent;
        }

        private void AddListener()
        {
            KeyboardManager.Instance.onKeyboardManagerUpdateKey.AddListener(OnKeyboardManagerUpdateState);
        }

        private void RemoveListener()
        {
            KeyboardManager.Instance.onKeyboardManagerUpdateKey.RemoveListener(OnKeyboardManagerUpdateState);
        }

        private void OnKeyboardManagerUpdateState()
        {
            UpdateSate();
            SetText();
        }

        private void UpdateInput()
        {
            shiftTime += Time.deltaTime;

            if (shiftTime > KeyboardKeyModel.click_time)
            {
                shiftTime = 0;
                shitClicks = 0;
            }

            if (!press)
            {
                if (!holdHolder.parent.Equals(transform))
                {
                    holdHolder.SetParent(transform, true);
                    holdHolder.anchoredPosition = Vector3.zero;
                }

                foreach (Transform transform in holdHolder)
                    Destroy(transform.gameObject);

                return;
            }

            time += Time.deltaTime;

            if (time <= KeyboardKeyModel.click_time)
                return;

            holdList = KeyboardManager.Instance.Shifted || KeyboardManager.Instance.ShiftedLocked ? KeyboardKeyModel.shifted_hold : KeyboardKeyModel.normal_hold;

            if (holdList != null && holdList.Count > 0 && holdHolder.childCount == 0)
                foreach (string key in holdList)
                    Instantiate(keyboardHoldKey, holdHolder).Setup(KeyboardKeyModel, key);

            if (!holdHolder.parent.Equals(keyboardTransform))
            {
                holdHolder.SetParent(keyboardTransform, true);

                position = holdHolder.anchoredPosition;
            }

            GameObject keyObject = ScreenUtil.GetUIOverPointerByName("KEYBOARDMANAGER_KEY");

            if (keyObject != null)
            {
                KeyboardHoldKey holdKey = keyObject.GetComponent<KeyboardHoldKey>();

                if (this.holdKey != null && this.holdKey != holdKey)
                    this.holdKey.OutKey();

                if (holdKey != null && this.holdKey != holdKey)
                    holdKey.OverKey();

                this.holdKey = holdKey;
            }
            else if (holdKey != null)
            {
                holdKey.OutKey();
            }
        }

        private void UpdateHolder()
        {
            if (KeyboardKeyModel == null || !holdHolder.parent.Equals(keyboardTransform))
                return;

            float x = position.x;
            float y = position.y + KeyboardKeyModel.height_key;

            if (x - holdHolder.rect.width / 2 < -keyboardTransform.rect.width / 2)
                x = -keyboardTransform.rect.width / 2 + holdHolder.rect.width / 2;
            else if (x + holdHolder.rect.width / 2 > keyboardTransform.rect.width / 2)
                x = keyboardTransform.rect.width / 2 - holdHolder.rect.width / 2;

            if (y - holdHolder.rect.height / 2 < -keyboardTransform.rect.height / 2)
                y = -keyboardTransform.rect.height / 2 + holdHolder.rect.height / 2;
            else if (y + holdHolder.rect.height / 2 > keyboardTransform.rect.height / 2)
                y = keyboardTransform.rect.height / 2 - holdHolder.rect.height / 2;

            holdHolder.anchoredPosition = new Vector2(x, y);
        }

        private void UpdateSate()
        {
            if (KeyboardManager.Instance.Shifted && KeyboardKeyModel.shift)
            {
                keyText.color = fontPressColor;
                pressImage.color = new Color(1, 1, 1, 1);
            }
            else if (KeyboardManager.Instance.ShiftedLocked && KeyboardKeyModel.shift)
            {
                keyText.color = fontLockColor;
                pressImage.color = new Color(1, 1, 1, 0);
                lockImage.color = new Color(1, 1, 1, 1);
            }
            else
            {
                keyText.color = fontReleaseColor;
                pressImage.color = new Color(1, 1, 1, 0);
                lockImage.color = new Color(1, 1, 1, 0);
            }
        }

        private void SetSize()
        {
            rectTransform.sizeDelta = new Vector2(KeyboardKeyModel.width_key, KeyboardKeyModel.height_key);
        }

        private void SetTextures()
        {
            releaseImage.sprite = KeyboardManager.Instance.GetSprite(KeyboardKeyModel.release_key);
            releaseImage.type = KeyboardManager.Instance.HasSpriteBorder(releaseImage.sprite) ? Image.Type.Sliced : Image.Type.Simple;
            releaseImage.gameObject.SetActive(releaseImage.sprite != null);

            pressImage.sprite = KeyboardManager.Instance.GetSprite(KeyboardKeyModel.press_key);
            pressImage.type = KeyboardManager.Instance.HasSpriteBorder(pressImage.sprite) ? Image.Type.Sliced : Image.Type.Simple;
            pressImage.gameObject.SetActive(pressImage.sprite != null);
            pressImage.color = new Color(1, 1, 1, 0);

            lockImage.sprite = KeyboardManager.Instance.GetSprite(KeyboardKeyModel.lock_key);
            lockImage.type = KeyboardManager.Instance.HasSpriteBorder(lockImage.sprite) ? Image.Type.Sliced : Image.Type.Simple;
            lockImage.gameObject.SetActive(lockImage.sprite != null);
            lockImage.color = new Color(1, 1, 1, 0);
        }

        private void SetColors()
        {
            ColorUtility.TryParseHtmlString(KeyboardKeyModel.font_release_color, out fontReleaseColor);
            ColorUtility.TryParseHtmlString(KeyboardKeyModel.font_press_color, out fontPressColor);
            ColorUtility.TryParseHtmlString(KeyboardKeyModel.font_lock_color, out fontLockColor);
        }

        private void SetText()
        {
            string text = KeyboardManager.Instance.Shifted || KeyboardManager.Instance.ShiftedLocked ? KeyboardKeyModel.shifted : KeyboardKeyModel.normal;

            if (!string.IsNullOrEmpty(text) && !keyText.font.HasCharacters(text, out uint[] missing, false, true))
            {
                SystemUtil.Log(GetType(), $"Character not found in TMP Font Asset: {keyText.font.name} => {text} => missing: {JsonConvert.SerializeObject(missing)}");

                text = "";
            }
            
            keyText.text = text;
            keyText.color = fontReleaseColor;
        }

        public KeyboardKey Setup(KeyboardKeyModel KeyboardKeyModel)
        {
            this.KeyboardKeyModel = KeyboardKeyModel;

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

            time = 0;
            shiftTime = 0;
            shitClicks++;
        }

        public void OutKey()
        {
            if (!press)
                return;

            press = false;

            if (time <= KeyboardKeyModel.click_time)
            {
                if (KeyboardKeyModel.shift && shitClicks >= 2)
                {
                    KeyboardManager.Instance.KeyClick(KeyboardManager.Key.ShiftLock);

                    shiftTime = 0;
                    shitClicks = 0;
                }
                else if (KeyboardKeyModel.shift)
                {
                    KeyboardManager.Instance.KeyClick(KeyboardManager.Key.Shift);
                }
                else if (KeyboardKeyModel.delete)
                {
                    KeyboardManager.Instance.KeyClick(KeyboardManager.Key.Delete);
                }
                else if (KeyboardKeyModel.tab)
                {
                    KeyboardManager.Instance.KeyClick(KeyboardManager.Key.Tab);
                }
                else if (KeyboardKeyModel.swap)
                {
                    KeyboardManager.Instance.KeyClick(KeyboardManager.Key.Swap, null, KeyboardKeyModel.level);
                }
                else if (KeyboardKeyModel.enter)
                {
                    KeyboardManager.Instance.KeyClick(KeyboardManager.Key.Enter);
                }
                else
                {
                    KeyboardManager.Instance.KeyClick(KeyboardManager.Key.Text, Key, null, KeyboardKeyModel);
                }
            }
            else if (holdKey != null)
            {
                KeyboardManager.Instance.KeyClick(KeyboardManager.Key.Text, holdKey.Key);
            }

            holdKey = null;

            UpdateSate();
        }
    }
}
