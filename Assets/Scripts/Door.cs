using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable {

    [SerializeField] private Animator m_animator;
    [SerializeField][AnimatorParam(nameof(m_animator))] private int m_openProperty, m_openSuddenProperty;

    public void Interact(IInteractor interactor) {

    }

    // TODO: redo

    private void OnTriggerEnter(Collider other) {
        m_animator.SetTrigger(m_openProperty);
        m_animator.SetBool(m_openSuddenProperty, true);
        Debug.Log("Trigger enter");
        Destroy(GetComponent<BoxCollider>());
    }

}
