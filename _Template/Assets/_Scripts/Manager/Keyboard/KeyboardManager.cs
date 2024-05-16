using Assets._Scripts.Manager.Keyboard.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Assets._Scripts.Manager.Keyboard.Board;
using System;
using Assets._Scripts.Util;
using TMPro;
using Assets._Scripts.Manager.Keyboard.Data;
using System.Collections;
using Assets._Scripts.Manager.Language;
using Assets._Scripts.Manager.Keyboard.Key;
using UnityEngine.Events;
using Assets._Scripts.Manager.Keyboard.Event;
using UnityEngine.TextCore.Text;

namespace Assets._Scripts.Manager.Keyboard
{
    public class KeyboardManagerUpdateChange : UnityEvent { }
    public class KeyboardManagerUpdateKey : UnityEvent { }
    public class KeyboardManagerUpdateLevel : UnityEvent { }

    public class KeyboardManager : MonoBehaviour
    {
        #region Singleton
        private static KeyboardManager instance;
        public static KeyboardManager Instance { get { return instance; } }

        public static void InstanceNW(InitializerManager manager)
        {
            if (instance == null)
            {
                instance = GameObject.Instantiate<KeyboardManager>(Resources.Load<KeyboardManager>("Manager/Keyboard/KeyboardManager"));
                instance.name = "KeyboardManager";
            }

            instance.Initialize(manager);
        }
        #endregion 

        public enum Type 
        {
            Normal,
            Email,
            Numeric,
        }

        public enum Direction 
        {
            Left,
            Top,
            Right,
            Bottom,
        }

        public enum Key
        {
            Text,
            Enter,
            Shift,
            ShiftLock,
            Swap,
            Delete,
            Tab,
        }

        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private RectTransform keyboardHolder;
        [SerializeField]
        private KeyboardBoard keyboardBoard;

        public KeyboardManagerUpdateChange onKeyboardManagerUpdateChange = new KeyboardManagerUpdateChange();
        public KeyboardManagerUpdateKey onKeyboardManagerUpdateKey = new KeyboardManagerUpdateKey();
        public KeyboardManagerUpdateLevel onKeyboardManagerUpdateLevel = new KeyboardManagerUpdateLevel();

        private Dictionary<string, KeyboardModel> contents = new Dictionary<string, KeyboardModel>();

        private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

        private List<KeyboardBoard> keyboardBoards = new List<KeyboardBoard>();
        public List<KeyboardBoard> KeyboardBoards { get { return keyboardBoards; } }

        private List<KeyboardData> keyboardDatas;

        private Coroutine focusCR;

        private TMP_InputField inputField;

        private GameObject keyObject;

        public Vector3 Scale { get { return canvas.transform.localScale; } }

        private bool shifted;
        public bool Shifted { get { return shifted; } }

        private bool shiftedLocked;
        public bool ShiftedLocked { get { return shiftedLocked; } }

        private int level;
        public int Level { get { return level; } }

        private int tabIndex;

        private string character;
        private string accent;

        private void Update()
        {
            UpdateInput();
        }

        private void UpdateInput()
        {
            if (inputField == null)
                return;

            if (Input.GetMouseButtonDown(0) && keyObject == null)
                keyObject = ScreenUtil.GetUIOverPointerByName("KEYBOARDMANAGER_KEY");

            if (Input.GetMouseButtonUp(0) && keyObject != null)
            {
                KeyboardKey keyboardKey = keyObject.GetComponent<KeyboardKey>();

                if (keyboardKey != null)
                    keyboardKey.OutKey();

                keyObject = null;

                return;
            }

            if (Input.GetMouseButtonDown(0) && !ScreenUtil.PointerOverUIName("KEYBOARDMANAGER") && keyObject == null)
            {
                inputField = null;
            }
            else if (Input.GetMouseButton(0))
            {
                if (keyObject != null)
                {
                    KeyboardKey keyboardKey = keyObject.GetComponent<KeyboardKey>();

                    if (keyboardKey != null)
                        keyboardKey.OverKey();
                }

                UpdateFocus();
            }
        }

