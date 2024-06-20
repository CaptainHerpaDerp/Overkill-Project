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

        public void Start()
        {
            parentPlayer = transform.parent.GetComponent<Player>();
            parentPlayer.OnPlayerStart += () => 
            {
                teamColour = parentPlayer.TeamColor;

                // Get the team index based on the team colour
                int teamIndex = (int)teamColour;

                // Select the corresponding cooldown group based on the team index
                Transform cooldownGroup = playerCooldownGroups[teamIndex];

                // Assuiming the child order of the group is: Background, Fill, Ready

                backgroundImage = cooldownGroup.GetChild(0).GetComponent<Image>();
                fillImage = cooldownGroup.GetChild(1).GetComponent<Image>();
                readyImage = cooldownGroup.GetChild(2).GetComponent<Image>();

                if (backgroundImage == null || fillImage == null || readyImage == null)
                {
                    Debug.LogError("CooldownUI: Background, Fill or Ready image not found in the cooldown group!");
                }

                //TriggerLoop();
            };
        }

        private void OnEnable()
        {
            foreach (Transform child in specialAbilityParent)
            {
                if (child.TryGetComponent(out SpecialBehaviour specialBehaviour))
                {
                    //specialBehaviour.OnSpecialAbilityRefreshed += () => TriggerLoop();
                    //specialBehaviour.OnSpecialTriggered += (time) => CooldownLoop(time);
                }
            }
        }

        //private void TriggerLoop()
        //{
        //    StopAllCoroutines();

        //    cooldownText.enabled = false;
        //    triggerImage.enabled = true;

        //    StartCoroutine(DoTriggerLoop());
        //}

        //private void CooldownLoop(float time)
        //{
        //    print("cooldown loop");

        //    StopAllCoroutines();

        //    cooldownText.enabled = true;
        //    triggerImage.enabled = false;

        //    cooldownText.text = time.ToString("F0");

        //    StartCoroutine(DoCooldownLoop());
        //}

        //private IEnumerator DoTriggerLoop()
        //{
        //    while (true)
        //    {
        //        yield return new WaitForSeconds(triggerTime);
        //        triggerImage.sprite = pressedTrigger;
        //        yield return new WaitForSeconds(triggerTime);
        //        triggerImage.sprite = defaultTrigger;
        //    }
        //}

        //private IEnumerator DoCooldownLoop()
        //{
        //    while (true)
        //    {
        //        yield return new WaitForSeconds(1f);
        //        cooldownText.text = (int.Parse(cooldownText.text) - 1).ToString();
        //    }
        //}
    }
}