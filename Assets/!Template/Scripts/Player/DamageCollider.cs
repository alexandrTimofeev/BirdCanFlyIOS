using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageCollider : MonoBehaviour
{
    [SerializeField] private List<string> colliderTags = new List<string>();

    public event Action<DamageContainer> OnDamage;

    public List<string> ColliderTags => colliderTags;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DamageContainer damageContriner))
        {
            damageContriner.CollisionDamageCollider(this);
            if (damageContriner.ContainsAnyTags(colliderTags.ToArray()))
            {
                Hit(damageContriner);
            }
        }
    }

    private void Hit(DamageContainer damageContriner)
    {
        if (gameObject.activeInHierarchy == false)
            return;

        damageContriner.HitDamageCollider(this);
        OnDamage?.Invoke(damageContriner);
    }
}
