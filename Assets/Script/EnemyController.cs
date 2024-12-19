using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float SearchSpeed = 0.0f;
    [SerializeField] float maxHP = 1200.0f;
    [SerializeField] float currentHP = 1200.0f;
    [SerializeField] float resetInTime = 3.0f;
    [SerializeField] float healRate = 0.5f;
    [SerializeField] Image healthBarFill;
    [SerializeField] Animator animator;
    [SerializeField] Animator animatorShoot;
    [SerializeField] Transform[] MovePoint;
    [SerializeField] GameObject shootPoint;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] PlayerController playerController;
    [SerializeField] ManagerSonido sonido;
    [SerializeField] GameObject cube;

    float targetFillAmount;

    public bool IsDown = false;
    bool inState = false;
    public bool SSync = false;
    public bool test = true;

    static readonly float smoothSpeed = 16.0f;
    static readonly string DOWN = "Down";
    static readonly string LAS = "Laser";
    static readonly string RAY = "Rayn";
    static readonly string SHO = "Shoot";
    static readonly string CHO = "Cshoot";
    static readonly string WLK = "Walk";

    string currentState = "";
    int currentPoint = 0;

    private List<int> attackOrder = new List<int>();
    private int currentIndex = -1;

    private void Awake()
    {
        currentHP = maxHP;

        if (healthBarFill != null)
        {
            targetFillAmount = currentHP / maxHP;
            healthBarFill.fillAmount = targetFillAmount;
        }

        InitializeAttackOrder();

        test = true;
        Invoke(nameof(StartPlay), 8);
    }

    private void StartPlay()
    {
        test = false;
    }

    public void MyUpdate()
    {
        if (test) return;

        if (IsDown)
        {
            cube.SetActive(false);
            CallAnimator(DOWN);
            agent.speed = 0;
            Heal();
            return;
        }

        if (playerController.healActive)
        {
            return;
        }

        HPbarr();
        NewState();
        FollowPlayerRotation();
        ShootFollowPlayer();
        MakeAction();
    }

    void InitializeAttackOrder()
    {
        attackOrder.Clear();
        for (int i = 0; i < 3; i++)
        {
            attackOrder.Add(i);
        }

        ShuffleList(attackOrder);
    }

    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void NewState()
    {
        if (inState) return;
        if (currentState == WLK) return;
        int GetPoint = Random.Range(0, MovePoint.Length);
        if (currentPoint == GetPoint) GetPoint++;
        Vector3 nextPoint = MovePoint[GetPoint].position;

        agent.SetDestination(nextPoint);
        CallAnimator(WLK);
    }

    public int GetNextAttack()
    {
        currentIndex++;

        if (currentIndex >= attackOrder.Count)
        {
            InitializeAttackOrder();
            currentIndex = 0;
        }

        return attackOrder[currentIndex];
    }

    void MakeAction()
    {
        if (inState) return;
        int selectAttak = GetNextAttack();
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                switch (selectAttak)
                {
                    case 0:
                        Rayn();
                        inState = true;
                        break;
                    case 1:
                        Laser();
                        inState = true;
                        break;
                    case 2:
                        Shoot();
                        inState = true;
                        break;
                }
            }
        }
    }

    void Rayn()
    {
        CallAnimator(RAY);
    }

    void Laser()
    {
        CallAnimator(LAS);
    }

    void Shoot()
    {
        CallAnimator(CHO);
    }

    void SyncShootAnimation()
    {
        shootPoint.SetActive(true);
        animatorShoot.Play(SHO);
        Invoke(nameof(SyncShootSound), 1);
    }

    void SyncShootSound()
    {
        sonido.Play("CHo");
    }

    void CallShootAnimation()
    {
        shootPoint.SetActive(false);
    }

    void ShootFollowPlayer()
    {
        if (!SSync) return;
        shootPoint.transform.position = playerController.transform.position;
    }

    void FollowPlayerRotation()
    {
        if (currentState == WLK)
        {
            agent.angularSpeed = 500;
            return;
        }
        agent.angularSpeed = 0;
        Vector3 directionToTarget = playerController.transform.position - transform.position;
        directionToTarget.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * SearchSpeed);
    }

    void CallReset()
    {
        Invoke(nameof(ResetState), resetInTime);
    }

    void ResetState()
    {
        inState = false;
        currentState = "";
    }

    void HPbarr()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetFillAmount, smoothSpeed * Time.deltaTime);
        }
    }

    public void GetDamange(float value, bool stun = false, bool pest = false, bool slow = false)
    {
        if (!IsDown)
        {
            if (value > currentHP)
            {
                currentHP = 0;
                healthBarFill.fillAmount = 0;
                IsDown = true;
            }
            else
            {
                currentHP -= value;
                targetFillAmount = currentHP / maxHP;
            }
        }
    }

    public void Heal()
    {
        if (currentHP < maxHP)
        {
            currentHP += healRate * Time.deltaTime;
            targetFillAmount = currentHP / maxHP;
            HPbarr();
        }
        else
        {
            currentHP = maxHP;
            CallAnimator("Idle");
            agent.speed = 10;
            inState = false;
            SSync = false;
            IsDown = false;
        }
    }

    void CallAnimator(string state)
    {
        if (currentState == state) return;
        animator.Play(state);
        sonido.Play(state);
        currentState = state;
    }
}
