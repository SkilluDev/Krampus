using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Box : MonoBehaviour, IThrowable {
    [BoxGroup("Behaviour")][SerializeField] private Transform m_pinTarget;
    [BoxGroup("Behaviour")][SerializeField] private Vector3 m_inMouthScale;

    
    [SerializeField] private LayerMask m_destroyMask;
    [SerializeField] private LayerMask m_stunMask;
    [SerializeField] private Transform m_allModel;
     [SerializeField] private Transform m_boxModel;
    [SerializeField] private GameObject m_specialEffect;

    [SerializeField] private float m_stunDuration;

    [SerializeField] private Collider m_collider;
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private bool m_inMove;
   private Krampus m_owner;

    [SerializeField] private float m_throwForce;

    public Vector3 InteractionPoint => m_pinTarget.transform.position;
    public int Priority => 0;

    public void Consume(Krampus krampus) => throw new System.NotImplementedException();

    public void Hit(Krampus krampus) {

    }

    public void Hold() {
        transform.localScale = m_inMouthScale;
    }

	public void Interact(IInteractor interactor) { }
    public void Prepare(Krampus krampus) {
        m_rigidbody.velocity = Vector3.zero;
        m_collider.enabled = false;
        m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        


    }
	private void Update() {
        if (m_inMove) {
            m_boxModel.Rotate(1440 * Time.deltaTime, 0, 0);
        }
	}

	public void ReelIn(Krampus krampus, Vector3 position, float progress) {
        transform.position = position - transform.InverseTransformPoint(m_pinTarget.position);

    }

    public void Throw(Vector3 vector3, Krampus krampus) {

        transform.rotation = Quaternion.identity;
        m_allModel.rotation = Quaternion.LookRotation(vector3);
        m_specialEffect.SetActive(true);
        m_rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
        m_rigidbody.velocity = vector3 * m_throwForce;
        m_collider.enabled = true;
        m_collider.isTrigger = true;
        m_owner = krampus;
        m_inMove = true;

    }
    

    void OnTriggerEnter(Collider other) {

        if ((m_stunMask & (1 << other.gameObject.layer)) != 0) {


            if (other.gameObject.GetComponent<Nun>()  ) {
                other.gameObject.GetComponent<Nun>().Stun(m_stunDuration);
             }
              if (other.gameObject.GetComponent<Child>()  ) {
                other.gameObject.GetComponent<Child>().Stun(m_stunDuration);
             }
            Destroy(gameObject);
           
        }
       
        if ((m_destroyMask & (1 << other.gameObject.layer)) != 0) {

            Destroy(gameObject);
        }
         
	}
}
