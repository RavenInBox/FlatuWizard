using UnityEngine;

public class PlayerDetectDamange : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] LayerMask damangeLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & damangeLayer) != 0)
        {
            controller.GetDamange(120.0f);
        }
    }
}
