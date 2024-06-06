using Players;
using UnityEngine;

public class WinnerCrown : MonoBehaviour
{
    [SerializeField] private GameObject playerVisibleCrown, restVisibleCrown;

    public void SetCrownParentPlayer(Player winPlayer)
    {
        if (playerVisibleCrown == null || restVisibleCrown == null)
        {
            Debug.LogError("Crown not set in inspector");
            return;
        }

        playerVisibleCrown.layer = 7 + (int)winPlayer.TeamColor;
        restVisibleCrown.layer = 11 + (int)winPlayer.TeamColor;

        // Sets the colour of the player visible crown to be transparent
        Color crownColor = playerVisibleCrown.GetComponent<MeshRenderer>().material.color;
        crownColor.a = 0;
        playerVisibleCrown.GetComponent<MeshRenderer>().material.color = crownColor;
    }
}
