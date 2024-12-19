using System;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [Serializable]
    private struct Reference
    {
        public Animator animator;
        public string[] state;
        public string currentState;
    }

    [SerializeField] private Reference[] reference = new Reference[0];

    public void AnimatorState(int sRef, string newState)
    {
        if (reference[sRef].currentState == newState) return;
        reference[sRef].animator.Play(newState);
        reference[sRef].currentState = newState;
    }
}
