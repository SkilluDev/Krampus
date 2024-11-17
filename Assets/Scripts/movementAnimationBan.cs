using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementAnimationBan : MonoBehaviour
{
    private GameObject krampus;
    characterController krampusController;

    Animation anim;
    string[] bannedAnimName;

    void Start()
    {
        krampus = GameObject.FindWithTag("Player");
        krampusController = krampus.GetComponent<characterController>();

        anim = GetComponent<Animation>();
    }

    void Update()
    {
        foreach (string animName in bannedAnimName) {
            krampusController.shouldKrampusMove = !(anim.IsPlaying(animName)); 
        }
    }
}
