using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementAnimationBan : MonoBehaviour
{
    characterController krampusController;

    Animation anim;
    string[] bannedAnimName;

    void Start()
    {
        krampusController = GetComponentInParent<characterController>();

        anim = GetComponent<Animation>();
    }

    void Update()
    {
        int howManyPlaying = 0;
        foreach (string animName in bannedAnimName)
        {
            if (anim.IsPlaying(animName)) { 
                howManyPlaying++;
            }

        }
        if (howManyPlaying > 0)
        {
            krampusController.shouldKrampusMove = false;
        }
        else { krampusController.shouldKrampusMove = true; }
    }
}