        private void UpdateFocus()
        {
            if (inputField == null) 
                return;

            UpdateFocus(inputField);
        }

        private void UpdateFocus(TMP_InputField inputField)
        {
            inputField.Select();
            inputField.ActivateInputField();
            inputField.caretPosition = inputField.text.Length;
        }

        private void Initialize(InitializerManager manager)
        {
            transform.SetParent(manager.transform);

            SetProperties();
            SetKeyboards();
        }

        private IEnumerator FocusCR()
        {
            while (keyboardDatas != null && keyboardDatas.Count > 0)
            {
                keyboardDatas.RemoveAll(data => data?.inputField == null);

                foreach (KeyboardData keyboardData in keyboardDatas)
                {
                    if (keyboardData.inputField.isFocused && inputField != keyboardData.inputField)
                    {
                        tabIndex = keyboardDatas.FindIndex(x => x.inputField == keyboardData.inputField);

                        inputField = keyboardData.inputField;

                        if (inputField != null)
                        {
                            foreach (KeyboardBoard keyboard in keyboardBoards)
                            {
                                if (keyboard.KeyboardKeyboardModel.type == keyboardData.type && keyboard.KeyboardKeyboardModel.language == LanguageManager.Instance.Language)
                                {
                                    character = null;
                                    accent = null;
                                    shifted = false;
                                    shiftedLocked = false;
                                    level = 0;

                                    onKeyboardManagerUpdateChange?.Invoke();

                                    keyboard.Show();
                                }
                                else
                                {
                                    keyboard.Hide();
                                }
                            }
                        }
                    }
                    else if (inputField == null)
                    {
                        foreach (KeyboardBoard keyboard in keyboardBoards)
                            keyboard.Hide();
                    }
                }

                yield return null;
            }
        }

        private void SetProperties()
        {
#if UNITY_STANDALONE
            string[] files = Directory.GetFiles($"{Application.streamingAssetsPath}/Manager/Keyboard", "*.json");

            contents.Clear();

            foreach (string file in files)
                contents.Add(Path.GetFileNameWithoutExtension(file), JsonConvert.DeserializeObject<KeyboardModel>(File.ReadAllText(file)));
#elif UNITY_ANDROID || UNITY_IOS
            TextAsset[] assets = Resources.LoadAll<TextAsset>("Manager/Keyboard");

            contents.Clear();

            foreach (TextAsset asset in assets)
                contents.Add(asset.name, JsonConvert.DeserializeObject<KeyboardModel>(asset.text));
#endif
        }

        private void SetKeyboards()
        {
            foreach (KeyValuePair<string, KeyboardModel> content in contents)
            {
                foreach (int type in content.Value.keyboards)
                {
#if UNITY_STANDALONE
                    try
                    {
                        KeyboardKeyboardModel keyboardKeyboardModel = JsonConvert.DeserializeObject<KeyboardKeyboardModel>(File.ReadAllText($"{Application.streamingAssetsPath}/Manager/Keyboard/_{content.Key}/{type}.json"));
                        keyboardKeyboardModel.type = (Type)type;
                        keyboardKeyboardModel.language = content.Key;

                        SetKeyboardData(content.Value, keyboardKeyboardModel);

                        keyboardBoards.Add(Instantiate(keyboardBoard, keyboardHolder).Setup(content.Key, keyboardKeyboardModel));
                    }
                    catch(Exception ex)
                    {
                        SystemUtil.Log(GetType(), ex, SystemUtil.LogType.Exception);
                    }
#elif UNITY_ANDROID || UNITY_IOS
                    try
                    {
                        KeyboardKeyboardModel keyboardKeyboardModel = JsonConvert.DeserializeObject<KeyboardKeyboardModel>(Resources.Load<TextAsset>($"Manager/Keyboard/_{content.Key}/{type}").text);
                        keyboardKeyboardModel.type = (Type)type;
                        keyboardKeyboardModel.language = content.Key;

                        SetKeyboardData(content.Value, keyboardKeyboardModel);

                        keyboardBoards.Add(Instantiate(keyboardBoard, keyboardHolder).Setup(content.Key, keyboardKeyboardModel));
                    }
                    catch (Exception ex)
                    {
                        SystemUtil.Log(GetType(), ex, SystemUtil.LogType.Exception);
                    }
#endif
                }
            }
        }

