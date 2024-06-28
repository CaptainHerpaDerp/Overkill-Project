using Players;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RespawnUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI respawnText;
    [SerializeField] private Image darkeningPanel;

    [Header("Fade Time of the Panel")]
    [SerializeField] private float fadeTime;

    [SerializeField] Player parentPlayer;

    private void Start()
    {
        parentPlayer.OnPlayerFall += ShowRespawnUI;
        parentPlayer.OnPlayerRespawn += HideRespawnUI;
    }

    public void ShowRespawnUI(int countdownTime)
    {
        respawnText.gameObject.SetActive(true);
        StartCoroutine(FadeInPanel());
        StartCoroutine(DoRespawnTextCountdown(countdownTime));
    }

    private IEnumerator FadeInPanel()
    {
        float time = 0;

        darkeningPanel.gameObject.SetActive(true);

        Color color = darkeningPanel.color;
        color.a = 0;
        darkeningPanel.color = color;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, time / fadeTime);
            color.a = alpha;
            darkeningPanel.color = color;
            yield return null;
        }

        yield return null;
    }

    private IEnumerator DoRespawnTextCountdown(int countdownFrom)
    {
        int time = 0;
        while (time < countdownFrom)
        {
            respawnText.text = "Respawning in " + (countdownFrom - time) + "...";
            time++;
            yield return new WaitForSeconds(1);
        }

        yield return null;
    }

    public void HideRespawnUI(object a, object b)
    {
        darkeningPanel.gameObject.SetActive(false);
        respawnText.gameObject.SetActive(false);
    }
}
