using static TeamColors.ColorEnum;
using UnityEngine;
using Players;
using TeamColors;
using System.Collections.Generic;
using System.Linq;

namespace GameManagement
{

    public enum AchievementType { MostDeaths, MostKills, MostCreaturesTamed, MostTimeWinning, MostCamper, MostMover }

    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance;

        public Dictionary<Player, Achievement> AchievementList = new Dictionary<Player, Achievement>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("AchievementManager already exists in the scene. Deleting this instance.");
                Destroy(this);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            GameManager.Instance.OnGameReload += ResetAchievements;
            PlayerManager.Instance.OnPlayersSelected += StartAchievements;
        }

        private void ResetAchievements()
        {
            AchievementList.Clear();
            AchievementList = new Dictionary<Player, Achievement>();

            foreach (Player player in GameManager.Instance.PlayerList)
            {
                player.OnPlayerRespawn -= ProcessRespawn;
            }

            foreach (KeyValuePair<Transform, ColorEnum.TEAMCOLOR> kvp in AnimalLocator.Instance.animalTransformPairs)
            {
                AnimalLocator.Instance.OnCreatureColorChanged -= ProcessCreatureColorTurn;
            }
        }

        private void StartAchievements()
        {
            foreach (Player player in GameManager.Instance.PlayerList)
            {
                AchievementList[player] = new Achievement();

                player.OnPlayerRespawn += ProcessRespawn;
                GameManager.Instance.OnGameEnd += () => AchievementList[player].timeMoving = player.timeMoving;  
            }

            GameManager.Instance.OnGameEnd += () =>
            {
                Player mostMovePlayer = GameManager.Instance.GetMostMovePlayerWinner();
                AchievementList[mostMovePlayer].timeMoving = mostMovePlayer.timeMoving;
            };

            foreach (KeyValuePair<Transform, ColorEnum.TEAMCOLOR> kvp in AnimalLocator.Instance.animalTransformPairs)
            {
                AnimalLocator.Instance.OnCreatureColorChanged += ProcessCreatureColorTurn;
            }
        }

        private void ProcessRespawn(Player pusherPlayer, Player respawnedPlayer)
        {
            if (pusherPlayer != null)
                AchievementList[pusherPlayer].pushCount++;

            if (respawnedPlayer != null)
                AchievementList[respawnedPlayer].respawnCount++;
        }

        private void ProcessCreatureColorTurn(TEAMCOLOR newColor)
        {
            foreach (Player player in AchievementList.Keys)
            {
                if (player.TeamColor == newColor)
                    AchievementList[player].creatureTurnCount++;
            }
        }

        public int GetIndexOfAchievementWinner(AchievementType type)
        {
            if (GetAchievementWinner(type) == null)
                return -1;

            return GameManager.Instance.PlayerList.IndexOf(GetAchievementWinner(type));
        }

        public string GetAchievementValue(AchievementType type, int winnerIndex)
        {
            Player winner = GameManager.Instance.PlayerList[winnerIndex];

            switch (type)
            {
                case AchievementType.MostDeaths:
                    return AchievementList[winner].respawnCount.ToString();
                case AchievementType.MostKills:
                    return AchievementList[winner].pushCount.ToString();
                case AchievementType.MostCreaturesTamed:
                    return AchievementList[winner].creatureTurnCount.ToString();
                case AchievementType.MostTimeWinning:
                    return (int)AchievementList[winner].timeWinning + " Sec";
                case AchievementType.MostCamper:
                    return (int)AchievementList[winner].timeStanding + " Sec";
                case AchievementType.MostMover:
                    return (int)AchievementList[winner].timeMoving + " Sec";
            }

            return "none";
        }

        public Player GetAchievementWinner(AchievementType type)
        {
            Player winner = null;
            int highestValue = 0;

            foreach (Player player in AchievementList.Keys)
            {
                switch (type)
                {
                    case AchievementType.MostDeaths:
                        if (AchievementList[player].respawnCount > highestValue)
                        {
                            highestValue = AchievementList[player].respawnCount;
                            winner = player;
                        }
                        break;
                    case AchievementType.MostKills:
                        if (AchievementList[player].pushCount > highestValue)
                        {
                            highestValue = AchievementList[player].pushCount;
                            winner = player;
                        }
                        break;
                    case AchievementType.MostCreaturesTamed:
                        if (AchievementList[player].creatureTurnCount > highestValue)
                        {
                            highestValue = AchievementList[player].creatureTurnCount;
                            winner = player;
                        }
                        break;
                    case AchievementType.MostTimeWinning:
                        if (AchievementList[player].timeWinning > highestValue)
                        {
                            highestValue = (int)AchievementList[player].timeWinning;
                            winner = player;
                        }
                        break;
                    case AchievementType.MostCamper:
                        if (AchievementList[player].timeStanding > highestValue)
                        {
                            highestValue = (int)AchievementList[player].timeStanding;
                            winner = player;
                        }
                        break;
                    case AchievementType.MostMover:
                        if (AchievementList[player].timeMoving > highestValue)
                        {
                            highestValue = (int)AchievementList[player].timeMoving;
                            winner = player;
                        }
                        break;
                }
            }

            return winner;
        }
    }
}
