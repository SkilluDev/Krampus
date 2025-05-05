using UnityEngine.InputSystem;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KrampInput {
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class FlickInteraction : IInputInteraction {
        public float duration = 0.05f;
        public float activationStart = 0.7f;
        public float activationFinish = 0.2f;

        static FlickInteraction() {
            InputSystem.RegisterInteraction<FlickInteraction>("Flick Release");
        }

        public void Process(ref InputInteractionContext context) {
            if (context.timerHasExpired) {
                context.Canceled();
                return;
            }

            var value = context.ReadValue<Vector2>();

            switch (context.phase) {
                case InputActionPhase.Waiting:
                    if (value.magnitude >= activationStart) {
                        context.Started();
                        context.SetTimeout(duration);
                        Debug.Log("Start timer");
                    }
                    break;

                case InputActionPhase.Started:
                    if (value.magnitude <= activationFinish) {
                        context.Performed();
                        Debug.Log("end timer");
                    }
                    break;
            }

        }

        public void Reset() {

        }
    }
}