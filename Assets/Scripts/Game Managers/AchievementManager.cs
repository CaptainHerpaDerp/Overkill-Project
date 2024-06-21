using static TeamColors.ColorEnum;
using UnityEngine;
using Players;
using TeamColors;
using System.Collections.Generic;
using System.Linq;
using Creatures;

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
                var (mostMovePlayer, moveTime) = GameManager.Instance.GetMostMovePlayer();
                AchievementList[mostMovePlayer].timeMoving = moveTime;

                var (leastMovePlayer, campTime) = GameManager.Instance.GetLeastMovePlayer();
                AchievementList[leastMovePlayer].timeStanding = campTime;

                var (mostWinPlayer, winTime) = GameManager.Instance.GetMostWinPlayerWinner();
                AchievementList[mostWinPlayer].timeWinning = winTime;
            };

            CreatureManager.OnConvert += ProcessCreatureColorTurn;
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
                {
                    //print($"asshole: {AchievementList[player].creatureTurnCount}, colour: {newColor}");
                    AchievementList[player].creatureTurnCount++;
                }
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
            int value = 0;
            string stringAddition = "";

            switch (type)
            {
                case AchievementType.MostDeaths:
                    value = AchievementList[winner].respawnCount;
                    break;
                case AchievementType.MostKills:
                    value = AchievementList[winner].pushCount;
                    break;
                case AchievementType.MostCreaturesTamed:
                    value = AchievementList[winner].creatureTurnCount;
                    break;
                case AchievementType.MostTimeWinning:
                    value = (int)AchievementList[winner].timeWinning;
                    stringAddition = " Sec";
                    break;
                case AchievementType.MostCamper:
                    value = (int)AchievementList[winner].timeStanding;
                    stringAddition = " Sec";
                    break;
                case AchievementType.MostMover:
                    value = (int)AchievementList[winner].timeMoving;
                    stringAddition = " Sec";
                    break;
            }

            if (value == 0)
            {
                return "none";
            }
            else
            {
                return value.ToString() + stringAddition;
            }
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
