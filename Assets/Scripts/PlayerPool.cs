using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPool : Pool
{
    public int _bulletPoolSize = 5;
    public int _minePoolSize = 2;

    void Start()
    {
        //Generating bulletPool
        for (int i = 0; i < _bulletPoolSize; i++)
        {
            _bulletPool.Add(Instantiate(_bulletPrefab));

            Projectile bulletProperties = _bulletPool[i].GetComponent<Projectile>();
            bulletProperties._pool = this.GetComponent<PlayerPool>();
            bulletProperties.gameObject.layer = LayerMask.NameToLayer("PlayerBullet");

            _bulletPool[i].SetActive(false);
        }

        //Generating minePool
        for (int i = 0; i < _minePoolSize; i++)
        {
            _minePool.Add(Instantiate(_minePrefab));
            _minePool[i].GetComponent<Mine>()._minePool = this.GetComponent<PlayerPool>();
            _minePool[i].SetActive(false);
        }
    }
}

