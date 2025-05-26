using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Animator layoutAnim;

    bool enabledBefore = false;

    public void ToggleUI()
    {
        // Check for C key or Gamepad Start button (JoystickButton7)
        if (!enabledBefore)
        {
            layoutAnim.Play("FadeIN");
            enabledBefore = true;
        }
        if (enabledBefore)
        {
            layoutAnim.Play("FadeOut");
            enabledBefore = false;
        }
    }
}
