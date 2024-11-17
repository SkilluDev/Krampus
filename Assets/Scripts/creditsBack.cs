using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için

public class CreditsBack : MonoBehaviour
{
    public float creditsDuration = 19f; // Credits ekranının gösterilme süresi (saniye)
    public FadeTransition fadeTransition; // FadeTransition script referansı

    private void Start()
    {
        // Credits ekranının sonunda fade-out yapıp ana menüye dönmek için coroutine başlat
        StartCoroutine(GoToMainMenuAfterCredits());
    }

    private IEnumerator GoToMainMenuAfterCredits()
    {
        // Credits süresi boyunca bekleyin
        yield return new WaitForSeconds(creditsDuration);

        // Fade-out efekti ve sahne geçişini başlat
        fadeTransition.LoadSceneWithFade("UITest"); // "MainMenu" sahnesine geçiş yapın
    }
}