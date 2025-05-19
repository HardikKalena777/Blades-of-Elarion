using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour
{
    public string triggerTag;

    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(triggerTag))
        {
            onTriggerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(triggerTag))
        {
            onTriggerExit.Invoke();
        }
    }
}
