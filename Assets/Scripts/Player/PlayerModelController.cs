using System.Collections.Generic;
using UnityEngine;

namespace Players
{
    public enum AnimationState
    {
        Walk,
        Idle,
        Voice,
        Special        
    }

    public class PlayerModelController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> playerModels = new List<GameObject>();
        private GameObject modelInstance;
        private Animator currentModelAnimator;

        // Animator State Names
        private const string WALK = "Walk", IDLE = "Idle";

        /// <summary>
        /// Set the player model to the corresponding player index
        /// </summary>
        /// <param name="playerIndex"></param>
        public void SetPlayerModel(int playerIndex)
        {
            if (modelInstance != null)
            {
                Destroy(modelInstance);
            }

            modelInstance = Instantiate(playerModels[playerIndex], transform);
            currentModelAnimator = modelInstance.GetComponent<Animator>();
        }

        public void PlayAnimation(AnimationState animation)
        {
            if (currentModelAnimator == null)
            {
                Debug.LogWarning("No model animator assigned!");
                return;
            }

            switch (animation)
            {
                case AnimationState.Walk:
                    currentModelAnimator.Play(WALK);
                    break;
                case AnimationState.Idle:
                    currentModelAnimator.Play(IDLE);                  
                    break;
                case AnimationState.Voice:
                    break;
                case AnimationState.Special:
                    break;
            }
        }


    }
}