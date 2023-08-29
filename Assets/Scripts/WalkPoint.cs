using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkPoint : MonoBehaviour
{
    public MobileEnemyNavigation _enemyTank;

    private void Update()
    {
        gameObject.transform.position = _enemyTank._generatedWalkPoint;
    }
}
