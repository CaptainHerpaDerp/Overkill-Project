using System.Collections.Generic;
using UnityEngine;

namespace TeamColors
{
    /// <summary>
    /// Keeps track of the player's position and team color.
    /// </summary>
    public class PlayerLocator : MonoBehaviour
    {
        public static PlayerLocator Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("PlayerLocator already exists in the scene. Deleting this instance.");
                Destroy(this);
            }
        }

        private Dictionary<ColorEnum.TEAMCOLOR, Transform> playerTransformPairs = new Dictionary<ColorEnum.TEAMCOLOR, Transform>();

        public Transform GetTransformOfTeam(ColorEnum.TEAMCOLOR teamColor)
        {
            return playerTransformPairs[teamColor];
        }

        public Vector3 GetPositionOfPlayer(ColorEnum.TEAMCOLOR teamColor)
        {
            return playerTransformPairs[teamColor].position;
        }

        public void RegisterPlayerOfTeam(ColorEnum.TEAMCOLOR teamColor, Transform playerTransform)
        {
            print("Registered player of team " + teamColor);
            playerTransformPairs.Add(teamColor, playerTransform);
        }
    }
}