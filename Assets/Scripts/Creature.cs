using UnityEngine;

public abstract class Creature
{
    private float health;
    private float attackDamage;

    public abstract void GetDamage();
    public abstract void Attack();
}
