using UnityEngine;

public class EnemyAttackSystem : MonoBehaviour
{
    [SerializeField] private int damage = 25;//Burasi silahtan gelen damagei almali
    private bool isAttacking = false;

    //Bu metotta her karede damage veriyor ama boyle bir seyin olmamasi lazim, need to fix
    private void OnTriggerStay(Collider other)
    {
        if (isAttacking && other.CompareTag("Player")//Vector distance yapan kod günü kurtarýr ama daha iyi bir yöntem lazým. 
            && Vector3.Distance(transform.position, other.transform.position) <= 2f)//If attacking and obj tag is enemy
        {
            PlayerManager.Instance.TakeDamage(damage);
            isAttacking = false;
        }
    }

    /// <summary>
    /// StartAttack fonksiyonu icin elimizdeki weaponun 
    /// sahip oldugu damage parametresini verip, o sekilde dmg vermeliyiz.
    /// Ustelik bu fonksiyonu silahin saldiri animasyonunda bir aralik icinde vermeliyiz
    /// </summary>
    public void StartAttack()//Animation Event
    {
        isAttacking = true;
    }

    // Saldiri bittiginde tetiklenir
    public void EndAttack()//Animation Event
    {
        isAttacking = false;
    }
}
