using UnityEngine;

public class AnimationEventListener : MonoBehaviour
{
    public bool IsAttacking;


    public void OnPrimaryAttackEnd()
    {
        IsAttacking = false;
    }

    public void OnSecondaryAttackEnd()
    {
        IsAttacking = false;
    }

    public void OnSwordColliderEnable()
    {
        GameManager.Instance.ChangeWeaponComponentStatus(true);
    }

    public void OnSwordColliderDisable()
    {
        GameManager.Instance.ChangeWeaponComponentStatus(false);
    }
}
