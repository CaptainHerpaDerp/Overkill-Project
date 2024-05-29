using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField] private Image triggerImage;
    [SerializeField] private float triggerTime = 1f;
    [SerializeField] private Sprite defaultTrigger, pressedTrigger;

    [SerializeField] private TMPro.TextMeshProUGUI cooldownText;

    [SerializeField] private Transform specialAbilityParent;

    private void OnEnable()
    {
        foreach (Transform child in specialAbilityParent)
        {
            if (child.TryGetComponent(out SpecialBehaviour specialBehaviour))
            {
                specialBehaviour.OnSpecialAbilityRefreshed += () => TriggerLoop();
                specialBehaviour.OnSpecialTriggered += (time) => CooldownLoop(time);
            }
        }
    }

    private void Start()
    {
        TriggerLoop();
    }

    private void TriggerLoop()
    {
        StopAllCoroutines();

        cooldownText.enabled = false;
        triggerImage.enabled = true;

        StartCoroutine(DoTriggerLoop());
    }

    private void CooldownLoop(float time)
    {
        StopAllCoroutines();

        cooldownText.enabled = true;
        triggerImage.enabled = false;

        cooldownText.text = time.ToString("F0");

        StartCoroutine(DoCooldownLoop());
    }

    private IEnumerator DoTriggerLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(triggerTime);
            triggerImage.sprite = pressedTrigger;
            yield return new WaitForSeconds(triggerTime);
            triggerImage.sprite = defaultTrigger;
        }
    }

    private IEnumerator DoCooldownLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            cooldownText.text = (int.Parse(cooldownText.text) - 1).ToString();
        }
    }
}
