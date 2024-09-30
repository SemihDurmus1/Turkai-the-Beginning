using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    [SerializeField]public Weapon weaponReference;

    //[SerializeField] private int damage = 100;//Burasi silahtan gelen damagei almali
    private bool isAttacking = false;

    private void OnTriggerStay(Collider other)
    {
        if (isAttacking && other.CompareTag("Enemy"))//If attacking and obj tag is enemy
        {
            //Find enemyscript and getdmg
            EnemyBase enemyScript = other.GetComponent<EnemyBase>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(weaponReference.attackDamage);
            }
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
        Debug.Log("Attack basladi");
    }

    // Saldiri bittiginde tetiklenir
    public void EndAttack()//Animation Event
    {
        isAttacking = false;
    }

}
