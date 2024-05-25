using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyTank
{
    public abstract int GetTankLevel();

    public abstract void EnemyHit();
}
