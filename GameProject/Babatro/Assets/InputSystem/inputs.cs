using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace InputSystem
{
    public class Inputs : MonoBehaviour
    {
        public UnityEvent rollEvent = new();
        public UnityEvent changeBindEvent = new();

        public void OnRoll(InputValue value)
        {
            if (value.isPressed)
                rollEvent?.Invoke();
        }
        
        public void OnChangeBind(InputValue value)
        {
            if (value.isPressed)
                changeBindEvent?.Invoke();
        }
    }
}