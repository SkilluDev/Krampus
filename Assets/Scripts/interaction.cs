using System.Collections;
using DG.Tweening;
using UnityEngine;

public class interaction : MonoBehaviour
{
    public Camera cam;
    public float tongueLength;
    public LayerMask tonguable;
    private LineRenderer lineRenderer;
    private GameObject child;
    
    void Start()
    {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }



    void Update()
    {
        Vector3 worldPosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        Vector3 upray = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
        Vector3 dir = ( upray - transform.position ).normalized;
        
        Debug.DrawLine(transform.position, worldPosition, Color.green);
        Debug.DrawLine(worldPosition, upray, Color.red);
        Debug.DrawLine(transform.position, upray, Color.blue);
        Debug.DrawRay(transform.position, dir, Color.yellow);

        RaycastHit hit;

        if (Input.GetButtonDown("Fire1"))
        {
            if (Physics.Raycast(transform.position, dir, out hit , tongueLength, tonguable) && !Physics.Raycast(transform.position, dir,(hit.rigidbody.position - transform.position).magnitude, 1<<6))
            {
                //Vector3.Lerp(hit.rigidbody.position, transform.position, 0.5f);
                child = hit.transform.gameObject;
                
                float time = 1f;
                DOTween.To(()=>hit.rigidbody.position,(x)=>hit.rigidbody.position=x, transform.position, time).SetEase(Ease.InOutExpo);
                StartCoroutine(UpdateLineRenderer());
                StartCoroutine(StopUpdateLineRenderer(time));
            }

        }

    }

    IEnumerator StopUpdateLineRenderer(float time)
    {
        yield return new WaitForSeconds(time);
        lineRenderer.enabled = false;
        StopCoroutine(UpdateLineRenderer());
        yield break;
    }

    IEnumerator UpdateLineRenderer()
    {
        lineRenderer.enabled = true;
        while (true)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, child.transform.position);
            yield return new WaitForEndOfFrame();
        }
    }
}
