using UnityEngine;

public class TorretaAnimatorController : MonoBehaviour
{
    public Animator animator;

    public void CallAnimatorState(string newState)
    {
        animator.Play(newState);
    }
}
