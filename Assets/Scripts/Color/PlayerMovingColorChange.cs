using UnityEngine;

public class PlayerMovingColorChange : MonoBehaviour {
    
    public ColorEnum.TEAMCOLOR playerColor;

    public void OnColorTriggerEnter(Collider other) {
        other.gameObject.GetComponent<Plant>().TeamColor = playerColor;
    }

    public void SetCharacterColor(ColorEnum.TEAMCOLOR newColor) {
        playerColor = newColor;
    }
}
