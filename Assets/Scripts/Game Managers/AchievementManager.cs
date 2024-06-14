using static TeamColors.ColorEnum;
using UnityEngine;
using Players;
using TeamColors;
using System.Collections.Generic;
using System.Collections;
using System;

namespace GameManagement { 

    public class AchievementManager : MonoBehaviour
    {
        public Dictionary<Player, Achievement> achievementList = new Dictionary<Player, Achievement>(); 
        
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

        public static AchievementManager Instance;

        // Start is called before the first frame update
        void Start()
        {
            GameManager.Instance.OnGameReload += ResetAchievements;
            PlayerManager.Instance.OnPlayersSelected += StartAchievements;
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void ResetAchievements() {
            achievementList.Clear();
            achievementList = new Dictionary<Player, Achievement>();

            foreach (Player player in GameManager.Instance.PlayerList) {
                player.OnPlayerRespawn -= ProcessRespawn;
            }

            foreach (KeyValuePair<Transform, ColorEnum.TEAMCOLOR> kvp in AnimalLocator.Instance.animalTransformPairs)
            {
                AnimalLocator.Instance.OnCreatureColorChanged -= ProcessCreatureColorTurn;
            }
        }

        private void StartAchievements() { 
            foreach(Player player in GameManager.Instance.PlayerList) {
                achievementList[player] = new Achievement();

                player.OnPlayerRespawn += ProcessRespawn;
            }

            foreach (KeyValuePair<Transform, ColorEnum.TEAMCOLOR> kvp in AnimalLocator.Instance.animalTransformPairs) {
                AnimalLocator.Instance.OnCreatureColorChanged += ProcessCreatureColorTurn;
            }

        }

        private void ProcessRespawn(Player pusherPlayer, Player respawnedPlayer) {
            achievementList[pusherPlayer].pushCount++;
            achievementList[respawnedPlayer].respawnCount++;
        }

        private void ProcessCreatureColorTurn(TEAMCOLOR newColor) {
            foreach (Player player in achievementList.Keys) {
                if (player.TeamColor == newColor)
                    achievementList[player].creatureTurnCount++;
            }
        }


    }
}
