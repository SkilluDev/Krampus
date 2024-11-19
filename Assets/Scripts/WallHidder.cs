using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class WallHidder : MonoBehaviour
{

    public MeshRenderer wallStored;
    [SerializeField] float disapearanceSpeed;
    [SerializeField] float minAlphaPrec = 50;
    [SerializeField] float range = 200f;




    private void Update()
    {
        RaycastHit hit;

        Vector3 fromPosition = transform.position;
        Vector3 direction = (Camera.main.transform.position - fromPosition).normalized;




        if (Physics.Raycast(transform.position, direction, out hit, range, LayerMask.GetMask("Wall")))
        {
           
            Debug.DrawRay(transform.position, direction, Color.yellow);
            MeshRenderer wall = hit.transform.gameObject.GetComponent<MeshRenderer>();
            if (wall != null)
            {
                if(wallStored == null) { wallStored = wall; }
                Debug.Log(wall);
                if (wall != wallStored )
                {
                    Color color2 = wallStored.materials[1].color;
                    color2.a = 1;
                    wallStored.materials[1].color = color2;
                    wallStored = wall;
                    
                }

                Color color = wallStored.materials[1].color;
                Debug.Log (color.a);
                if (color.a > (minAlphaPrec/100))
                {
                    color.a -= disapearanceSpeed/100 * Time.deltaTime;
                    wallStored.materials[1].color = color;
                }
            }

        }
        
        if(hit.collider == null)
        {
            Color color2 = wallStored.materials[1].color;
            color2.a = 1;
            wallStored.materials[1].color = color2;
        }
    }
}
