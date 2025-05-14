using System.Collections;
using System.Collections.Generic;

using LitMotion;
using LitMotion.Extensions;
using Unity.Collections;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    private Transform[] m_tutorials;
    private int m_tutorialCounter = 0;

    [SerializeField] private float m_transitionLength;
    [SerializeField] private float m_rotateAngle;
    [SerializeField] private float m_slideLength;

    [SerializeField] private GameObject m_tutorialText;

    private float m_distanceBetween = 1f;

    private bool m_isMoving = false;

    private MotionHandle m_handle;



	private void Awake() {
        m_tutorials = new Transform[gameObject.transform.childCount];
        for (int i = 0;i<m_tutorials.Length;i++){
            m_tutorials[m_tutorials.Length-1-i] = gameObject.transform.GetChild(i);
            m_tutorials[m_tutorials.Length-1-i].transform.position += new Vector3(0,0,m_distanceBetween*i);
        }
	}
	private void Update()
    {
        //LMB to go forward RMB to skip
        if (InputSubscribe.Raw.UI.Quit.WasPerformedThisFrame()){
            Debug.Log("Should quit tutorial now");
            if (m_handle.IsActive()) m_handle.Cancel();
            gameObject.SetActive(false);
            m_tutorialText.SetActive(false);
        }
        if (InputSubscribe.Raw.UI.Advance.WasPerformedThisFrame() && !m_isMoving && gameObject.activeSelf) {
            MoveBack(m_tutorialCounter++%m_tutorials.Length);
        }
        
    }

    private void MoveBack(int id){

        m_isMoving = true;
        
        var page = m_tutorials[id].transform;
        var oldLocalPosition = page.localPosition;
        var oldLocalMainPosition = transform.localPosition;

        var oldPosition = page.position;
        var oldRotation = page.rotation;
        var lSequence = LSequence.Create();

        

        
        lSequence.Append(LMotion.Create(page.rotation, page.rotation*Quaternion.Euler(new Vector3(0,0,m_rotateAngle)), m_transitionLength).WithEase(Ease.InOutCubic)
        .BindToRotation(page));

        var currentLocalPagePosition = oldLocalPosition;
        var nextLocalPagePosition = currentLocalPagePosition+Vector3.right*m_slideLength*2;
        Debug.Log(currentLocalPagePosition+"->"+nextLocalPagePosition);

        lSequence.Join(LMotion.Create(currentLocalPagePosition,nextLocalPagePosition, m_transitionLength).WithEase(Ease.InOutCubic)
        .BindToLocalPosition(page));

        var currentLocalMainPosition = oldLocalMainPosition;
        var nextLocalMainPosition = currentLocalMainPosition-Vector3.right*m_slideLength;

        lSequence.Join(LMotion.Create(currentLocalMainPosition, nextLocalMainPosition, m_transitionLength).WithEase(Ease.InOutCubic)
        .BindToLocalPosition(transform));

        currentLocalPagePosition = nextLocalPagePosition;
        nextLocalPagePosition = currentLocalPagePosition-Vector3.forward*(m_tutorials.Length+1)*m_distanceBetween;

        lSequence.Append(LMotion.Create(currentLocalPagePosition,nextLocalPagePosition, m_transitionLength).WithEase(Ease.InOutCubic).
        WithOnComplete(()=>page.SetAsFirstSibling()).BindToLocalPosition(page));

        lSequence.Append(LMotion.Create(page.rotation*Quaternion.Euler(new Vector3(0,0,m_rotateAngle)), page.rotation, m_transitionLength).WithEase(Ease.InOutCubic)
        .BindToRotation(page));

        currentLocalPagePosition = nextLocalPagePosition;
        nextLocalPagePosition = currentLocalPagePosition-Vector3.right*m_slideLength*2;

        lSequence.Join(LMotion.Create(currentLocalPagePosition,nextLocalPagePosition, m_transitionLength).WithEase(Ease.InOutCubic)
        .BindToLocalPosition(page));

        currentLocalMainPosition = nextLocalMainPosition;
        nextLocalMainPosition = currentLocalMainPosition+Vector3.right*m_slideLength;

        lSequence.Join(LMotion.Create(currentLocalMainPosition, nextLocalMainPosition, m_transitionLength).WithEase(Ease.InOutCubic)
        .BindToLocalPosition(transform));

        currentLocalMainPosition = nextLocalMainPosition;
        nextLocalMainPosition = currentLocalMainPosition+Vector3.forward*m_distanceBetween;

        lSequence.Append(LMotion.Create(currentLocalMainPosition, nextLocalMainPosition, m_transitionLength).WithEase(Ease.InOutCubic)
        .WithOnComplete(()=>m_isMoving=false).BindToLocalPosition(transform));

        m_tutorials[id].transform.position = oldPosition;
        m_tutorials[id].transform.localPosition = oldLocalPosition;

        m_tutorials[id].transform.rotation = oldRotation;
        transform.localPosition = oldLocalMainPosition;

        m_handle = lSequence.Run();
    }
}
