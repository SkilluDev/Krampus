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
    public LayerMask tonguable;
    private LineRenderer lineRenderer;
    private GameObject child;
    private TrailRenderer trail;

    Ray ray;
    RaycastHit hitData;
    Vector3 testWorldPosition;
    Vector3 dir;
    
    RaycastHit hit;
    private float completionRadius = 1f;

    public static int goodChildrenEatCount=0;
    public static int badChildrenEatCount=0;
    
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        trail=gameObject.transform.GetChild(2).gameObject.GetComponent<TrailRenderer>();
        Debug.Log(trail);

    }



    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hitData, 1000))
            {
                Debug.Log(hitData.transform.name);
                dir = ( hitData.transform.position - transform.position ).normalized;
                
                if (Physics.Raycast(transform.position, dir, out hit , tongueLength, tonguable) && !Physics.Raycast(transform.position, dir,(hit.rigidbody.position - transform.position).magnitude, 1<<6))
                {
                    child = hit.transform.gameObject;
                    if (child.gameObject.GetComponent<Child>().isBad)
                    {
                        badChildrenEatCount++;
                    }
                    else
                    {
                        goodChildrenEatCount++;
                    }
                    trail.enabled = true;
                    trail.gameObject.transform.position = child.transform.position;
                    float time = 1f;
                    animator.SetTrigger(CurrentlyEating);
                    animator.SetTrigger(CurrentlyEating);
                    hit.transform.DOMoveInTargetLocalSpace(transform, Vector3.zero, time).SetEase(Ease.OutSine);
                    trail.transform.DOMoveInTargetLocalSpace(transform, Vector3.zero, time).SetEase(Ease.OutSine);
                    /*Tweener childMove = DOTween.To(()=>hit.transform.position,(x)=>hit.transform.position=x, transform.position, time).SetEase(Ease.InOutExpo);
                    Tweener trailMove = DOTween.To(()=>trail.transform.position,(x)=>trail.transform.position=x, transform.position, time).SetEase(Ease.InOutExpo);*/
                    StartCoroutine(UpdateLineRenderer());
                    StartCoroutine(StopUpdateLineRenderer(time));
                }
            }
        }

    }
    
    IEnumerator StopUpdateLineRenderer(float time)
    {
        yield return new WaitForSeconds(time);
        lineRenderer.enabled = false;
        trail.enabled = false;
        StopCoroutine(UpdateLineRenderer());
        Destroy(child);
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
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, child.transform.position);
            yield return new WaitForEndOfFrame();
        }
    }
    
    
    
}
