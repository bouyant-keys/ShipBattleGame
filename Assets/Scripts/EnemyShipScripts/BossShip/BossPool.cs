using System.Collections.Generic;
using UnityEngine;

public class BossPool : MonoBehaviour, Pool
{
    [SerializeField] ShipTypes _bossShip;

    public List<GameObject> _bulletPool = new List<GameObject>();
    public GameObject _bulletPrefab;

    public List<GameObject> _superMinePool = new List<GameObject>();
    public GameObject _superMinePrefab;

    public int _bulletPoolSize = 5;
    public int _minePoolSize = 2;

    private void Start()
    {
        //Generating bulletPool
        for (int i = 0; i < _bulletPoolSize; i++)
        {
            _bulletPool.Add(Instantiate(_bulletPrefab));

            _bulletPool[i].GetComponent<Projectile>()._pool = this;
            _bulletPool[i].layer = LayerMask.NameToLayer("EnemyBullet");

            _bulletPool[i].SetActive(false);
        }

        //Generating minePool
        for (int i = 0; i < _minePoolSize; i++)
        {
            _superMinePool.Add(Instantiate(_superMinePrefab));
            _superMinePool[i].SetActive(false);
        }
    }
    
    public virtual void GetBulletFromPool(Transform trans, ShipTypes tank)
    {
        Debug.Log("Method is unused, use GetBulletFromPool(Vector3 position, Quaternion rotation) instead.");
    }
    
    public virtual void GetBulletFromPool(Vector3 position, Quaternion rotation)
    {
        if (_bulletPool.Count <= 0) return;

        GameObject projectile = _bulletPool[_bulletPool.Count - 1];
        projectile.transform.position = position;
        projectile.transform.rotation = rotation;
        projectile.gameObject.SetActive(true);

        //Idea is that boss bullets shouldn't riccochet, so just launch instead of overwriting
        projectile.GetComponent<Projectile>().Launch(_bossShip._bulletSpeed, projectile.transform.forward);

        _bulletPool.RemoveAt(_bulletPool.Count - 1);
    }

    public virtual void GetMineFromPool(Vector3 pos)
    {
        if (_superMinePool.Count == 0) return;

        GameObject mine = _superMinePool[_superMinePool.Count - 1];
        mine.transform.position = pos;
        mine.transform.rotation = Quaternion.identity; //Might be unnecessary
        mine.gameObject.SetActive(true);

        mine.GetComponent<SuperMine>().LayMine();
        _superMinePool.RemoveAt(_superMinePool.Count - 1);
    }

    public void ReturnObjToPool(GameObject obj)
    {
        obj.SetActive(false);

        if (obj.GetComponent<Projectile>()) _bulletPool.Add(obj);
        else if (obj.GetComponent<SuperMine>()) _superMinePool.Add(obj);
        else print("GameObject doesn't belong to a pool and cannot be returned one.");
    }
}
