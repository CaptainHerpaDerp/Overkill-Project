using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

/// <summary>
/// Class that controls the character screen saver, where each character is introduced 
/// </summary>
public class CharacterScreenSaver : MonoBehaviour
{
    [SerializeField] private GameObject redChar, greenChar, blueChar, purpleChar;

    [SerializeField] private int selectedCharacter = 0;
    [SerializeField] private float waitTime = 5f;

    [Header("The character will fall from the top of the screen onto a position, and then fall again to the bottom of the screen")]
    [SerializeField] private float startY;
    [SerializeField] private float stopY, endY;

    [Header("The falling time of the character from the start-stop and stop-end points")]
    [SerializeField] private float incrementFallTime;

    private List<GameObject> characters;

    private void Start()
    {
        // Null checks
        if (redChar == null || greenChar == null || blueChar == null || purpleChar == null)
        {
            Debug.LogError("One or more characters are missing");
            return;
        }

        // Create a list and add the characters
        characters = new List<GameObject> { redChar, greenChar, blueChar, purpleChar };

        // Distable all characters
        foreach (var character in characters)
        {
            character.SetActive(false);
        }

        characters[selectedCharacter].SetActive(true);
        characters[selectedCharacter].transform.GetChild(0).gameObject.SetActive(true);

        StartCoroutine(LoopThroughCharacters());
    }

    private IEnumerator LoopThroughCharacters()
    {
        while (true)
        {
            // Get the next character
            GameObject newChar = characters[selectedCharacter];
            Animator characterAnimator = characters[selectedCharacter].GetComponentInChildren<Animator>();

            // Set the character's position to the start position
            Vector3 localCharPos = newChar.transform.localPosition;
            localCharPos = new Vector3(localCharPos.x, startY, localCharPos.z);
            newChar.transform.localPosition = localCharPos;
            newChar.SetActive(true);

            // Make the character fall
            characterAnimator.SetBool("Grounded", false);

            float time = 0;
            float posY = startY;
            // Make the character fall smoothly to the stop position
            while (time <= incrementFallTime)
            {
                time += Time.deltaTime;

                posY = Mathf.Lerp(startY, stopY, time / incrementFallTime);
                localCharPos = new Vector3(localCharPos.x, posY, localCharPos.z);
                newChar.transform.localPosition = localCharPos;

                yield return null;
            }

            characterAnimator.SetBool("Grounded", true);

            yield return new WaitForSeconds(waitTime);

            characterAnimator.SetBool("Grounded", false);

            time = 0;
            posY = stopY;
            // Make the character fall smoothly to the stop position
            while (time <= incrementFallTime)
            {
                time += Time.deltaTime;

                posY = Mathf.Lerp(stopY, endY, time / incrementFallTime);
                localCharPos = new Vector3(localCharPos.x, posY, localCharPos.z);
                newChar.transform.localPosition = localCharPos;

                yield return null;
            }

        }

        yield break;
    }
}
