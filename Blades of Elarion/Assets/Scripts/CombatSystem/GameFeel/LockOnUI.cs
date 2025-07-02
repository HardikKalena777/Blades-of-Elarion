using UnityEngine;
using UnityEngine.UI;
public class LockOnUI : MonoBehaviour
{
    public RectTransform uiMarker;
    public Camera mainCamera;
    public Transform target;

    private void Update()
    {
        if (target == null) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + Vector3.up * 2f);
        bool isBehind = screenPos.z < 0f;

        if(isBehind)
        {
            screenPos *= -1f;     
        }

        uiMarker.position = screenPos;
        uiMarker.gameObject.SetActive(!isBehind);
    }

    public void HideMarker(bool hide)
    {
        if (hide)
        {
            uiMarker.gameObject.SetActive(false);
        }
        else
        {
            uiMarker.gameObject.SetActive(true);
        }
    }
}
