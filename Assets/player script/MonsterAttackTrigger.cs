using UnityEngine;

public class MonsterAttackTrigger : MonoBehaviour
{
    [Header("공격력 설정")]
    public int attackDamage = 1;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealthSystem playerHealth = other.GetComponent<PlayerHealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }
}
