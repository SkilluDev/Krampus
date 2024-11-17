using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static AudioClip windup1, windup2;
    static AudioSource src;

    private void Start()
    {
        src = GetComponent<AudioSource>();
        windup1 = Resources.Load<AudioClip>("Clips/windup1");
    }

    public static void PlaySound(string clipName)
    {
        switch (clipName)
        {
            case "windup1":
                src.PlayOneShot(windup1);
                break;
        }
    }
}
