using UnityEngine;
using UnityEngine.UI;

public class TorretaController : MonoBehaviour
{
    [SerializeField] float damange;
    [SerializeField] float range;
    [SerializeField] float HP;
    [SerializeField] float maxHP;
    [SerializeField] Image healthBarFill;
    [SerializeField] float smoothSpeed = 15f;
    float targetFillAmount;

    [SerializeField] int count = 1;

    [SerializeField] MasterGame master;
    [SerializeField] TorretaAnimatorController animatorController;
    [SerializeField] MinionController[] minions = new MinionController[0];

    [SerializeField] PlayerController Player;
    [SerializeField] BulletController bulletController;
    [SerializeField] ManagerSonido managerSonido;

    MinionController minionTarget;

    [SerializeField] bool playerFocus = false;
    [SerializeField] bool waitHit = false;
    int target = 0;

    string CurrentState = "";

    const string DEAD = "MuerteTorreta";
    const string ATK = "AtaqueTorreta";
    const string IDLE = "IdleTorreta";
    const string HIT = "ImpactoTorreta";

    private void Awake()
    {
        HP = maxHP;
    }

    void Start()
    {
        if (healthBarFill != null)
        {
            targetFillAmount = HP / maxHP;
            healthBarFill.fillAmount = targetFillAmount;
        }
    }

    public void MyUpdate()
    {
        CalculateHpBarr();
        SearchTarget();
    }

    void CalculateHpBarr()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetFillAmount, smoothSpeed * Time.deltaTime);
        }
    }

    public void Damange(float damange)
    {
        if (damange > HP)
        {
            VFX_SFX(DEAD);
            Invoke(nameof(EndGame), 3);
        }
        else
        {
            HP -= damange;
            targetFillAmount = HP / maxHP;
        }
    }

    void EndGame()
    {
        master.Victory();
    }

    private void SearchTarget()
    {
        if (waitHit)
        {
            return;
        }
        
        float playerDistance = (Player.transform.position - transform.position).sqrMagnitude;

        if (playerDistance > range && playerFocus)
        {
            target = 0;
            count = 1;
            playerFocus = false;
        }

        if (playerDistance <= range && playerFocus)
        {
            target = 1;
            VFX_SFX(ATK);
            waitHit = true;
            return;
        }

        foreach (MinionController m in minions)
        {
            if (m.HP < damange) return;

            if (target != 2)
            {
                target = 2;
            }

            float minionDistance = (m.transform.position - transform.position).sqrMagnitude;

            if (minionDistance <= range)
            {
                VFX_SFX(ATK);
                minionTarget = m;
                waitHit = true;
                return;
            }
        }

        if (playerDistance <= range)
        {
            target = 1;
            playerFocus = true;
            VFX_SFX(ATK);
            waitHit = true;
        }
        else
        {
            VFX_SFX(IDLE, false, true);
        }
    }

    private void Attack()
    {
        if (target == 0) return;

        switch (target)
        {
            case 1:
                target = 0;
                bulletController.AsignTarget(Player.transform);
                minionTarget = null;
                break;
            case 2:
                bulletController.AsignTarget(minionTarget.transform);
                break;
        }
    }

    public void IsHit()
    {
        if(!minionTarget)
        {
            float SetDamange = damange * count;
            Player.GetDamange(SetDamange);
            count++;
        }
        else
        {
            minionTarget.GetDamange(damange);
        }

        VFX_SFX(HIT, true, false);

        waitHit = false;

        CurrentState = "";
    }

    private void CallIdleSound()
    {
        managerSonido.Play(IDLE);
    }

    private void VFX_SFX(string state, bool onlySound = false, bool onlyAnimation = false)
    {
        if (CurrentState == state) return;

        if (!onlyAnimation) managerSonido.Play(state);

        if (!onlySound) animatorController.CallAnimatorState(state);

        CurrentState = state;
    }
}
