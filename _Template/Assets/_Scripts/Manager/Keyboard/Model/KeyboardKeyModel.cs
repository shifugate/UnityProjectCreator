using Assets._Scripts.Manager.Keyboard.Model.__Base;

namespace Assets._Scripts.Manager.Keyboard.Model
{
    public class KeyboardKeyModel : KeyboardModelBase
    {
        public KeyboardKeyLevelModel level0 = new KeyboardKeyLevelModel();
        public KeyboardKeyLevelModel level1 = new KeyboardKeyLevelModel();
        public KeyboardKeyLevelModel level2 = new KeyboardKeyLevelModel();

        public KeyboardKeyLevelModel Level { get { return KeyboardManager.Instance.GetLevel(this); } }
    }
}
