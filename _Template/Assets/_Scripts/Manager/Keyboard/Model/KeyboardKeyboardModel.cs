using Assets._Scripts.Manager.Keyboard.Model.__Base;
using System.Collections.Generic;

namespace Assets._Scripts.Manager.Keyboard.Model
{
    public class KeyboardKeyboardModel : KeyboardModelBase
    {
        public string language;
        public KeyboardManager.Type type;
        public List<KeyboardLevelModel> levels = new List<KeyboardLevelModel>();
    }
}
