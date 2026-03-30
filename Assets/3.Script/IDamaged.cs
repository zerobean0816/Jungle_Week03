using UnityEngine;
using System;

public interface IDamaged
{
    event Action<float, Vector3> OnDamaged;

    public void TakeDamage(float damageAmount);
}
