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
        private Animator[] greenModelAnimators;

        private Animator currentModelAnimator;

        [Header("Since the green character is comprised of 2 characters (2 animators) track its index in the list so that different logic can be applied")]
        [SerializeField] private int greenCharacterIndex;
        private bool isGreen = false;

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

            if (playerIndex == greenCharacterIndex)
            {
                isGreen = true;
            }

            if (isGreen)
            {
                greenModelAnimators = modelInstance.GetComponentsInChildren<Animator>();
            }

            currentModelAnimator = modelInstance.GetComponent<Animator>();
        }

        public void SetMagnitude(float value)
        {
            if (isGreen)
            {
                foreach (Animator animator in greenModelAnimators)
                {
                    if (animator != null)
                    {
                        animator.SetFloat("Magnitude", value);
                    }
                    else
                    {
                        Debug.LogWarning("No green model animator assigned!");
                    }
                }
            }
            else
            {
                if (currentModelAnimator != null)
                {
                    currentModelAnimator.SetFloat("Magnitude", value);
                }
            }
        }

        public void SetGrounded(bool isGrounded)
        {
            if (isGreen)
            {
                foreach (Animator animator in greenModelAnimators)
                {
                    if (animator != null)
                    {
                        animator.SetBool("Grounded", isGrounded);
                    }
                    else
                    {
                        Debug.LogWarning("No green model animator assigned!");
                    }
                }
            }
            else
            {
                if (currentModelAnimator != null)
                {
                    currentModelAnimator.SetBool("Grounded", isGrounded);
                }
            }
        }

        public void SetPushing(bool isPushing)
        {
            if (isGreen)
            {
                foreach (Animator animator in greenModelAnimators)
                {
                    if (animator != null)
                    {
                        animator.SetBool("Pushing", isPushing);
                    }
                    else
                    {
                        Debug.LogWarning("No green model animator assigned!");
                    }
                }
            }
            else
            {
                if (currentModelAnimator != null)
                {
                    currentModelAnimator.SetBool("Pushing", isPushing);
                }
            }
        }

        public void PlayAnimation(AnimationState animation)
        {
            if (isGreen)
            {
                foreach (Animator animator in greenModelAnimators)
                {
                    if (animator != null)
                    {
                        switch (animation)
                        {
                            case AnimationState.Special:
                                animator.SetTrigger(SPECIAL);
                                break;
                            case AnimationState.Jump:
                                animator.SetTrigger(JUMP);
                                break;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No green model animator assigned!");
                    }
                }
            }
            else
            {
                if (currentModelAnimator == null)
                {
                    Debug.LogWarning("No model animator assigned!");
                    return;
                }

                switch (animation)
                {
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
}