        private void SetKeyboardData(KeyboardModel keyboardModel, KeyboardKeyboardModel keyboardKeyboardModel)
        {
            if (keyboardKeyboardModel.font_release_color == null)
                keyboardKeyboardModel.font_release_color = keyboardModel.font_release_color;

            if (keyboardKeyboardModel.font_press_color == null)
                keyboardKeyboardModel.font_press_color = keyboardModel.font_press_color;

            if (keyboardKeyboardModel.font_release_hold_color == null)
                keyboardKeyboardModel.font_release_hold_color = keyboardModel.font_release_hold_color;

            if (keyboardKeyboardModel.font_press_hold_color == null)
                keyboardKeyboardModel.font_press_hold_color = keyboardModel.font_press_hold_color;

            if (keyboardKeyboardModel.font_lock_color == null)
                keyboardKeyboardModel.font_lock_color = keyboardModel.font_lock_color;

            if (keyboardKeyboardModel.release_key == null)
                keyboardKeyboardModel.release_key = keyboardModel.release_key;

            if (keyboardKeyboardModel.press_key == null)
                keyboardKeyboardModel.press_key = keyboardModel.press_key;

            if (keyboardKeyboardModel.release_hold_key == null)
                keyboardKeyboardModel.release_hold_key = keyboardModel.release_hold_key;

            if (keyboardKeyboardModel.press_hold_key == null)
                keyboardKeyboardModel.press_hold_key = keyboardModel.press_hold_key;

            if (keyboardKeyboardModel.lock_key == null)
                keyboardKeyboardModel.lock_key = keyboardModel.lock_key;

            if (keyboardKeyboardModel.background == null)
                keyboardKeyboardModel.background = keyboardModel.background;

            if (keyboardKeyboardModel.width_key == 0)
                keyboardKeyboardModel.width_key = keyboardModel.width_key;

            if (keyboardKeyboardModel.height_key == 0)
                keyboardKeyboardModel.height_key = keyboardModel.height_key;

            if (keyboardKeyboardModel.space_row == 0)
                keyboardKeyboardModel.space_row = keyboardModel.space_row;

            if (keyboardKeyboardModel.space_key == 0)
                keyboardKeyboardModel.space_key = keyboardModel.space_key;

            if (keyboardKeyboardModel.margin_x == 0)
                keyboardKeyboardModel.margin_x = keyboardModel.margin_x;

            if (keyboardKeyboardModel.margin_y == 0)
                keyboardKeyboardModel.margin_y = keyboardModel.margin_y;

            if (keyboardKeyboardModel.start_at == null)
                keyboardKeyboardModel.start_at = keyboardModel.start_at;

            if (keyboardKeyboardModel.show_at == null)
                keyboardKeyboardModel.show_at = keyboardModel.show_at;

            if (keyboardKeyboardModel.show_margin == 0)
                keyboardKeyboardModel.show_margin = keyboardModel.show_margin;

            if (keyboardKeyboardModel.click_time == 0)
                keyboardKeyboardModel.click_time = keyboardModel.click_time;

            SetSprite(keyboardKeyboardModel.release_key);
            SetSprite(keyboardKeyboardModel.press_key);
            SetSprite(keyboardKeyboardModel.release_hold_key);
            SetSprite(keyboardKeyboardModel.press_hold_key);
            SetSprite(keyboardKeyboardModel.lock_key);
            SetSprite(keyboardKeyboardModel.background);

            foreach (KeyboardLevelModel keyboardLevelModel in keyboardKeyboardModel.levels)
                SetLevelData(keyboardKeyboardModel, keyboardLevelModel);
        }

