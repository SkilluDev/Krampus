using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimationBan : MonoBehaviour
{
    private KrampusController m_krampusController;

    private Animation m_anim;
    private string[] m_bannedAnimName;

    private void Start()
    {
        m_krampusController = GetComponentInParent<KrampusController>();

        m_anim = GetComponent<Animation>();
    }

    private void Update()
    {
        int howManyPlaying = 0;
        foreach (string animName in m_bannedAnimName)
        {
            if (m_anim.IsPlaying(animName))
            {
                howManyPlaying++;
            }

        }
        if (howManyPlaying > 0)
        {
            m_krampusController.shouldKrampusMove = false;
        }
        else { m_krampusController.shouldKrampusMove = true; }
    }
}
