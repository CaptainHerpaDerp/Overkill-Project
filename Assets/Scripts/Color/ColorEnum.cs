using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;


public class ColorEnum
{
    public enum PLANTCOLOR {
        RED,
        PURPLE,
        BLUE, 
        GREEN,
        DEFAULT
    }

    static readonly Color[] RGBColors = new Color[] {
        Color.red,
        new Color(160, 32, 240),
        Color.blue,
        Color.green,
        Color.black
    };

    public static Color GetColor(PLANTCOLOR color) {
        return RGBColors[(int)color];
    }


}
