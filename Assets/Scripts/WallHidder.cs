using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class WallHidder : MonoBehaviour
{

    public List< MeshRenderer> wallsStored = new List<MeshRenderer>();
    [SerializeField] float disapearanceSpeed;
    [SerializeField] float minAlphaPrec = 50;
    [SerializeField] float range = 200f;




    private void Update()
    {
        RaycastHit hit;

        Vector3 fromPosition = transform.position;
        Vector3 direction = (Camera.main.transform.position - fromPosition).normalized;


        MeshRenderer wallHit = null;

        if (Physics.Raycast(transform.position, direction, out hit, range, LayerMask.GetMask("Wall")))
        {

            Debug.DrawRay(transform.position, direction, Color.yellow);
            wallHit = hit.transform.gameObject.GetComponent<MeshRenderer>();
            if (wallsStored.Contains(wallHit))
            {
                Color color = wallHit.materials[1].color;
                if (color.a > (minAlphaPrec / 100))
                {
                    color.a -= disapearanceSpeed / 100 * Time.deltaTime;
                    wallHit.materials[1].color = color;
                }
            }
            else
            {
                wallsStored.Add(wallHit);
            }
        }

            for(int i = 0; i < wallsStored.Count; i++)
            {
                MeshRenderer wall = wallsStored[i];

            
                if (wall != null)
                {
                    if (wallHit == wall) continue;
                }
                
                
                Color color = wall.materials[1].color;
                if (color.a < 1)
                {
                    color.a += disapearanceSpeed / 100 * Time.deltaTime;
                    wall.materials[1].color = color;
                }
                else 
                {
                    wallsStored.Remove(wall);
                    i--;
                }
                

            }
           

        }
        
       
    }

