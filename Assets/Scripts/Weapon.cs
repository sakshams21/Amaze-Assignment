using UnityEngine;

public class Weapon : MonoBehaviour
{
    private BoxCollider m_Collider;

    private void Start()
    {
        m_Collider = GetComponent<BoxCollider>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy=other.GetComponentInParent<EnemyController>();

            if (enemy != null)
            {
                enemy.Hit(GameManager.Instance.CurrentAttack);
                EventManager.TriggerEnemyHit(other.ClosestPoint(transform.position));
                m_Collider.enabled = false;
            }
        }
    }
}
