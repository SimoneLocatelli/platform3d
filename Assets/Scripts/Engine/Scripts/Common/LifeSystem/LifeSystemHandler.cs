using UnityEngine;

public static class LifeSystemHandler
{
    internal static void ApplyDamage(Collision2D collision, int damage)
    {
        if (collision == null)
            return;

        ApplyDamage(collision.gameObject, damage);
    }

    public static void ApplyDamage(Collider2D collider, int damage)
    {
        if (collider == null)
            return;

        ApplyDamage(collider.gameObject, damage);
    }

    public static void ApplyDamage(GameObject gameObject, int damage)
    {
        if (gameObject == null)
            return;

        var lifeSystem = gameObject.GetComponent<LifeSystem>();

        if (lifeSystem == null)
            return;

        lifeSystem.ApplyDamage(damage);
    }
}