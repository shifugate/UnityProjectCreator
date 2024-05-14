using Assets._Scripts.Manager.Keyboard.Model.__Base;
using System.Collections.Generic;

namespace Assets._Scripts.Manager.Keyboard.Model
{
    public class KeyboardKeyLevelModel : KeyboardModelBase
    {
        public string normal;
        public string shifted;
        public List<string> normal_hold = new List<string>();
        public List<string> shifted_hold = new List<string>();
        public bool shift;
        public bool enter;
        public bool swap;
        public bool delete;
        public bool tab;
        public int level;
    }
}
