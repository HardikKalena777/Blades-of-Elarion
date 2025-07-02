using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class TargetLockSystem : MonoBehaviour
{
    public CinemachineCamera playerFollowCamera;
    public CinemachineCamera targetCamera;

    public float lockOnRaduis;
    public LayerMask targetLayerMask;
    public Transform currentTarget;
    public bool isLockedOn;
    public float rotationSpeed;

    private ThirdPersonController controller;
    private LockOnUI lockOnUI;

    private void Start()
    {
        controller = GetComponent<ThirdPersonController>();
        lockOnUI = GetComponent<LockOnUI>();
    }

    private void Update()
    {
        controller.SetCameraInputLock(isLockedOn);
        if (Input.GetKeyDown(KeyCode.Mouse2) || Input.GetKeyDown(KeyCode.Joystick1Button11))
        {
            currentTarget = ScanNearbyEnemies();
        }
        SwitchTargetCamera();
    }

    private void LateUpdate()
    {
        if(isLockedOn)
        {
            Vector3 direction = currentTarget.position - transform.position;
            direction.y = 0; 
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public Transform ScanNearbyEnemies()
    {
        if (!isLockedOn)
        {
            Collider[] targetColliders = Physics.OverlapSphere(transform.position, lockOnRaduis, targetLayerMask);
            float shortestDistance = Mathf.Infinity;
            Transform closestTarget = null;
            foreach (Collider collider in targetColliders)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestTarget = collider.transform;
                }
            }
            return closestTarget;
        }
        else
        {
            currentTarget = null;
            return null;
        }
    }

    public void SwitchTargetCamera()
    {
        if(currentTarget != null)
        {
            playerFollowCamera.Priority = 0;
            targetCamera.Priority = 10;
            targetCamera.LookAt = currentTarget;
            lockOnUI.target = currentTarget;
            lockOnUI.HideMarker(false);
            isLockedOn = true;
        }
        else
        {
            playerFollowCamera.Priority = 10;
            targetCamera.Priority = 0;
            targetCamera.LookAt = null;
            lockOnUI.target = null;
            lockOnUI.HideMarker(true);
            isLockedOn = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lockOnRaduis);
    }

}
