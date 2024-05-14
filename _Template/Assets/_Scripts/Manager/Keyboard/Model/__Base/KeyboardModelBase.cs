namespace Assets._Scripts.Manager.Keyboard.Model.__Base
{
    public class KeyboardModelBase
    {
        public string font_release_color;
        public string font_press_color;
        public string font_lock_color;
        public string font_release_hold_color;
        public string font_press_hold_color;
        public KeyboardSpriteModel release_key;
        public KeyboardSpriteModel press_key;
        public KeyboardSpriteModel lock_key;
        public KeyboardSpriteModel release_hold_key;
        public KeyboardSpriteModel press_hold_key;
        public KeyboardSpriteModel background;
        public float width_key;
        public float height_key;
        public float space_row;
        public float space_key;
        public int margin_x;
        public int margin_y;
        public int? start_at;
        public int? show_at;
        public float show_margin;
        public float click_time;
    }
}
