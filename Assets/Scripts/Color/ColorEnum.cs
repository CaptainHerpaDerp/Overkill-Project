using Color = UnityEngine.Color;

namespace TeamColors
{
    public class ColorEnum
    {
        public enum TEAMCOLOR {
            RED,
            GREEN,
            BLUE, 
            PURPLE,
            DEFAULT
        }

        static readonly Color[] RGBColors = new Color[] {
            Color.red,
            Color.green,
            Color.blue,
            Color.magenta,
            Color.black
        };

        public static Color GetColor(TEAMCOLOR color) {
            return RGBColors[(int)color];
        }
    }
}

