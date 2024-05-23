using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;


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
