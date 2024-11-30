using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class interaction : MonoBehaviour
{
    private static readonly int CurrentlyEating = Animator.StringToHash("CurrentlyEating");

    public Texture2D cursor;
    [SerializeField] Camera cam;
    [SerializeField] Animator animator;
    public float tongueLength;
    [SerializeField] Transform tonguePosition;
 

    private LineRenderer lineRenderer;
    private GameObject child;
    private TrailRenderer trail;

    public GameObject model;
    
    private bool canTongue = true;

    [SerializeField] float lolipopRadius = 2f;


   Vector3 lastRealPosition = Vector3.zero;



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
      Cursor.SetCursor(cursor, new Vector2(cursor.width/2,cursor.height/2), CursorMode.ForceSoftware);
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        trail=GetComponentInChildren<TrailRenderer>();
        cam = Camera.main.GetComponent<Camera>();

    }



    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canTongue && GetComponent<characterController>().shouldKrampusMove)
        {
            StartCoroutine(ShootThoungh());
        }
    }

    IEnumerator ShootThoungh() 
    {
        GetComponent<characterController>().shouldKrampusMove = false;
        animator.SetBool("hasHit", false);
        canTongue = false;
        animator.SetTrigger("Shoot");
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitData, 1000, LayerMask.GetMask("MapCollider", "Child")))
        {
            Vector3 realPoint = new Vector3(hitData.point.x, 1f, hitData.point.z);
            lastRealPosition = realPoint;
            dir = (realPoint - transform.position);
            RotatePlayer(dir);
            yield return new WaitForSeconds(0.4f);
            Debug.DrawRay(transform.position+Vector3.up, dir, Color.red, 5f);
            if (Physics.Raycast(transform.position+Vector3.up, dir.normalized, out hit, tongueLength, LayerMask.GetMask("Wall", "Child", "Door")))
            {
                Debug.Log(hit.transform.name);
                Debug.Log(hit.transform.gameObject.layer);
                Collider[] cols = Physics.OverlapSphere(hit.point, lolipopRadius, LayerMask.GetMask("Child"));
                if (cols.Length > 0)
                { 
                    RotatePlayer(dir);
                    child = cols[0].gameObject;
                    if (Physics.Raycast(transform.position, (child.transform.position - transform.position).normalized, Vector3.Distance(transform.position, child.transform.position), LayerMask.GetMask("Wall")))
                    {
                        animator.SetBool("hasHit", false);
                        float time = 0.2f;
                        empty = new GameObject();
                        empty.transform.position = tonguePosition.position;
                        tonguePoint = transform.position + dir.normalized * tongueLength;

                        StartCoroutine(EmptyTongueOut(time));
                        StartCoroutine(EmptyTongueIn(time));

                    } else 
                    {
                        Debug.Log(child.transform.name);
                        Debug.Log(child.transform.gameObject.layer);
                        child.GetComponent<ChildContoller>().Eat();
                        trail.enabled = true;
                        trail.gameObject.transform.position = child.transform.position;
                        float time = 0.85f;
                        animator.SetBool("hasHit", true);
                        StartCoroutine(UpdateLineRenderer());
                        StartCoroutine(StopUpdateLineRenderer(time));
                        lineRenderer.enabled = true;
                        trail.transform.DOMoveInTargetLocalSpace(transform, Vector3.zero, time).SetEase(Ease.InExpo);
                        child.transform.DOMoveInTargetLocalSpace(transform, Vector3.zero, time).SetEase(Ease.InExpo);
                    }
                } else
                {

                    Debug.Log("I am eating your mom");
                    float time = 0.2f;
                    empty = new GameObject();
                    empty.transform.position = tonguePosition.position;
                    tonguePoint = hit.point;
                    Destroy(empty, 3);

                    StartCoroutine(EmptyTongueOut(time));
                    StartCoroutine(EmptyTongueIn(time));
                }

            } else
            {
                animator.SetBool("hasHit", false);
                float time = 0.2f;
                empty = new GameObject();
                empty.transform.position = tonguePosition.position;
                tonguePoint = transform.position + dir.normalized * tongueLength;
                StartCoroutine(EmptyTongueOut(time));
                StartCoroutine(EmptyTongueIn(time));
            }
        }
        animator.SetTrigger("nextAction");
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
        if (!GetComponent<characterController>().isDead)
            GetComponent<characterController>().shouldKrampusMove = true;

    }
    IEnumerator StopUpdateLineRenderer(float time)
    {

       
        yield return new WaitForSeconds(time);
        lineRenderer.enabled = false;
        trail.enabled = false;
        StopCoroutine(UpdateLineRenderer());
        Destroy(child);
        Camera.main.GetComponent<CameraFollow>().Shake();
        canTongue = true;
        if(!GetComponent<characterController>().isDead)
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
    void GrandPoints() {
        if (child.gameObject.GetComponent<Child>().isBad) {
            badChildrenEatCount++;
            WinCondition.Instance.badChildEaten();
            WinCondition.Instance.AddScore(10);
            WinCondition.Instance.SubtractTime(-10);
        } else {
            goodChildrenEatCount++;
            WinCondition.Instance.SubtractScore(20);
            WinCondition.Instance.SubtractTime(15);
        }

    }


    void RotatePlayer(Vector3 dir) 
    {
        Debug.Log(dir);
        Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x,dir.y,dir.z));
        
        model.transform.rotation = Quaternion.Euler(0,rot.eulerAngles.y,0);


    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(lastRealPosition, 0.1f);
        Gizmos.DrawWireSphere(lastRealPosition, lolipopRadius);
    }

}
