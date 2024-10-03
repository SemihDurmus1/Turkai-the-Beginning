using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    private float currentHealth;
    protected bool isDead = false;

    [Header("Damage")]

    private EnemyAttackSystem _enemyAttackSystem;

    public int attackDamage = 20;
    public float attackRange = 2f;  // Saldýrý menzili
    public float attackCooldown = 1.5f;  // Saldýrý arasýndaki bekleme süresi
    private float lastAttackTime;

    [Header("Animator")]
    public Animator enemyAnimator;

    [Header("Control Parameters")]
    protected bool canMove = true;

    public Rigidbody[] ragdollBodies;

    protected virtual void Start()
    {
        currentHealth = maxHealth;  // Düþman canýný maksimum yap
        enemyAnimator = GetComponent<Animator>();
        _enemyAttackSystem = GetComponent<EnemyAttackSystem>();
    }

    // Hasar alma fonksiyonu
    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Arissa take dmg, new health: " + currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            enemyAnimator.SetTrigger("Hurt");
        }
    }

    // Ölüm fonksiyonu
    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;

        int deathState = Random.Range(0, 5);

        enemyAnimator.SetInteger("deathState", deathState);
        enemyAnimator.SetTrigger("Die");
        
        // Saldýrý animasyonunu durdur, böylece saldýrý event'leri çalýþmaz
        enemyAnimator.SetBool("isAttacking", false);
        enemyAnimator.ResetTrigger("Attack");

        Destroy(gameObject.GetComponent<Rigidbody>());
        Destroy(gameObject.GetComponent<NavMeshAgent>());

        //_enemyAttackSystem.enabled = false;

        CapsuleCollider[] colliders = gameObject.GetComponents<CapsuleCollider>();

        foreach (CapsuleCollider collider in colliders)
        {
            Destroy(collider);
        }

        foreach (var rb in ragdollBodies) //Ragdoll activate
        {
            rb.isKinematic = false;
        }
        //Destroy(this);
    }

    // Oyuncuya saldirma
    public virtual void StartAttackAnim()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            enemyAnimator.SetBool("isAttacking", true);
            enemyAnimator.SetTrigger("Attack");  // Saldýrý animasyonunu tetikle
        }
    }

    private void AttackAnimEvent()//Animation Event
    {
        //if (isDead)
        //{
        //    enemyAnimator.SetBool("isAttacking", false);
        //    _enemyAttackSystem.EndAttack();
        //    return; 
        //}

        _enemyAttackSystem ??= GetComponentInChildren<EnemyAttackSystem>();

        if (_enemyAttackSystem != null)
        {
            _enemyAttackSystem.StartAttack();
        }

        //PlayerManager.Instance.GetComponent<PlayerManager>().TakeDamage(attackDamage);
        lastAttackTime = Time.time;
    }

    private void StopAttackAnimEvent()//Animation Event
    {
        enemyAnimator.SetBool("isAttacking", false);
        if (_enemyAttackSystem != null)
        {
            _enemyAttackSystem.EndAttack();
        }
    }

    private void CantMove()//Animation Event
    {
        canMove = false;
    }

    private void CanMove()//Animation Event
    {
        canMove = true;
    }
}