        private void SetLevelData(KeyboardKeyboardModel keyboardKeyboardModel, KeyboardLevelModel keyboardLevelModel)
        {
            if (keyboardKeyboardModel.font_release_color == null)
                keyboardKeyboardModel.font_release_color = keyboardLevelModel.font_release_color;

            if (keyboardKeyboardModel.font_press_color == null)
                keyboardKeyboardModel.font_press_color = keyboardLevelModel.font_press_color;

            if (keyboardKeyboardModel.font_release_hold_color == null)
                keyboardKeyboardModel.font_release_hold_color = keyboardLevelModel.font_release_hold_color;

            if (keyboardKeyboardModel.font_press_hold_color == null)
                keyboardKeyboardModel.font_press_hold_color = keyboardLevelModel.font_press_hold_color;

            if (keyboardKeyboardModel.font_lock_color == null)
                keyboardKeyboardModel.font_lock_color = keyboardLevelModel.font_lock_color;

            if (keyboardKeyboardModel.release_key == null)
                keyboardKeyboardModel.release_key = keyboardLevelModel.release_key;

            if (keyboardKeyboardModel.press_key == null)
                keyboardKeyboardModel.press_key = keyboardLevelModel.press_key;

            if (keyboardKeyboardModel.release_hold_key == null)
                keyboardKeyboardModel.release_hold_key = keyboardLevelModel.release_hold_key;

            if (keyboardKeyboardModel.press_hold_key == null)
                keyboardKeyboardModel.press_hold_key = keyboardLevelModel.press_hold_key;

            if (keyboardKeyboardModel.lock_key == null)
                keyboardKeyboardModel.lock_key = keyboardLevelModel.lock_key;

            if (keyboardKeyboardModel.background == null)
                keyboardKeyboardModel.background = keyboardLevelModel.background;

            if (keyboardKeyboardModel.width_key == 0)
                keyboardKeyboardModel.width_key = keyboardLevelModel.width_key;

            if (keyboardKeyboardModel.height_key == 0)
                keyboardKeyboardModel.height_key = keyboardLevelModel.height_key;

            if (keyboardKeyboardModel.space_row == 0)
                keyboardKeyboardModel.space_row = keyboardLevelModel.space_row;

            if (keyboardKeyboardModel.space_key == 0)
                keyboardKeyboardModel.space_key = keyboardLevelModel.space_key;

            if (keyboardKeyboardModel.margin_x == 0)
                keyboardKeyboardModel.margin_x = keyboardLevelModel.margin_x;

            if (keyboardKeyboardModel.margin_y == 0)
                keyboardKeyboardModel.margin_y = keyboardLevelModel.margin_y;

            if (keyboardKeyboardModel.click_time == 0)
                keyboardKeyboardModel.click_time = keyboardLevelModel.click_time;

            SetSprite(keyboardKeyboardModel.release_key);
            SetSprite(keyboardKeyboardModel.press_key);
            SetSprite(keyboardKeyboardModel.release_hold_key);
            SetSprite(keyboardKeyboardModel.press_hold_key);
            SetSprite(keyboardKeyboardModel.lock_key);
            SetSprite(keyboardKeyboardModel.background);

            foreach (KeyboardRowModel keyboardRowModel in keyboardLevelModel.rows)
                SetRowData(keyboardKeyboardModel, keyboardRowModel);
        }

