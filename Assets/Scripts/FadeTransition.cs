using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeTransition : MonoBehaviour
{
    public Image fadeImage; // The UI Image used for fade effect
    public float fadeDuration = 1.0f; // Duration of the fade effect in seconds

    private void Start()
    {
        // Fade in when the scene starts
        if (fadeImage != null) // Check if fadeImage is assigned
        {
            StartCoroutine(FadeIn());
        }
        else
        {
            Debug.LogError("fadeImage is not assigned in the inspector!");
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        // Trigger fade out and load the next scene
        if (fadeImage != null)
        {
            StartCoroutine(FadeOut(sceneName));
        }
        else
        {
            Debug.LogError("fadeImage is not assigned in the inspector!");
        }
    }

    private IEnumerator FadeIn()
    {
        Color color = fadeImage.color;
        color.a = 1;
        fadeImage.color = color;

        for (float t = fadeDuration; t > 0; t -= Time.deltaTime)
        {
            color.a = t / fadeDuration;
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0;
        fadeImage.color = color; // Ensure it's fully transparent
    }

    private IEnumerator FadeOut(string sceneName)
    {
        Color color = fadeImage.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = t / fadeDuration;
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1;
        fadeImage.color = color; // Ensure it's fully opaque

        // Load the specified scene
        SceneManager.LoadScene(sceneName);
    }
}