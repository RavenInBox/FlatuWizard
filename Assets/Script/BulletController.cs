using UnityEngine;
using UnityEngine.VFX;

public class BulletController : MonoBehaviour
{
    public Transform target;
    public float defaultSpeed;
    public float acceleration;
    public float currentSpeed;
    public float stopDistance;
    public float rotationSpeed;
    public float rateEff;
    public TorretaController torreta;
    

    private void Awake()
    {
        currentSpeed = defaultSpeed;
    }

    public void MyUpdate()
    {
        Shoot();
    }

    private void Shoot()
    {
        if (!target) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < stopDistance)
        {

            torreta.IsHit();
            target = null;
            transform.localPosition = new Vector3(0, 11, 0);
            currentSpeed = defaultSpeed;
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        currentSpeed += acceleration * Time.deltaTime;
        transform.position += currentSpeed * Time.deltaTime * direction;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void AsignTarget(Transform t)
    {
        target = t;
    }
}
