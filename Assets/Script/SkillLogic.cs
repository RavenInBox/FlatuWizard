using UnityEngine;

public class SkillLogic : MonoBehaviour
{
    float skillDamange;
    bool Stun = false;
    bool Pest = false;
    bool Slow = false;
    Vector3 target;

    [SerializeField] int XPmount;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] PlayerController playerController;
    [SerializeField] EnemyController enemyController;
    [SerializeField] Animator animator;
    [SerializeField] ManagerSonido sonido;
    [SerializeField] Transform myParent;
    [SerializeField] int speed;
    [SerializeField] string VSFXCall;
    [SerializeField] string EspecialSFX;
    [SerializeField] bool isAShoot;
    [SerializeField] bool isAShootAndSkill;

    const string DFL = "Default";
    const string OPS = "OPS";
    const string RFB = "RFB";
    const string RF = "RF";

    public void SkillPrepare(Vector3 t, float value, bool stun = false, bool pest = false, bool slow = false)
    {
        skillDamange = value;
        Stun = stun;
        Pest = pest;
        Slow = slow;
        target = t;
    }

    public void SkillLaunch()
    {
        if (!isAShoot)
        {
            Vector3 FixTarget = new(target.x, 0, target.z);
            transform.position = FixTarget;
        }
        else transform.position = myParent.position;
        animator.Play(VSFXCall);
    }

    public void MyUpdate()
    {
        FowarMove();
    }

    void FowarMove()
    {
        if (isAShoot)
        {
            float Ypos = isAShootAndSkill ? 0f : 3.1f;
            Vector3 notY = new(target.x, Ypos, target.z);
            Vector3 direccion = (notY - transform.position).normalized;
            transform.Translate(speed * Time.deltaTime * direccion, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            playerController.GetXP(XPmount);
            enemyController.GetDamange(skillDamange, Stun, Pest, Slow);

            if (isAShoot && !isAShootAndSkill)
            {
                CallResetAnimator();
                gameObject.SetActive(false);
            }
        }
    }

    void CallResetAnimator()
    {
        animator.Play(DFL);
    }

    void DisableThis()
    {
        gameObject.SetActive(false);
    }

    void REventCall()
    {
        sonido.Play(RF);
    }

    void RImpactEvent()
    {
        sonido.Play(RFB);
    }

    void REveneEnd()
    {
        Invoke(nameof(CallOops), 0.7f);
        animator.Play(DFL);
        gameObject.SetActive(false);
    }

    void CallOops()
    {
        sonido.Play(OPS);
    }

    void EEventCall()
    {
        isAShoot = false;
        sonido.Play(EspecialSFX);
    }

    void EEndEvent()
    {
        isAShoot = true;
        animator.Play(DFL);
        gameObject.SetActive(false);
    }
}
