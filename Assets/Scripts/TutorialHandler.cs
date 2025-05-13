using System.Collections;
using System.Collections.Generic;

using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    private Transform[] m_tutorials;
    private int m_tutorialCounter = 0;

    [SerializeField] private float m_transitionLength;
    [SerializeField] private float m_rotateAngle;

	void Awake() {
        m_tutorials = new Transform[gameObject.transform.childCount];
        for (int i = 0;i<m_tutorials.Length;i++){
            m_tutorials[i] = gameObject.transform.GetChild(i);
            m_tutorials[i].transform.position += new Vector3(0,0,10*i);
        }
	}
	void Update()
    {
        if (InputSubscribe.Raw.Player.Move.WasPerformedThisFrame()) {
            MoveBack(m_tutorialCounter++%m_tutorials.Length);
        }
        
    }

    void MoveBack(int id){
        
        var page = m_tutorials[id].transform;
        var oldPosition = page.position;
        var oldRotation = page.rotation;
        var lSequence = LSequence.Create();
        //lSequence.AppendInterval(m_transitionLength);
        lSequence.Insert(m_transitionLength,LMotion.Create(page.rotation, Quaternion.Euler(new Vector3(0,0,m_rotateAngle)), m_transitionLength).WithEase(Ease.InOutCubic)
        .BindToRotation(page));

        lSequence.Insert(m_transitionLength*2, LMotion.Create(Vector3.zero,Vector3.forward*(m_tutorials.Length+1)*10, m_transitionLength).WithEase(Ease.InOutCubic)
        .BindToLocalPosition(page));

        lSequence.Insert(m_transitionLength*3, LMotion.Create(Quaternion.Euler(new Vector3(0,0,m_rotateAngle)), page.rotation, m_transitionLength).WithEase(Ease.InOutCubic)
        .BindToRotation(page));

        lSequence.Insert(m_transitionLength*4, LMotion.Create(Vector3.zero, -Vector3.forward*10, m_transitionLength).WithEase(Ease.InOutCubic)
        .BindToLocalPosition(transform));

        m_tutorials[id].transform.position = oldPosition;
        m_tutorials[id].transform.rotation = oldRotation;


        lSequence.Run();
    }
}
