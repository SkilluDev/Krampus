using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class interaction : MonoBehaviour
{
    private static readonly int CurrentlyEating = Animator.StringToHash("CurrentlyEating");
    [SerializeField] Camera cam;
    [SerializeField] Animator animator;
    public float tongueLength;
    [SerializeField] Transform tonguePosition;
    public LayerMask tonguable;
    private LineRenderer lineRenderer;
    private GameObject child;
    private TrailRenderer trail;

    private bool canTongue = true;

    Ray ray;
    RaycastHit hitData;
    Vector3 dir;
    
    RaycastHit hit;

    public WinCondition winCondition;

    public static int goodChildrenEatCount=0;
    public static int badChildrenEatCount=0;
    
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        trail=gameObject.transform.GetChild(2).gameObject.GetComponent<TrailRenderer>();

    }



    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hitData, 1000,LayerMask.GetMask("MapCollider", "Child")))
            {
                Vector3 realPoint = new Vector3(hitData.point.x, 2, hitData.point.z);
                dir = ( realPoint - transform.position );
                Debug.DrawRay(transform.position, dir, Color.red,5f);
                if (Physics.Raycast(transform.position, dir.normalized, out hit , tongueLength, LayerMask.GetMask("Wall","Child","Door")))
                {
                    Debug.Log(hit.transform.name);
                    Debug.Log(hit.transform.gameObject.layer);
                    if (hit.transform.gameObject.layer == 7)
                    {
                        child = hit.transform.gameObject;
                        child.GetComponent<ChildContoller>().Eat();
                        trail.enabled = true;
                        trail.gameObject.transform.position = child.transform.position;
                        float time = 0.75f;
                        animator.SetTrigger("Eat");
                    
                        hit.transform.DOMoveInTargetLocalSpace(transform, Vector3.zero, time).SetEase(Ease.OutSine);
                        trail.transform.DOMoveInTargetLocalSpace(transform, Vector3.zero, time).SetEase(Ease.OutSine);
                        StartCoroutine(UpdateLineRenderer());
                        StartCoroutine(StopUpdateLineRenderer(time));
                    }
                    
                }
            }
        }

    }
    
    IEnumerator StopUpdateLineRenderer(float time)
    {

        GetComponent<characterController>().shouldKrampusMove = false;
        yield return new WaitForSeconds(time);
        lineRenderer.enabled = false;
        trail.enabled = false;
        StopCoroutine(UpdateLineRenderer());
        Destroy(child);
        GetComponent<characterController>().shouldKrampusMove = true;
        GrandPoints();
        yield break;
    }

    IEnumerator UpdateLineRenderer()
    {
        lineRenderer.enabled = true;
        while (true)
        {
            if (!child)
            {
                yield break;
            }
            lineRenderer.SetPosition(0, tonguePosition.position+Vector3.up*0.5f);
            lineRenderer.SetPosition(1, child.transform.position+Vector3.up*1.5f);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator TongueTimeout(float waitTime)
    {
        canTongue = false;
        yield return new WaitForSeconds(waitTime);
        canTongue = true;
    }

    void GrandPoints() 
    {
        if (child.gameObject.GetComponent<Child>().isBad)
        {
            badChildrenEatCount++;
            WinCondition.AddScore(10);
        }
        else
        {
            goodChildrenEatCount++;
            WinCondition.SubtractScore(20);
        }

        if (badChildrenEatCount == ChildSpawner.badChildrenCount)
        {
            winCondition.GameWon();
        }
    }
    
    
    
}
