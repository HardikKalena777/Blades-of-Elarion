using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public Animator animator;
    public GameObject weaponObject; // Your weapon GameObject (e.g., sword on back/hip)
    public Transform weaponHipSocket;
    public Transform weaponHandSocket;


    private bool isWeaponDrawn = false;
    public bool IsWeaponDrawn => isWeaponDrawn;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        SheathWeaponInstantly();
    }

    private void Update()
    {
        //ToggleWeapon();
    }

    public void ToggleWeapon()
    {
        //if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton8))
        //{
            if (isWeaponDrawn)
            {
                animator.SetTrigger("Sheath");
            }
            else
            {
                animator.SetTrigger("Draw");
            }

        //}
            isWeaponDrawn = !isWeaponDrawn;
    }

    // These methods are called via animation events
    public void AttachWeaponToHand()
    {
        if (weaponObject && weaponHandSocket)
        {
            weaponObject.transform.SetParent(weaponHandSocket);
            weaponObject.transform.localPosition = Vector3.zero;
            weaponObject.transform.localRotation = Quaternion.identity;
        }
    }

    public void AttachWeaponToHip()
    {
        if (weaponObject && weaponHipSocket)
        {
            weaponObject.transform.SetParent(weaponHipSocket);
            weaponObject.transform.localPosition = Vector3.zero;
            weaponObject.transform.localRotation = Quaternion.identity;
        }
    }

    private void SheathWeaponInstantly()
    {
        isWeaponDrawn = false;
        AttachWeaponToHip();
    }
}
