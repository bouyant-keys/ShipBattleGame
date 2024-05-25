using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    BossPool _bossPool;
    Transform _playerTrans;

    [SerializeField] float _spawnObjInRadius = 3f;

    void Awake()
    {
        _bossPool = GameObject.FindGameObjectWithTag("BossPool").GetComponent<BossPool>();
        _playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void LaunchSuperMines(float spawnRange)
    {
        int mineNum = Mathf.FloorToInt(UnityEngine.Random.Range(1f, 4f)); //Results in 1, 2, or 3
        Vector3[] attackPoints = FindAttackPoints(spawnRange, mineNum);

        for (int i = 0; i < mineNum; i++)
        {
            _bossPool.GetMineFromPool(attackPoints[i]);
        }
    }

    public void LaunchBulletRings(float spawnRange)
    {
        int instanceNum = Mathf.FloorToInt(UnityEngine.Random.Range(1f, 4f)); //Results in 1, 2, or 3
        Vector3[] attackPoints = FindAttackPoints(spawnRange, instanceNum);

        int bulletNum = _bossPool._bulletPoolSize / instanceNum;

        for (int i = 0; i < instanceNum; i++) //For each instance of bullet rings
        {
            float angle = 0f;
            float angleStep = 360f/bulletNum;

            for (int j = 0; j < bulletNum; j++) //Get bullets from pool and assign correct rotation
            {
                Quaternion bulletDir = Quaternion.Euler(0f, angle, 0f);
                _bossPool.GetBulletFromPool(attackPoints[i], bulletDir);

                angle += angleStep;
            }
        }
    }

    private Vector3[] FindAttackPoints(float range, int points)
    {
        transform.position = _playerTrans.position;
        Vector3[] attackPoints = new Vector3[points];
        for (int i = 0; i < attackPoints.Length; i++)
        {
            attackPoints[i] = UnityEngine.Random.insideUnitCircle * range;
        }

        return attackPoints;
    }
}
