using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBossStatus
{
    void HealthBarSet();
    void TakeDamage(float Damage);
}
