using NaughtyAttributes;
using Sound;
using UnityEngine;
using UnityEngine.VFX;

public class ChildAnimator : MonoBehaviour {
    [SerializeField] private Child m_child;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_model;
    [SerializeField] private float m_turningSpeed = 5f;
    [SerializeField] private SpriteRenderer m_shapeSprite;
    [BoxGroup("Particles")][SerializeField] private VisualEffect m_goreParticle;
    [BoxGroup("Particles")][SerializeField] private VisualEffect m_vanishParticle;
    [BoxGroup("Animator Properties")][SerializeField][AnimatorParam(nameof(m_animator))] private int m_propertySpeed, m_propertyStun, m_propertyPanic, m_propertyReporting, m_propertyDeath;
    [BoxGroup("State Sprites")][SerializeField] private StatusSprite m_spriteRenderer;
    [BoxGroup("State Sprites")][SerializeField] private Sprite m_panicSprite;
    [BoxGroup("State Sprites")][SerializeField] private Sprite m_alertedSprite;
    [BoxGroup("Sounds")][SerializeField] private float m_screamVolume = 0.6f;
    [BoxGroup("Sounds")][SerializeField] private AudioSource m_screamSource;
    [BoxGroup("Sounds")][SerializeField] private Sex m_soundShock;
    [BoxGroup("Sounds")][SerializeField] private Sex m_soundKill;

    private void Start() {
        m_child.onStateChanged += ChildStateChanged;
        m_screamSource.time = Random.Range(0, m_screamSource.time);
    }

    private void ChildStateChanged(Child.State previous, Child.State current) {
        switch ((previous, current)) {
            case (_, Child.State.Stunned):
                m_animator.SetTrigger(m_propertyStun);
                m_spriteRenderer.SetSprite(m_panicSprite, 2);
                m_soundShock.Play(transform.position);
                m_screamSource.Play();
                break;
            case (_, Child.State.Dead):
                m_animator.SetTrigger(m_propertyDeath);
                break;
            case (_, Child.State.Consumed):
                m_soundKill.Play(transform.position, 1);

                if (m_child.Type != Game.MainGameInfo.GoodChildType) {
                    var particle = Instantiate(m_goreParticle, Game.MainGameInfo.Krampus.Kamera.Rendering.transform); //particle is screenspace.
                    particle.transform.SetPositionAndRotation(Game.MainGameInfo.Krampus.Tongue.transform.position, Quaternion.identity);
                } else {
                    var particle = Instantiate(m_vanishParticle);
                    particle.transform.SetPositionAndRotation(Game.MainGameInfo.Krampus.Tongue.transform.position, Quaternion.identity);
                }
                break;
            case (_, Child.State.Idle):
                m_spriteRenderer.ClearSprite();
                break;
            case (_, Child.State.InitialPanic):
                break;
            case (_, Child.State.Panic):
                m_spriteRenderer.SetSprite(m_panicSprite);
                break;
            case (_, Child.State.Reporting):
                break;
            case (_, Child.State.Alerted):
                m_spriteRenderer.SetSprite(m_alertedSprite);
                break;

        }
    }

    private void Update() {
        m_screamSource.volume = Mathf.Lerp(m_screamSource.volume, (m_child.CurrentState is Child.State.Panic or Child.State.InitialPanic) ? m_screamVolume : 0f, Time.deltaTime);
        m_model.rotation = Quaternion.Slerp(m_model.rotation, Quaternion.Euler(0, m_child.FacingAngle, 0), Time.deltaTime * m_turningSpeed);

        m_animator.SetBool(m_propertyPanic, m_child.CurrentState is Child.State.Panic or Child.State.InitialPanic);
        m_animator.SetBool(m_propertyReporting, m_child.CurrentState == Child.State.Reporting);
        m_animator.SetFloat(m_propertySpeed, m_child.Velocity / (m_child.CurrentState == Child.State.Panic ? m_child.RunSpeed : m_child.BaseMovementSpeed));
    }

    public void SetChildType(ChildType type) {
        var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var s in skinnedMeshRenderers) {
            s.material.SetColor("_Color", type.color);
        }

        m_shapeSprite.sprite = type.shape;
        var c = new Color(type.color.r, type.color.g, type.color.b, 0.25f);
        m_shapeSprite.color = c;
    }
}
