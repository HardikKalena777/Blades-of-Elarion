using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;

    private void Start()
    {
        target = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(target);
    }
}
