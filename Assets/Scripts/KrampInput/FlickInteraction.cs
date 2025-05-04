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
        public float duration = 0.7f;
        public float activationDeadzone = 0.2f;

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
                    if (value.magnitude >= 0.5f) {
                        context.Started();
                        Debug.Log("Action started");
                        context.SetTimeout(duration);
                    }
                    break;

                case InputActionPhase.Started:
                    if (value.magnitude <= activationDeadzone) {
                        context.Performed();
                        Debug.Log("Action performed");
                    }
                    break;
            }

        }

        public void Reset() {

        }
    }
}