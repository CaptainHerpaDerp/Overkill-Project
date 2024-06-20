using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GameManagement;
using TeamColors;

namespace UIManagement
{

    public class PlayerScoreBar : MonoBehaviour
    {
        public int player1Score = 0, player2Score = 0, player3Score = 0, player4Score = 0;

        [SerializeField] private Image pp1, pp2, pp3, pp4;

        [SerializeField] private float startWidth, maxWidth;

        [SerializeField] private float barGrowSpeed;

        [SerializeField] private float updateInterval = 1f;

        private GameManager gameManager;

        private void Start()
        {
            gameManager = GameManager.Instance;

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
                player1Score = gameManager.GetScoreForPlayer(0);
                player2Score = gameManager.GetScoreForPlayer(1);
                player3Score = gameManager.GetScoreForPlayer(2);
                player4Score = gameManager.GetScoreForPlayer(3);

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
            float targetP1Width = 0, targetP2Width = 0, targetP3Width = 0, targetP4Width = 0;
            float p1Diff = 0, p2Diff = 0, p3Diff = 0, p4Diff = 0;


            if (totalScore == 0)
                targetP1Width = 0;
            else
                targetP1Width = (player1Score / totalScore) * maxWidth;

            p1Diff = Mathf.Abs(targetP1Width - pp1.rectTransform.sizeDelta.x);
            if (p1Diff <= barGrowSpeed) 
                pp1.rectTransform.sizeDelta = new Vector2(targetP1Width, pp1.rectTransform.sizeDelta.y);
            else  
                pp1.rectTransform.sizeDelta = new Vector2(pp1.rectTransform.sizeDelta.x + Mathf.Sign(targetP1Width - pp1.rectTransform.sizeDelta.x) * barGrowSpeed,
                    pp1.rectTransform.sizeDelta.y);

            pp1.rectTransform.position = new Vector3(startWidth, pp1.rectTransform.position.y, 
                pp1.rectTransform.position.z);


            if (totalScore == 0)
                targetP2Width = 0;
            else
                targetP2Width = (player2Score / totalScore) * maxWidth;

            p2Diff = Mathf.Abs(targetP2Width - pp2.rectTransform.sizeDelta.x);
            if (p2Diff <= barGrowSpeed)
                pp2.rectTransform.sizeDelta = new Vector2(targetP2Width, pp2.rectTransform.sizeDelta.y);
            else
                pp2.rectTransform.sizeDelta = new Vector2(pp2.rectTransform.sizeDelta.x + Mathf.Sign(targetP2Width - pp2.rectTransform.sizeDelta.x) * barGrowSpeed,
                    pp2.rectTransform.sizeDelta.y);

            pp2.rectTransform.position = new Vector3(pp1.rectTransform.position.x + pp1.rectTransform.sizeDelta.x, pp2.rectTransform.position.y, 
                pp2.rectTransform.position.z);


            if (totalScore == 0)
                targetP3Width = 0;
            else
                targetP3Width = (player3Score / totalScore) * maxWidth;

            p3Diff = Mathf.Abs(targetP3Width - pp3.rectTransform.sizeDelta.x);
            if (p3Diff <= barGrowSpeed)
                pp3.rectTransform.sizeDelta = new Vector2(targetP3Width, pp3.rectTransform.sizeDelta.y);
            else
                pp3.rectTransform.sizeDelta = new Vector2(pp3.rectTransform.sizeDelta.x + Mathf.Sign(targetP3Width - pp3.rectTransform.sizeDelta.x) * barGrowSpeed,
                    pp3.rectTransform.sizeDelta.y);

            pp3.rectTransform.position = new Vector3(pp2.rectTransform.position.x + pp2.rectTransform.sizeDelta.x, pp3.rectTransform.position.y, 
                pp3.rectTransform.position.z);


            if (totalScore == 0)
                targetP4Width = 0;
            else
                targetP4Width = (player4Score / totalScore) * maxWidth;

            p4Diff = Mathf.Abs(targetP4Width - pp4.rectTransform.sizeDelta.x);
            if (p4Diff <= barGrowSpeed)
                pp4.rectTransform.sizeDelta = new Vector2(targetP4Width, pp4.rectTransform.sizeDelta.y);
            else
                pp4.rectTransform.sizeDelta = new Vector2(pp4.rectTransform.sizeDelta.x + Mathf.Sign(targetP4Width - pp4.rectTransform.sizeDelta.x) * barGrowSpeed,
                    pp4.rectTransform.sizeDelta.y);

            pp4.rectTransform.position = new Vector3(pp3.rectTransform.position.x + pp3.rectTransform.sizeDelta.x, pp4.rectTransform.position.y, 
                pp4.rectTransform.position.z);
            
        }
    }
}