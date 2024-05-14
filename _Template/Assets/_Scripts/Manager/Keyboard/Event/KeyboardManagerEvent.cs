using Assets._Scripts.Manager.Keyboard.Board;
using System;

namespace Assets._Scripts.Manager.Keyboard.Event
{
    public static class KeyboardManagerEvent
    {
        public static Action OnEnter;
        public static Action<KeyboardBoard> OnShowUpdate;
        public static Action<KeyboardBoard> OnShowComplete;
        public static Action<KeyboardBoard> OnHideUpdate;
        public static Action<KeyboardBoard> OnHideComplete;
    }
}
