using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static AudioClip windup1, windup2, step1, step2, kill, caught, tongue;
    private static AudioSource src;

    private void Start() {
        src = GetComponent<AudioSource>();
        windup1 = Resources.Load<AudioClip>("Clips/windup1");
        windup2 = Resources.Load<AudioClip>("Clips/windup2");
        step1 = Resources.Load<AudioClip>("Clips/step_1");
        step2 = Resources.Load<AudioClip>("Clips/step_2");
        kill = Resources.Load<AudioClip>("Clips/kill1");
        caught = Resources.Load<AudioClip>("Clips/catch");
        tongue = Resources.Load<AudioClip>("Clips/tongue");
    }

    public static void PlaySound(string clipName) {
        switch (clipName) {
            case "windup1":
                src.PlayOneShot(windup1);
                break;
            case "windup2":
                src.PlayOneShot(windup2);
                break;
            case "step1":
                src.PlayOneShot(step1);
                break;
            case "step2":
                src.PlayOneShot(step2);
                break;
            case "kill":
                src.PlayOneShot(kill);
                break;
            case "catch":
                src.PlayOneShot(caught);
                break;
            case "tongue":
                src.PlayOneShot(tongue);
                break;
        }
    }
}
