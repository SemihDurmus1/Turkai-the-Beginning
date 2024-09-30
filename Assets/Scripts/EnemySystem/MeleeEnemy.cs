//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MeleeEnemy : EnemyAI
{
    //[SerializeField] private int damage = 25;//Burasi silahtan gelen damagei almali



    public void MeleeAttack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
        {
            player.GetComponent<PlayerManager>().TakeDamage(attackDamage);
        }
    }

    // Die fonksiyonunu override ediyoruz
    //protected override void Die()
    //{
    //    base.Die();  // �st s�n�f�n Die fonksiyonunu �a��r

    //    isDead = true;

    //    // Alt s�n�fta ek i�levler: NavMeshAgent'i devre d��� b�rak
    //    if (navMeshAgent != null)
    //    {
    //        navMeshAgent.enabled = false;
    //        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    //    }
    //    // Di�er bile�enleri devre d��� b�rakma i�lemleri burada yap�labilir
    //}
}
