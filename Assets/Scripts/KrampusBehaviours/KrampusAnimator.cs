using LitMotion;
using NaughtyAttributes;
using Sound;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class KrampusAnimator : KrampusBehaviour {
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_modelTransform;
    [SerializeField] private float m_fastVelocity = 4;
    [SerializeField] private float m_rotationSmoothing = 15;


    [SerializeField] private VisualEffect m_runningEffect;

    [BoxGroup("Sounds")][SerializeField] private Sex m_catchSoundBite;
    [BoxGroup("Sounds")][SerializeField] private Sex m_tongueSoundBite;
    [BoxGroup("Sounds")][SerializeField] private Sex m_windupSoundBite;
    [BoxGroup("Sounds")][SerializeField] private Sex m_crackSoundBite;
    [BoxGroup("Sounds")][SerializeField] private Sex m_deathSoundBite;

    [SerializeField][AnimatorParam(nameof(m_animator))] private int m_speedProperty, m_stopProperty, m_tongueOutProperty, m_tongueReadyProperty, m_tongueShouldEatProperty, m_deathProperty, m_wakeupProperty;

    [SerializeField] private Transform m_lockInCircle;
    private bool m_inLockInAnimation = false;
    private MotionHandle m_lockInAnimation;
    private MotionHandle m_lockInAnimation_2;
    private float m_minimalVelocity;
    private Quaternion m_rotationTarget;

    [SerializeField] private KrampusIndicator m_krampusIndicator;

    private bool HasLockInItem => Kramp.Stats.HasItemWithTag(ItemTag.LockIn);

    private void Start() {
        Kramp.Tongue.onStateChanged += TongueStateChanged;
        Kramp.Kontroller.onStateChanged += MovementStateChanged;
        m_lockInCircle.gameObject.SetActive(false);
        SetEnableModel(false);


        Kramp.KrampusEvents.onEffectRegistered.AddListener(EffectAnimation);
    }

    private void Update() {
        if (Kramp.Kontroller.CurrentState is not (KrampusController.State.Intro or KrampusController.State.GetUp)) {
            if (Kramp.Kontroller.CurrentState != KrampusController.State.Idle && Kramp.Tongue.CurrentState is KrampusTongue.State.Idle or KrampusTongue.State.Carrying) {
                SetTargetView(Kramp.Kontroller.VelocityVector);
            } else if (Kramp.Tongue.CurrentState == KrampusTongue.State.Windup) {
                SetTargetView(Kramp.Tongue.TongueDirection);
            }
        }
        m_modelTransform.rotation = Quaternion.Slerp(m_modelTransform.rotation, m_rotationTarget, Time.deltaTime * m_rotationSmoothing);
        m_runningEffect.SetFloat("Rotation", m_modelTransform.rotation.eulerAngles.y);

        m_animator.SetFloat(m_speedProperty, Mathf.Max(m_minimalVelocity, Kramp.Kontroller.Velocity / Kramp.Kontroller.RunSpeed), 0.2f, Time.deltaTime);

        if (Kramp.Stats.GetFinalStat(KrampusStats.Stat.Speed) > Kramp.Kontroller.RunSpeed) {
            m_runningEffect.Play();
        }
    }

    public void TongueStateChanged(KrampusTongue.State previous, KrampusTongue.State current) {
        if (previous != current) Debug.Log("Tongue moment:" + previous + " -> " + current);
        switch ((previous, current)) {
            case (KrampusTongue.State.Idle, KrampusTongue.State.Windup):
                m_animator.SetBool(m_tongueReadyProperty, true);
                m_animator.SetBool(m_tongueShouldEatProperty, false);
                m_windupSoundBite.Play(transform.position, 1);
                break;
            case (KrampusTongue.State.Windup, KrampusTongue.State.Idle):
                m_animator.SetBool(m_tongueReadyProperty, false);
                break;
            case (KrampusTongue.State.Windup, KrampusTongue.State.TargetFetch):
                m_animator.SetTrigger(m_tongueOutProperty);
                m_animator.SetBool(m_tongueReadyProperty, false);
                m_tongueSoundBite.Play(transform.position, 1);

                break;
            case (_, KrampusTongue.State.PreRetreat):

                m_animator.SetBool(m_tongueReadyProperty, false);
                m_animator.SetBool(m_tongueShouldEatProperty, false);
                LockOutAnimation();
                break;
            case (KrampusTongue.State.TargetFetch, KrampusTongue.State.Extending):
                SetTargetView(Kramp.Tongue.TongueDirection);
                break;
            case (KrampusTongue.State.Extending, KrampusTongue.State.Full):

                if (Kramp.Tongue.HitInteractable is Child) m_crackSoundBite.Play(transform.position, 1);
                break;
            case (KrampusTongue.State.PreRetreat, KrampusTongue.State.Retreating):
                if (Kramp.Tongue.HitInteractable != null) {
                    m_catchSoundBite.Play(transform.position, 1);
                    m_animator.SetBool(m_tongueShouldEatProperty, Kramp.Tongue.HitInteractable is IKrampable);
                }
                break;
            case (_, KrampusTongue.State.Done):
                if (Kramp.Kontroller.CurrentState == KrampusController.State.Walk || Kramp.Kontroller.CurrentState == KrampusController.State.Idle) {

                    LockInAnimation();
                }
                break;
        }
    }

    public void MovementStateChanged(KrampusController.State previous, KrampusController.State current, KrampusController.StateChangeReason reason) {
        if (previous == current) return;
        switch ((previous, current)) {
            case (KrampusController.State.Run, KrampusController.State.Idle):
                LockInAnimation();
                if (reason == KrampusController.StateChangeReason.Rapid) {
                    m_animator.SetTrigger(m_stopProperty);
                }
                m_runningEffect.Stop();
                m_minimalVelocity = 0f;
                break;

            case (KrampusController.State.Intro, _):
                SetEnableModel(true);
                m_animator.SetTrigger(m_wakeupProperty);
                break;
            case (_, KrampusController.State.Run):
                LockOutAnimation();
                if (Kramp.Stats.GetFinalStat(KrampusStats.Stat.Speed) > Kramp.Kontroller.RunSpeed) {
                    m_runningEffect.Play();
                }
                m_minimalVelocity = 1f;
                break;

            case (_, KrampusController.State.Walk):
                LockInAnimation();
                m_runningEffect.Stop();
                m_minimalVelocity = 0.05f;
                break;
            case (_, KrampusController.State.Idle):
                LockInAnimation();
                m_runningEffect.Stop();
                m_minimalVelocity = 0f;
                break;
            case (_, KrampusController.State.Dash):
                LockOutAnimation();
                break;
            case (_, KrampusController.State.Dead):
                LockOutAnimation();
                m_runningEffect.Stop();
                m_animator.SetBool(m_deathProperty, true);

                m_deathSoundBite.Play(transform.position, 1f);
                m_animator.SetLayerWeight(1, 0);
                break;
        }
    }

    public void SetTargetView(Vector3 direction) {
        m_rotationTarget = Quaternion.LookRotation(direction, Vector3.up);
    }

    public void SetEnableModel(bool b) {
        m_modelTransform.gameObject.SetActive(b);
    }

    public void StopSmoke() {
        m_runningEffect.Stop();
    }

    private void LockInAnimation() {
        if (!HasLockInItem) return;
        if (m_inLockInAnimation) { Debug.Log("Siema zatrzymalem ci lockIn"); return; }

        m_lockInAnimation_2 = LMotion.Create(0, 1, 0.25f).WithOnComplete(() => m_lockInCircle.gameObject.SetActive(true)).Bind(null);
        m_lockInAnimation = LMotion.Create(0.4f, 0.14f, Kramp.Kontroller.LockInThreshold).WithOnComplete(() => LMotion.Create(0.14f, 0.16f, 0.1f).WithOnComplete(() => LMotion.Create(0.16f, 0.14f, 0.1f).
        Bind(x => m_lockInCircle.localScale = new Vector3(x, x, 3))).Bind(x => m_lockInCircle.localScale = new Vector3(x, x, 3))).Bind(x => m_lockInCircle.localScale = new Vector3(x, x, 3));
        m_inLockInAnimation = true;

    }

    private void LockOutAnimation() {
        if (!HasLockInItem) return;
        if (!m_inLockInAnimation) return;
        //Debug.Log("Lock out baby");
        m_lockInCircle.gameObject.SetActive(false);
        m_lockInAnimation.TryCancel();
        m_lockInAnimation_2.TryCancel();
        m_inLockInAnimation = false;
    }

    void EffectAnimation(Krampus krampus, Effect effect) {
        if (effect.StatModifier.Stat == KrampusStats.Stat.Speed && effect.StatModifier.Modifier < 0)
        {
            m_krampusIndicator.PlayAniamtion(effect.Duration);
         }
     }

}
