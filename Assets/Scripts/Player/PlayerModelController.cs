using System.Collections.Generic;
using UnityEngine;

namespace Players
{
    public enum AnimationState
    {
        Walk,
        Idle,
        Voice,
        Special,
        Jump
    }

    public class PlayerModelController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> playerModels = new List<GameObject>();
        private GameObject modelInstance;
        private Animator currentModelAnimator;

        // Animator State Names
        private const string WALK = "Walk", IDLE = "Idle", SPECIAL = "Special", JUMP = "Jump";

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

        public void SetMagnitude(float value)
        {
            if (currentModelAnimator != null)
            {
                currentModelAnimator.SetFloat("Magnitude", value);
            }
        }

        public void SetGrounded(bool isGrounded)
        {
            if (currentModelAnimator != null)
            {
                currentModelAnimator.SetBool("Grounded", isGrounded);
            }
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
                    currentModelAnimator.SetTrigger(WALK);
                    break;
                case AnimationState.Idle:
                    currentModelAnimator.SetTrigger(IDLE);
                    break;
                case AnimationState.Voice:
                    break;
                case AnimationState.Special:
                    currentModelAnimator.SetTrigger(SPECIAL);
                    break;
                case AnimationState.Jump:
                    currentModelAnimator.SetTrigger(JUMP);
                    break;
            }
        }
    }
}