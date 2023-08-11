using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkPoint : MonoBehaviour
{
    public EnemyNavMesh _enemyTank;

    private void Update()
    {
        gameObject.transform.position = _enemyTank._generatedWalkPoint;
    }
}
