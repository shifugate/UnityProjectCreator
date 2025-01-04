using Assets._Scripts.Manager.Keyboard.Event;
using Assets._Scripts.Manager.Keyboard.Model;
using Assets._Scripts.Manager.Keyboard.Row;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.Manager.Keyboard.Board
{
    public class KeyboardBoard : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rowHolder;
        public RectTransform RowHolder { get { return rowHolder; } }
        [SerializeField]
        private KeyboardRow keyboardRow;
        [SerializeField]
        private Image backgroundImage;
        [SerializeField]
        private VerticalLayoutGroup verticalLayoutGroup;

        private string language;
        public string Language { get { return language; } }

        private KeyboardKeyboardModel keyboardKeyboardModel;
        public KeyboardKeyboardModel KeyboardKeyboardModel { get { return keyboardKeyboardModel; } }

        private bool showing = true;

        private Coroutine showCR;
        private Coroutine hideCR;

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
            SetTextures();
            SetRows();
        }

        private void SetSpace()
        {
            verticalLayoutGroup.spacing = keyboardKeyboardModel.space_row;
        }

        private void SetMargin()
        {
            verticalLayoutGroup.padding = new RectOffset(keyboardKeyboardModel.margin_x, keyboardKeyboardModel.margin_x, keyboardKeyboardModel.margin_y, keyboardKeyboardModel.margin_y);
        }

        private void SetTextures()
        {
            backgroundImage.sprite = KeyboardManager.Instance.GetSprite(keyboardKeyboardModel.background);
            backgroundImage.type = KeyboardManager.Instance.HasSpriteBorder(backgroundImage.sprite) ? Image.Type.Sliced : Image.Type.Simple;
            backgroundImage.gameObject.SetActive(backgroundImage.sprite != null);
        }

        private void SetRows()
        {
            foreach (Transform transform in rowHolder)
                Destroy(transform.gameObject);

            if (KeyboardManager.Instance.Level >= keyboardKeyboardModel.levels.Count)
                return;

            KeyboardLevelModel levelModel = keyboardKeyboardModel.levels[KeyboardManager.Instance.Level];

            foreach (KeyboardRowModel keyboardRowModel in levelModel.rows)
                Instantiate(keyboardRow, rowHolder).Setup(keyboardRowModel);
        }

        private void SetStart(bool now = false)
        {
            if (!showing)
                return;

            showing = false;

            KeyboardManager.Direction direction = (KeyboardManager.Direction)keyboardKeyboardModel.start_at;

            switch (direction)
            {
                case KeyboardManager.Direction.Left:
                    SetStartLeft(now);
                    break;
                case KeyboardManager.Direction.Top:
                    SetStartTop(now);
                    break;
                case KeyboardManager.Direction.Right:
                    SetStartRight(now);
                    break;
                case KeyboardManager.Direction.Bottom:
                    SetStartBottom(now);
                    break;
            }
        }

        private void SetShow(Action completeCallback)
        {
            if (showing)
                return;

            showing = true;

            KeyboardManager.Direction direction = (KeyboardManager.Direction)keyboardKeyboardModel.show_at;

            switch (direction)
            {
                case KeyboardManager.Direction.Left:
                    SetShowLeft();
                    break;
                case KeyboardManager.Direction.Top:
                    SetShowTop();
                    break;
                case KeyboardManager.Direction.Right:
                    SetShowRight();
                    break;
                case KeyboardManager.Direction.Bottom:
                    SetShowBottom();
                    break;
            }

            completeCallback?.Invoke();
        }

        private void SetStartLeft(bool now = false)
        {
            rowHolder.DOKill();
            rowHolder.DOAnchorPos(new Vector2(-Screen.width / KeyboardManager.Instance.Scale.x, 0), 0.25f)
                .SetDelay(now ? 0 : keyboardKeyboardModel.hide_delay)
                .OnUpdate(() => KeyboardManagerEvent.OnHideUpdate?.Invoke(this))
                .OnComplete(() =>
                {
                    rowHolder.gameObject.SetActive(false);

                    KeyboardManagerEvent.OnHideComplete?.Invoke(this);
                });
        }

        private void SetStartTop(bool now = false)
        {
            rowHolder.DOKill();
            rowHolder.DOAnchorPos(new Vector2(0, Screen.height / KeyboardManager.Instance.Scale.y), 0.25f)
                .SetDelay(now ? 0 : keyboardKeyboardModel.hide_delay)
                .OnUpdate(() => KeyboardManagerEvent.OnHideUpdate?.Invoke(this))
                .OnComplete(() =>
                {
                    rowHolder.gameObject.SetActive(false);

                    KeyboardManagerEvent.OnHideComplete?.Invoke(this);
                });
        }

        private void SetStartRight(bool now = false)
        {
            rowHolder.DOKill();
            rowHolder.DOAnchorPos(new Vector2(Screen.width / KeyboardManager.Instance.Scale.x, 0), 0.25f)
                .SetDelay(now ? 0 : keyboardKeyboardModel.hide_delay)
                .OnUpdate(() => KeyboardManagerEvent.OnHideUpdate?.Invoke(this))
                .OnComplete(() =>
                {
                    rowHolder.gameObject.SetActive(false);

                    KeyboardManagerEvent.OnHideComplete?.Invoke(this);
                });
        }

        private void SetStartBottom(bool now = false)
        {
            rowHolder.DOKill();
            rowHolder.DOAnchorPos(new Vector2(0, -Screen.height / KeyboardManager.Instance.Scale.y), 0.25f)
                .SetDelay(now ? 0 : keyboardKeyboardModel.hide_delay)
                .OnUpdate(() => KeyboardManagerEvent.OnHideUpdate?.Invoke(this))
                .OnComplete(() =>
                {
                    rowHolder.gameObject.SetActive(false);

                    KeyboardManagerEvent.OnHideComplete?.Invoke(this);
                });
        }

        private void SetShowLeft()
        {
            rowHolder.gameObject.SetActive(true);
            rowHolder.DOKill();
            rowHolder.DOAnchorPos(new Vector2(-(Screen.width / KeyboardManager.Instance.Scale.x) / 2 + rowHolder.rect.width / 2 + keyboardKeyboardModel.show_margin, 0), 0.25f)
                .OnUpdate(() => KeyboardManagerEvent.OnShowUpdate?.Invoke(this))
                .OnComplete(() => KeyboardManagerEvent.OnShowComplete?.Invoke(this));
        }

        private void SetShowTop()
        {
            rowHolder.gameObject.SetActive(true);
            rowHolder.DOKill();
            rowHolder.DOAnchorPos(new Vector2(0, (Screen.height / KeyboardManager.Instance.Scale.y) / 2 - rowHolder.rect.height / 2 - keyboardKeyboardModel.show_margin), 0.25f)
                .OnUpdate(() => KeyboardManagerEvent.OnShowUpdate?.Invoke(this))
                .OnComplete(() => KeyboardManagerEvent.OnShowComplete?.Invoke(this));
        }

        private void SetShowRight()
        {
            rowHolder.gameObject.SetActive(true);
            rowHolder.DOKill();
            rowHolder.DOAnchorPos(new Vector2((Screen.width / KeyboardManager.Instance.Scale.x) / 2 - rowHolder.rect.width / 2 - keyboardKeyboardModel.show_margin, 0), 0.25f)
                .OnUpdate(() => KeyboardManagerEvent.OnShowUpdate?.Invoke(this))
                .OnComplete(() => KeyboardManagerEvent.OnShowComplete?.Invoke(this));
        }

        private void SetShowBottom()
        {
            rowHolder.gameObject.SetActive(true);
            rowHolder.DOKill();
            rowHolder.DOAnchorPos(new Vector2(0, -(Screen.height / KeyboardManager.Instance.Scale.y) / 2 + rowHolder.rect.height / 2 + keyboardKeyboardModel.show_margin), 0.25f)
                .OnUpdate(() => KeyboardManagerEvent.OnShowUpdate?.Invoke(this))
                .OnComplete(() => KeyboardManagerEvent.OnShowComplete?.Invoke(this));
        }

        private IEnumerator ShowCR(Action completeCallback)
        {
            yield return null;

            SetShow(completeCallback);
        }

        private IEnumerator HideCR(bool now = false)
        {
            yield return null;

            SetStart(now);
        }

        public KeyboardBoard Setup(string language, KeyboardKeyboardModel keyboardKeyboardModel)
        {
            this.language = language;
            this.keyboardKeyboardModel = keyboardKeyboardModel;

            name = "KEYBOARDMANAGER";

            SetContent();
            SetStart();

            return this;
        }

        public void Show(Action completeCallback = null)
        {
            if (showCR != null)
                StopCoroutine(showCR);

            if (hideCR != null)
                StopCoroutine(hideCR);

            showCR = StartCoroutine(ShowCR(completeCallback));
        }

        public void Hide(bool now = false)
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (showCR != null)
                StopCoroutine(showCR);

            if (hideCR != null)
                StopCoroutine(hideCR);

            hideCR = StartCoroutine(HideCR(now));
        }
    }
}
