using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreBar : MonoBehaviour
{
    public int player1Score = 0, player2Score = 0, player3Score = 0, player4Score = 0;

    [SerializeField] private Image pp1, pp2, pp3, pp4;

    [SerializeField] private float startWidth, maxWidth;

    [SerializeField] private float updateInterval = 1f;

    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = ScoreManager.Instance;

        StartCoroutine(UpdateScores());
    }

    private void Update()
    {
        RecalculateSizes();
    }

    private IEnumerator UpdateScores()
    {
        while (true)
        {
            player1Score = scoreManager.GetScoreForPlayer(0);
            player2Score = scoreManager.GetScoreForPlayer(1);
            player3Score = scoreManager.GetScoreForPlayer(2);
            player4Score = scoreManager.GetScoreForPlayer(3);

            RecalculateSizes();

            yield return new WaitForSeconds(updateInterval);
        }
    }


    /// <summary>
    /// Have all four images fill up to the same width with respect to their score, there should be no overlap
    /// </summary>
    private void RecalculateSizes()
    {
        float totalScore = player1Score + player2Score + player3Score + player4Score;

        float p1Width = 0, p2Width = 0, p3Width = 0, p4Width = 0;

        if (player1Score > 0)
        {
            p1Width = (player1Score / totalScore) * maxWidth;
            pp1.rectTransform.sizeDelta = new Vector2(p1Width, pp1.rectTransform.sizeDelta.y);
            pp1.rectTransform.position = new Vector3(startWidth, pp1.rectTransform.position.y, pp1.rectTransform.position.z);
        }

        if (player2Score > 0)
        {
            p2Width = (player2Score / totalScore) * maxWidth;
            pp2.rectTransform.sizeDelta = new Vector2(p2Width, pp2.rectTransform.sizeDelta.y);
            pp2.rectTransform.position = new Vector3(pp1.rectTransform.position.x + p1Width, pp2.rectTransform.position.y, pp2.rectTransform.position.z);
        }

        if (player3Score > 0)
        {
            p3Width = (player3Score / totalScore) * maxWidth;
            pp3.rectTransform.sizeDelta = new Vector2(p3Width, pp3.rectTransform.sizeDelta.y);
            pp3.rectTransform.position = new Vector3(pp2.rectTransform.position.x + p2Width, pp3.rectTransform.position.y, pp3.rectTransform.position.z);
        }

        if (player4Score > 0)
        {
            p4Width = (player4Score / totalScore) * maxWidth;
            pp4.rectTransform.sizeDelta = new Vector2(p4Width, pp4.rectTransform.sizeDelta.y);
            pp4.rectTransform.position = new Vector3(pp3.rectTransform.position.x + p3Width, pp4.rectTransform.position.y, pp4.rectTransform.position.z);
        }
    }
}
