using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBasicAttak : MonoBehaviour
{
    public Transform launchPoint;
    public Transform target;
    public float speed = 10f;
    public float impactThreshold = 0.5f;
    private bool hasImpacted = false;

    private void OnEnable()
    {
        transform.position = launchPoint.position;
    }

    public void MyUpdate()
    {
        if (target == null || hasImpacted) return; 

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);

        if ((transform.position - target.position).sqrMagnitude <= impactThreshold * impactThreshold)
        {
            gameObject.SetActive(false);
            //transform.position = launchPoint.position;
        }
    }
}
