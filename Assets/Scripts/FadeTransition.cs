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
        StartCoroutine(FadeIn());
    }

    public void LoadSceneWithFade(string sceneName)
    {
        // Trigger fade out and load the next scene
        StartCoroutine(FadeOut(sceneName));
    }

    private IEnumerator FadeIn()
    {
        // Gradually decrease the alpha of the image to make it transparent
        Color color = fadeImage.color;
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
        // Gradually increase the alpha of the image to make it opaque
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