        private void SetRowData(KeyboardKeyboardModel keyboardKeyboardModel, KeyboardRowModel keyboardRowModel)
        {
            if (keyboardRowModel.font_release_color == null)
                keyboardRowModel.font_release_color = keyboardKeyboardModel.font_release_color;

            if (keyboardRowModel.font_press_color == null)
                keyboardRowModel.font_press_color = keyboardKeyboardModel.font_press_color;

            if (keyboardRowModel.font_release_hold_color == null)
                keyboardRowModel.font_release_hold_color = keyboardKeyboardModel.font_release_hold_color;

            if (keyboardRowModel.font_press_hold_color == null)
                keyboardRowModel.font_press_hold_color = keyboardKeyboardModel.font_press_hold_color;

            if (keyboardRowModel.font_lock_color == null)
                keyboardRowModel.font_lock_color = keyboardKeyboardModel.font_lock_color;

            if (keyboardRowModel.release_key == null)
                keyboardRowModel.release_key = keyboardKeyboardModel.release_key;

            if (keyboardRowModel.press_key == null)
                keyboardRowModel.press_key = keyboardKeyboardModel.press_key;

            if (keyboardRowModel.release_hold_key == null)
                keyboardRowModel.release_hold_key = keyboardKeyboardModel.release_hold_key;

            if (keyboardRowModel.press_hold_key == null)
                keyboardRowModel.press_hold_key = keyboardKeyboardModel.press_hold_key;

            if (keyboardRowModel.lock_key == null)
                keyboardRowModel.lock_key = keyboardKeyboardModel.lock_key;

            if (keyboardRowModel.background == null)
                keyboardRowModel.background = keyboardKeyboardModel.background;

            if (keyboardRowModel.width_key == 0)
                keyboardRowModel.width_key = keyboardKeyboardModel.width_key;

            if (keyboardRowModel.height_key == 0)
                keyboardRowModel.height_key = keyboardKeyboardModel.height_key;

            if (keyboardRowModel.space_row == 0)
                keyboardRowModel.space_row = keyboardKeyboardModel.space_row;

            if (keyboardRowModel.space_key == 0)
                keyboardRowModel.space_key = keyboardKeyboardModel.space_key;

            if (keyboardRowModel.margin_x == 0)
                keyboardRowModel.margin_x = keyboardKeyboardModel.margin_x;

            if (keyboardRowModel.margin_y == 0)
                keyboardRowModel.margin_y = keyboardKeyboardModel.margin_y;

            if (keyboardRowModel.click_time == 0)
                keyboardRowModel.click_time = keyboardKeyboardModel.click_time;

            SetSprite(keyboardRowModel.release_key);
            SetSprite(keyboardRowModel.press_key);
            SetSprite(keyboardRowModel.release_hold_key);
            SetSprite(keyboardRowModel.press_hold_key);
            SetSprite(keyboardRowModel.lock_key);
            SetSprite(keyboardRowModel.background);

            foreach (KeyboardKeyModel keyboardKeyModel in keyboardRowModel.keys)
                SetKeyData(keyboardRowModel, keyboardKeyModel);
        }

        private void SetKeyData(KeyboardRowModel keyboardRowModel, KeyboardKeyModel keyboardKeyModel)
        {
            if (keyboardKeyModel.font_release_color == null)
                keyboardKeyModel.font_release_color = keyboardRowModel.font_release_color;

            if (keyboardKeyModel.font_press_color == null)
                keyboardKeyModel.font_press_color = keyboardRowModel.font_press_color;

            if (keyboardKeyModel.font_release_hold_color == null)
                keyboardKeyModel.font_release_hold_color = keyboardRowModel.font_release_hold_color;

            if (keyboardKeyModel.font_press_hold_color == null)
                keyboardKeyModel.font_press_hold_color = keyboardRowModel.font_press_hold_color;

            if (keyboardKeyModel.font_lock_color == null)
                keyboardKeyModel.font_lock_color = keyboardRowModel.font_lock_color;

            if (keyboardKeyModel.release_key == null)
                keyboardKeyModel.release_key = keyboardRowModel.release_key;

            if (keyboardKeyModel.press_key == null)
                keyboardKeyModel.press_key = keyboardRowModel.press_key;

            if (keyboardKeyModel.release_hold_key == null)
                keyboardKeyModel.release_hold_key = keyboardRowModel.release_hold_key;

            if (keyboardKeyModel.press_hold_key == null)
                keyboardKeyModel.press_hold_key = keyboardRowModel.press_hold_key;

            if (keyboardKeyModel.lock_key == null)
                keyboardKeyModel.lock_key = keyboardRowModel.lock_key;

            if (keyboardKeyModel.background == null)
                keyboardKeyModel.background = keyboardRowModel.background;

            if (keyboardKeyModel.width_key == 0)
                keyboardKeyModel.width_key = keyboardRowModel.width_key;

            if (keyboardKeyModel.height_key == 0)
                keyboardKeyModel.height_key = keyboardRowModel.height_key;

            if (keyboardKeyModel.space_row == 0)
                keyboardKeyModel.space_row = keyboardRowModel.space_row;

            if (keyboardKeyModel.space_key == 0)
                keyboardKeyModel.space_key = keyboardRowModel.space_key;

            if (keyboardKeyModel.margin_x == 0)
                keyboardKeyModel.margin_x = keyboardRowModel.margin_x;

            if (keyboardKeyModel.margin_y == 0)
                keyboardKeyModel.margin_y = keyboardRowModel.margin_y;

            if (keyboardKeyModel.click_time == 0)
                keyboardKeyModel.click_time = keyboardRowModel.click_time;

            SetSprite(keyboardKeyModel.release_key);
            SetSprite(keyboardKeyModel.press_key);
            SetSprite(keyboardKeyModel.release_hold_key);
            SetSprite(keyboardKeyModel.press_hold_key);
            SetSprite(keyboardKeyModel.lock_key);
            SetSprite(keyboardKeyModel.background);
        }

