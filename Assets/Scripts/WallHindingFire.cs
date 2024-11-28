using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHindingFire : MonoBehaviour
{

   [SerializeField] MeshRenderer[] walls;

    public bool playerIsInsideTheRoom = false;
    public float globalAlpha = 1.0f;

    [Header("Settings")]
    [SerializeField] float minAlpha = 0.33f;
    [SerializeField] float alphaSpeed = 100f;

   
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            playerIsInsideTheRoom = false;
           
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerIsInsideTheRoom = true;

        }
    }
    private void Update()
    {
        if (!playerIsInsideTheRoom)
        {
            if (globalAlpha < 1)
            {
                globalAlpha += (alphaSpeed/100) * Time.deltaTime;
                foreach (MeshRenderer wall in walls)
                {
                    Color color = wall.materials[1].color;

                    {
                        color.a = globalAlpha;
                        wall.materials[1].color = color;
                    }
                }
            }
        }
        else 
        {
            if (globalAlpha > minAlpha)
            {
                globalAlpha -= (alphaSpeed / 100) * Time.deltaTime;
                foreach (MeshRenderer wall in walls)
                {
                    Color color = wall.materials[1].color;

                    {
                        color.a = globalAlpha;
                        wall.materials[1].color = color;
                    }
                }
            }
        }
    }
}
