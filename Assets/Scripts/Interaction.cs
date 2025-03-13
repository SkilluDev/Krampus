using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Interaction : MonoBehaviour {
    private static readonly int CurrentlyEating = Animator.StringToHash("CurrentlyEating");

    public Texture2D cursor;
    [FormerlySerializedAs("cam")][SerializeField] private Camera m_cam;
    [FormerlySerializedAs("animator")][SerializeField] private Animator m_animator;
    [FormerlySerializedAs("tonguePosition")][SerializeField] private Transform m_tonguePosition;
    [FormerlySerializedAs("animator")][SerializeField] private float m_tongueLength;


    private LineRenderer lineRenderer;
    private GameObject child;
    private TrailRenderer trail;

    public GameObject model;

    private bool canTongue = true;

    [SerializeField] private float lolipopRadius = 2f;


    private Vector3 lastRealPosition = Vector3.zero;



    private Ray ray;
    private RaycastHit hitData;
    private Vector3 dir;

    private RaycastHit hit;

    public static int goodChildrenEatCount = 0;
    public static int badChildrenEatCount = 0;

    private GameObject empty;

    private Vector3 tonguePoint;

    private void Start() {
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        trail = GetComponentInChildren<TrailRenderer>();
        m_cam = Camera.main.GetComponent<Camera>();

    }



    private void Update() {
        if (Input.GetButtonDown("Fire1") && canTongue && GetComponent<CharacterController>().shouldKrampusMove) {
            StartCoroutine(ShootTongue());
        }
    }

    private IEnumerator ShootTongue() {
        GetComponent<CharacterController>().shouldKrampusMove = false;
        m_animator.SetBool("hasHit", false);
        canTongue = false;
        m_animator.SetTrigger("Shoot");
        ray = m_cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitData, 1000, LayerMask.GetMask("MapCollider", "Child"))) {
            SoundManager.PlaySound("tongue");
            Vector3 realPoint = new Vector3(hitData.point.x, 1f, hitData.point.z);
            lastRealPosition = realPoint;
            dir = (realPoint - transform.position);
            RotatePlayer(dir);
            yield return new WaitForSeconds(0.4f);
            Debug.DrawRay(transform.position + Vector3.up, dir, Color.red, 5f);
            if (Physics.Raycast(transform.position + Vector3.up, dir.normalized, out hit, m_tongueLength, LayerMask.GetMask("Wall", "Child", "Door"))) {
                Debug.Log(hit.transform.name);
                Debug.Log(hit.transform.gameObject.layer);
                Collider[] cols = Physics.OverlapSphere(hit.point, lolipopRadius, LayerMask.GetMask("Child"));
                if (cols.Length > 0) {
                    RotatePlayer(dir);
                    child = cols[0].gameObject;
                    if (Physics.Raycast(transform.position, (child.transform.position - transform.position).normalized, Vector3.Distance(transform.position, child.transform.position), LayerMask.GetMask("Wall"))) {
                        m_animator.SetBool("hasHit", false);
                        float time = 0.2f;
                        empty = new GameObject();
                        empty.transform.position = m_tonguePosition.position;
                        tonguePoint = transform.position + dir.normalized * m_tongueLength;

                        StartCoroutine(EmptyTongueOut(time));
                        StartCoroutine(EmptyTongueIn(time));

                    } else {
                        Debug.Log(child.transform.name);
                        Debug.Log(child.transform.gameObject.layer);
                        child.GetComponent<ChildContoller>().Eat();
                        SoundManager.PlaySound("catch");
                        trail.enabled = true;
                        trail.gameObject.transform.position = child.transform.position;
                        float time = 0.85f;
                        m_animator.SetBool("hasHit", true);
                        StartCoroutine(UpdateLineRenderer());
                        StartCoroutine(StopUpdateLineRenderer(time));
                        lineRenderer.enabled = true;
                        trail.transform.DOMoveInTargetLocalSpace(transform, Vector3.zero, time).SetEase(Ease.InExpo);
                        child.transform.DOMoveInTargetLocalSpace(transform, Vector3.zero, time).SetEase(Ease.InExpo);
                    }
                } else {

                    Debug.Log("I am eating your mom");
                    float time = 0.2f;
                    empty = new GameObject();
                    empty.transform.position = m_tonguePosition.position;
                    tonguePoint = hit.point;
                    Destroy(empty, 3);

                    StartCoroutine(EmptyTongueOut(time));
                    StartCoroutine(EmptyTongueIn(time));
                }

            } else {
                m_animator.SetBool("hasHit", false);
                float time = 0.2f;
                empty = new GameObject();
                empty.transform.position = m_tonguePosition.position;
                tonguePoint = transform.position + dir.normalized * m_tongueLength;
                StartCoroutine(EmptyTongueOut(time));
                StartCoroutine(EmptyTongueIn(time));
            }
        }
        m_animator.SetTrigger("nextAction");
    }



    private IEnumerator EmptyTongueOut(float time) {
        empty.transform.DOMove(tonguePoint, time).SetEase(Ease.OutCubic);
        lineRenderer.enabled = true;
        canTongue = false;
        while (true) {
            lineRenderer.SetPosition(0, m_tonguePosition.position);
            lineRenderer.SetPosition(1, empty.transform.position);
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator EmptyTongueIn(float time) {
        yield return new WaitForSeconds(time);
        empty.transform.DOMove(m_tonguePosition.position, time).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(time);
        StopCoroutine(EmptyTongueOut(time));
        lineRenderer.enabled = false;
        canTongue = true;
        if (!GetComponent<CharacterController>().isDead)
            GetComponent<CharacterController>().shouldKrampusMove = true;

    }
    private IEnumerator StopUpdateLineRenderer(float time) {


        yield return new WaitForSeconds(time);
        lineRenderer.enabled = false;
        trail.enabled = false;
        StopCoroutine(nameof(UpdateLineRenderer));
        SoundManager.PlaySound("kill");
        Destroy(child);
        Camera.main.GetComponent<CameraFollow>().Shake();
        canTongue = true;
        if (!GetComponent<CharacterController>().isDead)
            GetComponent<CharacterController>().shouldKrampusMove = true;
        GrandPoints();
        yield break;
    }

    private IEnumerator UpdateLineRenderer() {
        lineRenderer.enabled = true;
        canTongue = false;

        while (true) {
            if (!child) {
                yield break;
            }
            lineRenderer.SetPosition(0, m_tonguePosition.position);
            lineRenderer.SetPosition(1, child.transform.position + Vector3.up * 1.5f);
            yield return new WaitForEndOfFrame();
        }
    }

    private void GrandPoints() {
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


    private void RotatePlayer(Vector3 dir) {
        Debug.Log(dir);
        Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, dir.y, dir.z));

        model.transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(lastRealPosition, 0.1f);
        Gizmos.DrawWireSphere(lastRealPosition, lolipopRadius);
    }

}
