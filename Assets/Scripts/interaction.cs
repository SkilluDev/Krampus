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

    public static int goodChildrenEatCount=0;
    public static int badChildrenEatCount=0;

    private GameObject empty;

    private Vector3 tonguePoint;
    
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        trail=gameObject.transform.GetChild(2).gameObject.GetComponent<TrailRenderer>();

    }



    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canTongue)
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
                    else
                    {
                        float time = 0.2f;
                        empty = new GameObject();
                        empty.transform.position = tonguePosition.position;
                        tonguePoint = hit.point;
                        StartCoroutine(EmptyTongueOut(time));
                        StartCoroutine(EmptyTongueIn(time));
                    }
                    
                }
                else
                {
                    float time = 0.2f;
                    empty = new GameObject();
                    empty.transform.position = tonguePosition.position;
                    tonguePoint = transform.position+dir.normalized*tongueLength;
                    StartCoroutine(EmptyTongueOut(time));
                    StartCoroutine(EmptyTongueIn(time));
                }
            }
        }

    }

    IEnumerator EmptyTongueOut(float time)
    {
        empty.transform.DOMove(tonguePoint, time).SetEase(Ease.OutCubic);
        lineRenderer.enabled = true;
        canTongue = false;
        while (true)
        {
            lineRenderer.SetPosition(0, tonguePosition.position);
            lineRenderer.SetPosition(1, empty.transform.position);
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator EmptyTongueIn(float time)
    {
        yield return new WaitForSeconds(time);
        empty.transform.DOMove(tonguePosition.position, time).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(time);
        StopCoroutine(EmptyTongueOut(time));
        lineRenderer.enabled = false;
        canTongue = true;
        
    }
    IEnumerator StopUpdateLineRenderer(float time)
    {

        GetComponent<characterController>().shouldKrampusMove = false;
        yield return new WaitForSeconds(time);
        lineRenderer.enabled = false;
        trail.enabled = false;
        StopCoroutine(UpdateLineRenderer());
        Destroy(child);
        canTongue = true;
        GetComponent<characterController>().shouldKrampusMove = true;
        GrandPoints();
        yield break;
    }

    IEnumerator UpdateLineRenderer()
    {
        lineRenderer.enabled = true;
        canTongue = false;
        while (true)
        {
            if (!child)
            {
                yield break;
            }
            lineRenderer.SetPosition(0, tonguePosition.position);
            lineRenderer.SetPosition(1, child.transform.position+Vector3.up*1.5f);
            yield return new WaitForEndOfFrame();
        }
    }
    void GrandPoints() 
    {
        if (child.gameObject.GetComponent<Child>().isBad)
        {
            badChildrenEatCount++;
            WinCondition.Instance.AddScore(10);
        }
        else
        {
            goodChildrenEatCount++;
            WinCondition.Instance.SubtractScore(20);
        }

        if (badChildrenEatCount == ChildSpawner.badChildrenCount)
        {
            WinCondition.Instance.GameWon();
        }
    }
    
    
    
}
