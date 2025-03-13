using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WallHindingFire : MonoBehaviour {

    [FormerlySerializedAs("walls")][SerializeField] private MeshRenderer[] walls;

    public bool playerIsInsideTheRoom = false;
    public float globalAlpha = 1.0f;

    [Header("Settings")]
    [FormerlySerializedAs("minAlpha")][SerializeField] private float minAlpha = 0.33f;
    [FormerlySerializedAs("m_alphaSpeed")][SerializeField] private float m_alphaSpeed = 100f;


    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            playerIsInsideTheRoom = false;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player") {
            playerIsInsideTheRoom = true;

        }
    }
    private void Update() {
        if (!playerIsInsideTheRoom) {
            if (globalAlpha < 1) {
                globalAlpha += (m_alphaSpeed / 100) * Time.deltaTime;
                foreach (MeshRenderer wall in walls) {
                    Color color = wall.materials[1].color;

                    {
                        color.a = globalAlpha;
                        wall.materials[1].color = color;
                    }
                }
            }
        } else {
            if (globalAlpha > minAlpha) {
                globalAlpha -= (m_alphaSpeed / 100) * Time.deltaTime;
                foreach (MeshRenderer wall in walls) {
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