        private void SetSprite(KeyboardSpriteModel spriteModel)
        {
            if (spriteModel?.file == null)
                return;

            if (sprites.ContainsKey(spriteModel.file))
                return;

            Texture2D texture = null;
            Sprite sprite = null;

            try
            {
#if UNITY_STANDALONE
                texture = new Texture2D(2, 2);
                texture.LoadImage(File.ReadAllBytes($"{Application.streamingAssetsPath}/Manager/Keyboard/__Texture/{spriteModel.file}"));
#elif UNITY_ANDROID || UNITY_IOS
                texture = Resources.Load<Texture2D>($"Manager/Keyboard/__Texture/{Path.GetFileNameWithoutExtension(spriteModel.file)}");
#endif

                if (spriteModel.margin == null || spriteModel.margin.Count != 4)
                    spriteModel.margin = new List<int>(new int[] { 0, 0, 0, 0 });

                sprite = Sprite.Create(texture, 
                    new Rect(0, 0, texture.width, texture.height), 
                    new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, 
                    new Vector4(spriteModel.margin[0], spriteModel.margin[1], spriteModel.margin[2], spriteModel.margin[3]));
            }
            catch (Exception ex)
            {
                SystemUtil.Log(GetType(), ex, SystemUtil.LogType.Exception);

                return;
            }

            if (sprite == null)
                return;

            sprites.Add(spriteModel.file, sprite);
        }

        public Sprite GetSprite(KeyboardSpriteModel spriteModel)
        {
            return spriteModel?.file == null || !sprites.ContainsKey(spriteModel.file) ? null : sprites[spriteModel.file];
        }

        public bool HasSpriteBorder(Sprite sprite)
        {
            return sprite != null && sprite.border != null && (sprite.border[0] > 0 || sprite.border[1] > 0 || sprite.border[2] > 0 || sprite.border[3] > 0);
        }

        public void SetInputFields(List<KeyboardData> data)
        {
            keyboardDatas = data;
            keyboardDatas.Sort((x, y) => x.tabIndex.CompareTo(y.tabIndex));

            if (focusCR != null)
                StopCoroutine(focusCR);

            focusCR = StartCoroutine(FocusCR());
        }

        public void AddInputField(KeyboardData data)
        {
            if (data == null)
                return;

            if (keyboardDatas == null)
                keyboardDatas = new List<KeyboardData>();

            keyboardDatas.RemoveAll(kData => kData?.inputField == null);

            if (!keyboardDatas.Exists(kData => kData.inputField == data.inputField))
                keyboardDatas.Add(data);

            keyboardDatas.Sort((x, y) => x.tabIndex.CompareTo(y.tabIndex));

            if (focusCR != null)
                StopCoroutine(focusCR);

            focusCR = StartCoroutine(FocusCR());
        }

