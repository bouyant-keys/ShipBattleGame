using UnityEngine;

public interface Pool
{
    public abstract void GetBulletFromPool(Transform trans, ShipTypes tank);

    public abstract void GetMineFromPool(Vector3 pos);

    public abstract void ReturnObjToPool(GameObject obj);
}
