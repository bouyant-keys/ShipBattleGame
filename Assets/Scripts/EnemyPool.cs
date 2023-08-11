using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : Pool
{
    public int _bulletPoolSize = 5; //Pool size should change per level & number of tanks
    public int _minePoolSize = 4; //Mine pool should be max 5-6 for multiple tanks and 4 for one tank

    private void Start()
    {
        //Generating bulletPool
        for (int i = 0; i < _bulletPoolSize; i++)
        {
            _bulletPool.Add(Instantiate(_bulletPrefab));

            Projectile bulletProperties = _bulletPool[i].GetComponent<Projectile>();
            bulletProperties._pool = this.GetComponent<EnemyPool>();
            bulletProperties.gameObject.layer = LayerMask.NameToLayer("EnemyBullet");

            _bulletPool[i].SetActive(false);
        }

        //Generating minePool
        for (int i = 0; i < _minePoolSize; i++)
        {
            _minePool.Add(Instantiate(_minePrefab));
            _minePool[i].GetComponent<Mine>()._minePool = this.GetComponent<EnemyPool>();
            _minePool[i].SetActive(false);
        }
    }
}
