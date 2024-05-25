using UnityEngine;

public class BulletShape : MonoBehaviour
{
    BossPool _pool;

    [SerializeField] ParticleSystem _circleEffect;

    public enum FireShape {Circle, Arc}

    public void Fire(FireShape shape, int bullets)
    {
        if (shape == FireShape.Circle) FireCircle(bullets);
        else if (shape == FireShape.Arc) FireArc(bullets);
    }

    private void FireCircle(int bullets)
    {
        float angle = 0f;
        float angleStep = 360f/bullets;

        for (int i = 0; i < bullets; i++)
        {
            Quaternion bulletDir = Quaternion.Euler(0f, angle, 0f);
            _pool.GetBulletFromPool(transform.position, bulletDir);

            angle += angleStep;
        }
    }

    private void FireArc(int bullets)
    {
        float angle = 0f;
        float angleStep = 60f/bullets;

        for (int i = 0; i < bullets; i++)
        {
            Quaternion bulletDir = Quaternion.Euler(0f, angle, 0f);
            _pool.GetBulletFromPool(transform.position, bulletDir);

            angle += angleStep;
        }
    }
}
