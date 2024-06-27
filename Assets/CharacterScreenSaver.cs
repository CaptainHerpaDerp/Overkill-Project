using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;
using TMPro;

/// <summary>
/// Class that controls the character screen saver, where each character is introduced 
/// </summary>
public class CharacterScreenSaver : MonoBehaviour
{
    [SerializeField] private GameObject redChar, greenChar, blueChar, purpleChar;

    [SerializeField] TextMeshProUGUI characterDescription;
    [SerializeField] TextMeshProUGUI characterName;

    [SerializeField] private int selectedCharacter = 0;
    [SerializeField] private float waitTime = 5f;

    [Header("The character will fall from the top of the screen onto a position, and then fall again to the bottom of the screen")]
    [SerializeField] private float startY;
    [SerializeField] private float stopY, endY;

    [Header("The falling time of the character from the start-stop and stop-end points")]
    [SerializeField] private float incrementFallTime;

    [Header("The descriptions of the characters in order (red-purple)")]
    [SerializeField] private List<string> characterDescriptions = new();
    [SerializeField] private float descriptionFadeTime = 1f;
    [SerializeField] private float descriptionWaitTime = 5f;

    [Header("The names of the characters in order (red-purple)")]
    [SerializeField] private List<string> characterNames = new();

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

        StartCoroutine(FadeOutInText(characterDescription, characterDescriptions[selectedCharacter], descriptionFadeTime));
        StartCoroutine(FadeOutInText(characterName, characterNames[selectedCharacter], descriptionFadeTime));

        StartCoroutine(LoopThroughCharacters());
    }

    private IEnumerator LoopThroughCharacters()
    {
        while (true)
        {
            // Get the next character
            GameObject newChar = characters[selectedCharacter];
            Animator[] characterAnimator = characters[selectedCharacter].GetComponentsInChildren<Animator>();

            // Set the character's position to the start position
            Vector3 localCharPos = newChar.transform.localPosition;
            localCharPos = new Vector3(localCharPos.x, startY, localCharPos.z);
            newChar.transform.localPosition = localCharPos;
            newChar.SetActive(true);

            // Make the character fall
            foreach (var animator in characterAnimator)
            {
                animator.SetBool("Grounded", false);
            }

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

            foreach (var animator in characterAnimator)
            {
                animator.SetBool("Grounded", true);
            }

            yield return new WaitForSeconds(waitTime);

            foreach (var animator in characterAnimator)
            {
                animator.SetBool("Grounded", false);
            }   

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


            // Disable the character
            newChar.SetActive(false);

            // Once the character has fallen out of the screen, repeat the process with the next character
            selectedCharacter = (selectedCharacter + 1) % characters.Count;

            StartCoroutine(FadeOutInText(characterDescription, characterDescriptions[selectedCharacter], descriptionFadeTime));
            StartCoroutine(FadeOutInText(characterName, characterNames[selectedCharacter], descriptionFadeTime));
        }
    }

    private IEnumerator FadeOutInText(TextMeshProUGUI textComponent, string text, float time)
    {
        textComponent.text = text;
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 0);

        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime / time;
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(descriptionWaitTime);

        while (alpha > 0)
        {
            alpha -= Time.deltaTime / time;
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, alpha);
            yield return null;
        }

        yield return null;
    }
}
