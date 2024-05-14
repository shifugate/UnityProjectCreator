using Assets._Scripts.Manager.Keyboard.Event;
using Assets._Scripts.Manager.Keyboard.Model;
using Assets._Scripts.Manager.Keyboard.Row;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
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

        private Coroutine hideCR;

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
            foreach (KeyboardRowModel keyboardRowModel in keyboardKeyboardModel.rows)
                Instantiate(keyboardRow, rowHolder).Setup(keyboardRowModel);
        }

        private void SetStart()
        {
            if (!showing)
                return;

            showing = false;

            KeyboardManager.Direction direction = (KeyboardManager.Direction)keyboardKeyboardModel.start_at;

            switch (direction)
            {
                case KeyboardManager.Direction.Left:
                    SetStartLeft();
                    break;
                case KeyboardManager.Direction.Top:
                    SetStartTop();
                    break;
                case KeyboardManager.Direction.Right:
                    SetStartRight();
                    break;
                case KeyboardManager.Direction.Bottom:
                    SetStartBottom();
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

        private void SetStartLeft()
        {
            rowHolder.DOKill();
            rowHolder.DOAnchorPos(new Vector2(-Screen.width / KeyboardManager.Instance.Scale.x, 0), 0.25f)
                .OnUpdate(() => KeyboardManagerEvent.OnHideUpdate?.Invoke(this))
                .OnComplete(() =>
                {
                    rowHolder.gameObject.SetActive(false);

                    KeyboardManagerEvent.OnHideComplete?.Invoke(this);
                });
        }

        private void SetStartTop()
        {
            rowHolder.DOKill();
            rowHolder.DOAnchorPos(new Vector2(0, Screen.height / KeyboardManager.Instance.Scale.y), 0.25f)
                .OnUpdate(() => KeyboardManagerEvent.OnHideUpdate?.Invoke(this))
                .OnComplete(() =>
                {
                    rowHolder.gameObject.SetActive(false);

                    KeyboardManagerEvent.OnHideComplete?.Invoke(this);
                });
        }

        private void SetStartRight()
        {
            rowHolder.DOKill();
            rowHolder.DOAnchorPos(new Vector2(Screen.width / KeyboardManager.Instance.Scale.x, 0), 0.25f)
                .OnUpdate(() => KeyboardManagerEvent.OnHideUpdate?.Invoke(this))
                .OnComplete(() =>
                {
                    rowHolder.gameObject.SetActive(false);

                    KeyboardManagerEvent.OnHideComplete?.Invoke(this);
                });
        }

        private void SetStartBottom()
        {
            rowHolder.DOKill();
            rowHolder.DOAnchorPos(new Vector2(0, -Screen.height / KeyboardManager.Instance.Scale.y), 0.25f)
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

        private IEnumerator HideCR()
        {
            yield return null;

            SetStart();
        }

        public KeyboardBoard Setup(string language, KeyboardKeyboardModel keyboardKeyboardModel)
        {
            this.language = language;
            this.keyboardKeyboardModel = keyboardKeyboardModel;

            name = "KEYBOARDMANAGER";

            SetSpace();
            SetMargin();
            SetTextures();
            SetRows();
            SetStart();

            return this;
        }

        public void Show(Action completeCallback)
        {
            if (hideCR != null)
                StopCoroutine(hideCR);

            SetShow(completeCallback);
        }

        public void Hide()
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (hideCR != null)
                StopCoroutine(hideCR);

            hideCR = StartCoroutine(HideCR());
        }
    }
}
