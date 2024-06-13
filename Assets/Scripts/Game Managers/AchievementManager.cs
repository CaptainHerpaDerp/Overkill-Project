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
        public Dictionary<Player, int> pushCount = new Dictionary<Player, int>(); 
        public Dictionary<Player, int> respawnCount = new Dictionary<Player, int>();

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
            foreach (KeyValuePair<Player, int> kvp in pushCount)
            {
                Debug.Log($"Key: {kvp.Key.TeamColor}, Value: {kvp.Value}");
            }
        }

        private void ResetAchievements() {
            pushCount.Clear();
            pushCount = new Dictionary<Player, int>();

            respawnCount.Clear();
            respawnCount = new Dictionary<Player, int>();
        }

        private void StartAchievements() { 
            foreach(Player player in GameManager.Instance.PlayerList) {
                pushCount[player] = 0;
                respawnCount[player] = 0;

                player.OnPlayerRespawn += ProcessRespawn;
            }   


        }

        private void ProcessRespawn(Player pusherPlayer, Player respawnedPlayer) {
            pushCount[respawnedPlayer]++;
            pushCount[pusherPlayer]++;
        }

    }
}
