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
    //    base.Die();  // Üst sýnýfýn Die fonksiyonunu çaðýr

    //    isDead = true;

    //    // Alt sýnýfta ek iþlevler: NavMeshAgent'i devre dýþý býrak
    //    if (navMeshAgent != null)
    //    {
    //        navMeshAgent.enabled = false;
    //        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    //    }
    //    // Diðer bileþenleri devre dýþý býrakma iþlemleri burada yapýlabilir
    //}
}
