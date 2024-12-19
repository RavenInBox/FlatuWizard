using UnityEngine;
using UnityEngine.AI;

public class MinionController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;
    [SerializeField] Transform spawnPosition;

    public float HP;
    [SerializeField] float MaxHP;
    [SerializeField] float damange;
    [SerializeField] float speed;
    [SerializeField] float moveSpeed;

    string currentState = "";
    const string DEAD = "MuerteMinion";
    const string ATK = "AtaqueMinion";
    const string IDLE = "IdleMinion";

    const float INITIAL_INVOKE_DELAY = 1.0f;
    const float REPEATING_INTERVAL = 0.3f;

    private void Awake()
    {
        agent.speed = moveSpeed;
    }

    private void OnDisable()
    {
        SetDefaultMinion();
    }

    public void StartActions()
    {
        CallAnimation(IDLE);
        agent.SetDestination(target.position);
        InvokeRepeating(nameof(SearchTower), INITIAL_INVOKE_DELAY, REPEATING_INTERVAL);
    }

    private void SearchTower()
    {
        float goal = (target.transform.position - transform.position).sqrMagnitude;

        if (goal < 0.3f)
        {
            CallAnimation(ATK);
            CanceThislInvoke();
        }
    }

    private void CanceThislInvoke()
    {
        CancelInvoke(nameof(SearchTower));
    }

    private void SetDefaultMinion()
    {
        HP = MaxHP;
        transform.position = spawnPosition.position;
    }

    private void Dead()
    {
        gameObject.SetActive(false);
    }

    public void GetDamange(float damange)
    {
        HP -= damange;

        if (damange > HP)
        {
            CallAnimation(DEAD);
        }
    }

    private void CallAnimation(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);
    }
}