        public void RemoveInputField(KeyboardData data)
        {
            if (data == null)
                return;

            if (keyboardDatas == null || keyboardDatas.Count == 0)
                return;

            if (focusCR != null)
                StopCoroutine(focusCR);

            keyboardDatas.RemoveAll(kData => kData?.inputField == null);
            keyboardDatas.RemoveAll(kData => kData == data);

            if (keyboardDatas.Count == 0)
                return;

            focusCR = StartCoroutine(FocusCR());
        }

        public void RemoveInputField(TMP_InputField inputField)
        {
            if (inputField == null)
                return;

            if (keyboardDatas == null || keyboardDatas.Count == 0)
                return;

            if (focusCR != null)
                StopCoroutine(focusCR);

            keyboardDatas.RemoveAll(data => data?.inputField == null);
            keyboardDatas.RemoveAll(data => data.inputField == inputField);

            if (keyboardDatas.Count == 0)
                return;

            focusCR = StartCoroutine(FocusCR());
        }

        public void KeyClick(Key key, string text = null, int? level = null, KeyboardKeyModel model = null)
        {
            if (character != null && accent != null)
            {
                string[] split0 = accent.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries );

                if (split0.Length > 0)
                {
                    foreach (string s0 in split0)
                    {
                        string[] split1 = s0.Split("=");

                        if (split1.Length != 2)
                            continue;

                        string[] split2 = split1[0].Split('+');

                        if (split2.Length != 2)
                            continue;

                        if (split2[1].Equals(text))
                        {
                            character = split1[1];
                            accent = null;

                            break;
                        }
                    }
                }

                if (accent != null)
                    character += text;

                shifted = false;

                if (inputField != null)
                {
                    if (!string.IsNullOrEmpty(character) && !inputField.fontAsset.HasCharacters(character, out uint[] missing, false, true))
                        character = "";

                    inputField.text += character;
                }

                character = null;
                accent = null;

                onKeyboardManagerUpdateKey?.Invoke();

                UpdateFocus();

                return;
            }

            switch(key)
            {
                case Key.Shift:
                    shifted = !shiftedLocked ? !shifted : false;
                    shiftedLocked = false;

                    onKeyboardManagerUpdateKey?.Invoke();

                    UpdateFocus();

                    break;
                case Key.ShiftLock:
                    shifted = false;
                    shiftedLocked = !shiftedLocked;

                    onKeyboardManagerUpdateKey?.Invoke();

                    UpdateFocus();

                    break;
                case Key.Enter:
                    KeyboardManagerEvent.OnEnter?.Invoke();

                    onKeyboardManagerUpdateKey?.Invoke();

                    UpdateFocus();

                    break;
                case Key.Swap:
                    if (level == null || level == this.level)
                        break;

                    this.level = (int)level;

                    shifted = false;
                    shiftedLocked = false;

                    onKeyboardManagerUpdateLevel?.Invoke();

                    UpdateFocus();

                    break;
                case Key.Tab:
                    keyboardDatas.RemoveAll(data => data?.inputField == null);

                    if (keyboardDatas.Count <= 1)
                        break;

                    tabIndex++;

                    if (tabIndex >= keyboardDatas.Count)
                        tabIndex = 0;

                    UpdateFocus(keyboardDatas[tabIndex].inputField);

                    break;
                case Key.Delete:
                    shifted = false;

                    if (inputField != null && inputField.text.Length > 0)
                        inputField.text = inputField.text.Substring(0, inputField.text.Length -1);

                    onKeyboardManagerUpdateKey?.Invoke();

                    UpdateFocus();

                    break;
                case Key.Text: 
                    if (model?.accents_normal != null && !shifted)
                    {
                        character = text;
                        accent = model.accents_normal;

                        break;
                    }
                    else if (model?.accents_shifted != null && shifted)
                    {
                        character = text;
                        accent = model.accents_shifted;

                        break;
                    }

                    shifted = false;

                    if (inputField != null)
                    {
                        if (!string.IsNullOrEmpty(text) && !inputField.fontAsset.HasCharacters(text, out uint[] missing, false, true))
                            text = "";

                        inputField.text += text;
                    }

                    onKeyboardManagerUpdateKey?.Invoke();

                    UpdateFocus();

                    break;
            }
        }
    }
}
