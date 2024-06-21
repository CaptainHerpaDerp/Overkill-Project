using Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeamColors;

namespace UIElements
{
    public class CooldownUI : MonoBehaviour
    {
        //[SerializeField] private Image triggerImage;
        //[SerializeField] private float triggerTime = 1f;
        //[SerializeField] private Sprite defaultTrigger, pressedTrigger;

        //[SerializeField] private TMPro.TextMeshProUGUI cooldownText;

        [SerializeField] private Transform specialAbilityParent;

        [Header("Player Cooldown Groups, order from Red to Purple")]
        [SerializeField] private List<Transform> playerCooldownGroups = new();

        [SerializeField] private Player parentPlayer;

        private ColorEnum.TEAMCOLOR teamColour;

        private Image backgroundImage, fillImage, readyImage;
        [SerializeField] private float readyImageFadeSpeed = 0.1f, readyImageFadeTime = 0.5f;

        // The maximum fill amount of the cooldown bar
        private float targetFillAmount;

        public void Start()
        {
            parentPlayer.OnPlayerStart += () =>
            {
                teamColour = parentPlayer.TeamColor;

                // Get the team index based on the team colour
                int teamIndex = (int)teamColour;

                // Select the corresponding cooldown group based on the team index
                Transform cooldownGroup = playerCooldownGroups[teamIndex];

                if (cooldownGroup == null)
                {
                    Debug.LogError("CooldownUI: Cooldown group not found for the player!");
                }

                cooldownGroup.gameObject.SetActive(true);

                // Assuiming the child order of the group is: Background, Fill, Ready

                backgroundImage = cooldownGroup.GetChild(0).GetComponent<Image>();
                fillImage = cooldownGroup.GetChild(1).GetComponent<Image>();
                readyImage = cooldownGroup.GetChild(2).GetComponent<Image>();

                if (backgroundImage == null || fillImage == null || readyImage == null)
                {
                    Debug.LogError("CooldownUI: Background, Fill or Ready image not found in the cooldown group!");
                }

                // Set the target fill amount to the height of the fill image
                targetFillAmount = fillImage.rectTransform.rect.height;

                UseAbilityPromptLoop();
            };
        }

        private void OnEnable()
        {
            foreach (Transform child in specialAbilityParent)
            {
                if (child.TryGetComponent(out SpecialBehaviour specialBehaviour))
                {
                    specialBehaviour.OnSpecialAbilityRefreshed += () => UseAbilityPromptLoop();
                    specialBehaviour.OnSpecialTriggered += (time) => CooldownLoop(time);
                }
            }
        }

        private void UseAbilityPromptLoop()
        {
            StopAllCoroutines();
            StartCoroutine(AbilityPromptLoop());
        }

        private void CooldownLoop(float time)
        {
            print("cooldown loop");
            StopAllCoroutines();

            // The ability is on cooldown, so hide the ready image
            readyImage.enabled = false;

            StartCoroutine(DoCooldownLoop(time));
        }

        private IEnumerator AbilityPromptLoop()
        {
            readyImage.enabled = true;

            // Set the alpha of the ready image to 0
            Color color = readyImage.color;
            color.a = 0;
            readyImage.color = color;

            // Constantly fade the ready image in and out
            while (true)
            {
                yield return new WaitForSeconds(readyImageFadeTime);

                for (float i = 0; i <= 1; i += readyImageFadeSpeed)
                {
                    color.a = i;
                    readyImage.color = color;
                    yield return new WaitForSeconds(readyImageFadeSpeed);
                }

                for (float i = 1; i >= 0; i -= readyImageFadeSpeed)
                {
                    color.a = i;
                    readyImage.color = color;
                    yield return new WaitForSeconds(readyImageFadeSpeed);
                }
            }
        }

        private IEnumerator DoCooldownLoop(float fillTime)
        {
            print("Filling with time: " + fillTime);

            // Set the fill image's height to 0
            fillImage.rectTransform.sizeDelta = new Vector2(fillImage.rectTransform.sizeDelta.x, 0);

            float currentHeight;
            float timeFactor;
            float time = 0;

            // Increase the fill bar from 0 to the target fill amount over the fill time
            while (time < fillTime)
            {
                timeFactor = time / fillTime;

                currentHeight = Mathf.Lerp(0, targetFillAmount, timeFactor);
                fillImage.rectTransform.sizeDelta = new Vector2(fillImage.rectTransform.sizeDelta.x, currentHeight);
                time += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            yield break;
        }
    }
}