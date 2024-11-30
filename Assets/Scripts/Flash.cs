using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Flash : MonoBehaviour
{
    // Start is called before the first frame update
    CanvasGroup canvasGroup;
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(Flashing());
    }
    
    IEnumerator Flashing()
    {
        float startTime;
        while (true)
        {
            canvasGroup.DOFade(1f,1f).SetEase(Ease.InOutCubic).SetUpdate(true);
            startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup-startTime < 2f) {
                yield return null;
            }
            canvasGroup.DOFade(0f,1f).SetEase(Ease.InOutCubic).SetUpdate(true);
            startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup-startTime < 2f) {
                yield return null;
            }
        }
    }
    
    IEnumerator WaitForRealSeconds (float seconds) {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup-startTime < seconds) {
            yield return null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
