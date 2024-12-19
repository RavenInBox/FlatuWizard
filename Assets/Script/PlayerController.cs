using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Componentes y dependencias de Unity
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] KeyBoardMouseController keyC;
    [SerializeField] InibidorController inibidor;
    [SerializeField] TorretaController torretaController;
    [SerializeField] EnemyController enemyController;
    [SerializeField] ManagerSonido sonido;
    [SerializeField] SkillsReference skillsReference;

    // Capas
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask torreta;
    [SerializeField] LayerMask enemy;

    // Transformaciones y objetos
    [SerializeField] GameObject ShieldObj;
    [SerializeField] GameObject SpeedBstObj;
    [SerializeField] GameObject[] BscEnemy;
    [SerializeField] GameObject[] BscTorreta;
    [SerializeField] Transform HealArea;

    // Estadísticas del personaje
    [SerializeField] float attackRange;
    [SerializeField] float damange;
    [SerializeField] float moveSpeed;
    [SerializeField] float currentHP;
    [SerializeField] float maxHP;
    [SerializeField] float healRate;
    [SerializeField] float maxGas;
    [SerializeField] float currentGas;
    float xp = 0;
    float topXP = 100;
    [SerializeField] int lvl = 1;
    static readonly int topLvl = 18;

    // Sistema de habilidades
    [SerializeField] float cooldownTimeQ = 3f;
    [SerializeField] float cooldownTimeW = 3f;
    [SerializeField] float cooldownTimeE = 3f;
    [SerializeField] float cooldownTimeR = 3f;
    [SerializeField] float cooldownTimeF = 3f;
    [SerializeField] float cooldownTimeD = 3f;
    [SerializeField] float GlobalCooldownTime = 1f;
    [SerializeField] float HealAreaCooldownTime = 300.0f;
    [SerializeField] float healTime = 10.0f;
    [SerializeField] float QGas;
    [SerializeField] float WGas;
    [SerializeField] float EGas;
    [SerializeField] float RGas;
    [SerializeField] int Wrange;
    [SerializeField] int Erange;
    [SerializeField] int Rrange;
    [SerializeField] float QscaledByLevel = 1.3f;
    [SerializeField] float WscaledByLevel = 1.1f;
    [SerializeField] float EscaledByLevel = 0.3f;
    [SerializeField] float RscaledByLevel = 3.1f;
    float skillDamange = 0;

    // Cooldowns actuales
    float nextUseTimeQ = 0f;
    float nextUseTimeW = 0f;
    float nextUseTimeE = 0f;
    float nextUseTimeR = 0f;
    float nextUseTimeF = 0f;
    float nextUseTimeD = 0f;
    float nextGlobalUse = 0f;
    float nextUseHeal = 300.0f;

    // Barras de progreso y UI
    [SerializeField] Image healthBarFill;
    [SerializeField] Image GasBarFill;
    [SerializeField] Image cooldownQ;
    [SerializeField] Image cooldownW;
    [SerializeField] Image cooldownE;
    [SerializeField] Image cooldownR;
    [SerializeField] Image cooldownD;
    [SerializeField] Image cooldownF;
    [SerializeField] Image cooldowHeal;
    [SerializeField] Image unQ;
    [SerializeField] Image unW;
    [SerializeField] Image unE;
    [SerializeField] Image unR;
    [SerializeField] Image unOne;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI lvlQ;
    [SerializeField] TextMeshProUGUI lvlW;
    [SerializeField] TextMeshProUGUI lvlE;
    [SerializeField] TextMeshProUGUI lvlR;

    // Estados del personaje
    string currentState = "Idle";
    static readonly int IdleHash = Animator.StringToHash("Idle");
    bool isDead = false;
    public bool healActive = false;

    // Constantes de estado
    static readonly string NCSK = "ncsk";
    static readonly string IDLE = "Idle";
    static readonly string BASIC = "Basic";
    static readonly string DEAD = "Dead";
    static readonly string LVLUP = "LVLUP";
    static readonly string QAT = "QAT";
    static readonly string WAT = "WAT";
    static readonly string EAT = "EAT";
    static readonly string RAT = "RAT";
    static readonly string WLK = "Walk";
    static readonly string UP = "up";
    static readonly string CUP = "cup";
    static readonly string SHI = "Shield";
    static readonly string BOS = "BoostS";
    static readonly string MAN = "Manolo";

    // Control de habilidades y rangos
    bool RangeSkill = false;
    int Qlvl;
    int Wlvl;
    int Elvl;
    int Rlvl;
    public int countSkills = 1;
    int type;
    SkillLogic SKAnimation;

    // Booleanos de control
    bool BscAtk;
    bool clicGround = true;
    bool stun = false;
    bool pest = false;
    bool slow = false;
    bool shieldIsActive;
    bool speedBoost;
    bool enemyt;
    bool SkillUp;

    [SerializeField] float cooldownCallMinions = 90f;
    float nextUseTimeCallMinions = 0f;

    // Colores y otros atributos
    [SerializeField] Color newColor = new (1f, 0f, 0f, 0.5f);
    [SerializeField] Color blakColor = new (1f, 0f, 0f, 0.5f);
    static readonly float smoothSpeed = 15f;
    float targetFillAmount;
    float targetFillAmountMana;
    Vector3 targetAtk;

    static readonly float boosMultiply = 2;
    static readonly float stopRange = 1.7f;

    private void Awake()
    {
        currentHP = maxHP;
        currentGas = maxGas;
        nextUseHeal = Time.time + HealAreaCooldownTime;

        if (healthBarFill != null)
        {
            targetFillAmount = currentHP / maxHP;
            healthBarFill.fillAmount = targetFillAmount;

            targetFillAmountMana = currentGas / maxGas;
            GasBarFill.fillAmount = targetFillAmountMana;
        }

        agent.speed = moveSpeed;
    }

    public void MyUpdate() // llamado desde MasterGame - Array/Update
    {
        keyC.UpdateInputs();

        if (isDead)
        {
            healthBarFill.fillAmount = 0;
            CallAnimator(DEAD);
            return;
        }

        Move();
        Skills();
        UpdateBarrs();
        RotateTowardsTarget();
        HandleCooldownUI();
        StopAgentLogic();
        AutoAttak();
        Heal();
    }

    #region MOVIMIENTO Y ROTACION

    private void Move()
    {
        if (currentState == IDLE || currentState == WLK || currentState == BASIC)
        {
            SetAgentSpeed();

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    ProcessMovementInput();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                ProcessMovementInput();
            }
        }
    }

    private void SetAgentSpeed()
    {
        float movementSpeed = speedBoost ? (moveSpeed * boosMultiply) : moveSpeed;
        agent.speed = movementSpeed;
    }

    private void ProcessMovementInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (HandleTargetMovement(ray, enemy, enemyController.transform.position, true)) return;

        if (HandleTargetMovement(ray, torreta, torretaController.transform.position, false)) return;

        HandleGroundMovement(ray);
    }

    private bool HandleTargetMovement(Ray ray, LayerMask layer, Vector3 targetPosition, bool isEnemy)
    {
        if (Physics.Raycast(ray, Mathf.Infinity, layer))
        {
            if (isEnemy && enemyController.IsDown) return false;
            if (BscAtk && enemyt == isEnemy) return true;

            agent.stoppingDistance = attackRange * stopRange;
            agent.SetDestination(targetPosition);
            BscAtk = true;
            clicGround = false;
            enemyt = isEnemy;
            targetAtk = targetPosition;
            CallAnimator(WLK);
            return true;
        }
        return false;
    }

    private void HandleGroundMovement(Ray ray)
    {
        if (Physics.Raycast(ray, out var hit_ground, Mathf.Infinity, ground))
        {
            BscAtk = false;
            clicGround = true;
            agent.stoppingDistance = 0.0f;
            agent.SetDestination(hit_ground.point);
            CallAnimator(WLK);
        }
    }

    void RotateTowardsTarget()
    {
        if (clicGround) return;
        Vector3 directionToTarget = targetAtk - transform.position;
        directionToTarget.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
    }

    void StopAgentLogic()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                if (BscAtk)
                {
                    BasicAttakLogic();
                }
                else if (RangeSkill)
                {
                    OutRangeSkill();
                }
                else
                {
                    CallAnimator(IDLE);
                }
            }
        }
    }
    #endregion

    #region ATAQUE BASICO
    void AutoAttak()
    {
        float torretaDistance = (transform.position - torretaController.transform.position).sqrMagnitude;
        float enemyDistance = (transform.position - enemyController.transform.position).sqrMagnitude;

        if (torretaDistance < enemyDistance)
        {
            if (torretaDistance < 300)
            {
                if (currentState == IDLE)
                {
                    BscAtk = true;
                    clicGround = false;
                    enemyt = false;
                    targetAtk = torretaController.transform.position;
                }
            }
        }
        else if (enemyDistance < torretaDistance && !enemyController.IsDown)
        {
            if (enemyDistance < 300)
            {
                if (currentState == IDLE)
                {
                    BscAtk = true;
                    clicGround = false;
                    enemyt = true;
                    targetAtk = enemyController.transform.position;
                }
            }
            else
            {
                if (currentState == BASIC)
                {
                    BscAtk = false;
                    clicGround = true;
                    enemyt = false;
                    agent.stoppingDistance = 0.0f;
                    agent.SetDestination(transform.position);
                    CallAnimator(IDLE);
                }
            }
        }
    }


    void BasicAttakLogic()
    {
        CallAnimator(BASIC);
    }

    public void LaunchBasic()
    {
        float my_damange = damange;
        if (lvl > 1) my_damange = damange * (lvl * 1.3f);

        if (enemyt)
        {
            enemyController.GetDamange(my_damange);
        }
        else
        {
            torretaController.Damange(my_damange);
        }

        agent.SetDestination(transform.position);
        GetXP(5.0f);

        if (enemyt)
        {
            foreach (GameObject basic in BscEnemy)
            {
                if (!basic.activeSelf)
                {
                    basic.SetActive(true);
                    break;
                }
            }
        }
        else
        {
            foreach (GameObject basic in BscTorreta)
            {
                if (!basic.activeSelf)
                {
                    basic.SetActive(true);
                    break;
                }
            }
        }
    }
    #endregion

    #region HABILIDADES
    private void Skills()
    {
        HighlightSkills();

        SkillsLvlAsign();

        if (SkillUp)
        {
            Invoke(nameof(ResetSkillUp), 1f);
            return;
        }

        CallMinions();

        if (currentState == IDLE || currentState == WLK || currentState == BASIC)
        {
            if (enemyController.IsDown) return;

            if (Time.time >= nextGlobalUse)
            {
                CheckSkillInput();
            }

            CheckDefensiveSkills();
        }
    }

    void ResetSkillUp()
    {
        SkillUp = false;
    }

    private void HighlightSkills()
    {
        if (countSkills > 0)
        {
            HighlightSkillLevel(unQ, Qlvl, 5, newColor);
            HighlightSkillLevel(unW, Wlvl, 5, newColor);
            HighlightSkillLevel(unE, Elvl, 5, newColor);
            HighlightSkillLevel(unR, Rlvl, 3, newColor, lvl == 6 || lvl == 12 || lvl == 18);
        }
        else
        {
            DisableSkillHighlight(unQ, Qlvl, blakColor);
            DisableSkillHighlight(unW, Wlvl, blakColor);
            DisableSkillHighlight(unE, Elvl, blakColor);
            DisableSkillHighlight(unR, Rlvl, blakColor, lvl == 12 || lvl == 18);
        }
    }

    private void HighlightSkillLevel(Image skillImage, int skillLevel, int maxLevel, Color color, bool condition = true)
    {
        if (skillLevel < maxLevel && condition)
        {
            skillImage.gameObject.SetActive(true);
            skillImage.color = color;
        }
    }

    private void DisableSkillHighlight(Image skillImage, int skillLevel, Color color, bool condition = true)
    {
        if (skillLevel > 0 && condition)
        {
            skillImage.gameObject.SetActive(false);
        }
        else
        {
            skillImage.color = color;
        }
    }

    private void CheckSkillInput()
    {
        if (Time.time >= nextUseTimeQ && keyC.bQ && Qlvl > 0)
        {
            if (currentGas < QGas)
            {
                CallSound(CUP);
                return;
            }
            PrepareQSkill();
        }

        if (Time.time >= nextUseTimeW && keyC.bW && Wlvl > 0)
        {
            if (currentGas < WGas)
            {
                CallSound(CUP);
                return;
            }
            PrepareSkill(WLaunch, 1, enemyController.transform.position, Wrange);
        }

        if (Time.time >= nextUseTimeE && keyC.bE && Elvl > 0)
        {
            if (currentGas < EGas)
            {
                CallSound(CUP);
                return;
            }
            PrepareSkill(ELaunch, 2, enemyController.transform.position, Erange);
        }

        if (Time.time >= nextUseTimeR && keyC.bR && Rlvl > 0)
        {
            if (currentGas < RGas)
            {
                CallSound(CUP);
                return;
            }
            PrepareSkill(RLaunch, 3, enemyController.transform.position, Rrange);
        }
    }

    private void CheckDefensiveSkills()
    {
        if (keyC.bD && Time.time >= nextUseTimeD)
        {
            ActivateShield();
        }
        else if (keyC.bD)
        {
            CallSound(CUP);
        }

        if (keyC.bF && Time.time >= nextUseTimeF)
        {
            ActivateSpeedBoost();
        }
        else if (keyC.bF)
        {
            CallSound(CUP);
        }
    }

    private void PrepareQSkill()
    {
        if (Time.time >= nextGlobalUse)
        {
            if (Time.time >= nextUseTimeQ)
            {
                if (keyC.bQ && Qlvl > 0)
                {
                    if (currentGas < QGas)
                    {
                        CallSound(CUP);
                        return;
                    }

                    BscAtk = false;

                    if (RangeSkill) return;

                    targetAtk = enemyController.transform.position;
                    type = 0;
                    RangeSkill = true;
                    QLaunch();
                }
            }
        }
    }

    private void PrepareSkill(System.Action launchSkill, int skillType, Vector3 targetPosition, float range = 0f)
    {
        BscAtk = false;
        targetAtk = targetPosition;

        if (range > 0)
        {
            float distanceSquared = (transform.position - targetAtk).sqrMagnitude;

            if (distanceSquared >= range)
            {
                agent.SetDestination(targetAtk);
                agent.stoppingDistance = range;
                RangeSkill = true;
                type = skillType;
                CallAnimator(WLK);
            }
            else
            {
                launchSkill();
            }
        }
        else
        {
            launchSkill();
        }
    }

    private void CallMinions()
    {
        if (Time.time < nextUseTimeCallMinions)
        {
            unOne.gameObject.SetActive(true);
            return;
        }

        if (enemyController.IsDown)
        {
            unOne.gameObject.SetActive(false);

            if (keyC.bOne)
            {
                inibidor.CallToAction();
                CallSound(MAN);
                nextUseTimeCallMinions = Time.time + cooldownCallMinions;
            }
        }
        else
        {
            unOne.gameObject.SetActive(true);

            if (keyC.bOne)
            {
                CallSound(CUP);
            }
        }
    }

    private void ActivateShield()
    {
        shieldIsActive = true;
        ShieldObj.SetActive(true);
        CallSound(SHI);
        nextUseTimeD = Time.time + cooldownTimeD;
        Invoke(nameof(DEnd), 3);
    }

    private void ActivateSpeedBoost()
    {
        speedBoost = true;
        SpeedBstObj.SetActive(true);
        CallSound(BOS);
        nextUseTimeF = Time.time + cooldownTimeF;
        Invoke(nameof(FEnd), 3);
    }

    void SkillsLvlAsign()
    {
        if (countSkills > 0)
        {
            if (keyC.bQ)
            {
                if (Qlvl < 1) unQ.gameObject.SetActive(false);
                Qlvl++;
                countSkills--;
                lvlQ.text = Qlvl.ToString();
                SkillUp = true;
                CallSound(UP);
            }
            if (keyC.bW)
            {
                if (Wlvl < 1) unW.gameObject.SetActive(false);
                Wlvl++;
                countSkills--;
                lvlW.text = Wlvl.ToString();
                SkillUp = true;
                CallSound(UP);
            }
            if (keyC.bE)
            {
                if (Elvl < 1) unE.gameObject.SetActive(false);
                Elvl++;
                countSkills--;
                lvlE.text = Elvl.ToString();
                SkillUp = true;
                CallSound(UP);
            }
            if (keyC.bR)
            {
                if (lvl == 6 || lvl == 12 || lvl == 18)
                {
                    if (Rlvl < 1) unR.gameObject.SetActive(false);
                    Rlvl++;
                    countSkills--;
                    lvlR.text = Rlvl.ToString();
                    SkillUp = true;
                    CallSound(UP);
                }
            }
        }
        else
        {
            CallSound(NCSK);
        }
    }

    void QLaunch()
    {
        float lvlBaseDamange = lvl * QscaledByLevel;
        float damangeBase = lvlBaseDamange * damange;
        skillDamange = damangeBase * Qlvl;
        nextUseTimeQ = Time.time + cooldownTimeQ;
        agent.destination = transform.position;

        stun = false;
        pest = true;
        slow = false;

        clicGround = false;
        SKAnimation = skillsReference.ActiveThisSkill(0);
        UseGas(QGas);
        AllWait();
        CallAnimator(QAT);
        CallSound(QAT);
        Invoke(nameof(ResetRangeSkill), 1);
    }

    void WLaunch()
    {
        CallSound(WAT);
        float lvlBaseDamange = lvl * WscaledByLevel;
        float damangeBase = lvlBaseDamange * damange;
        skillDamange = damangeBase * Wlvl;
        nextUseTimeW = Time.time + cooldownTimeW;

        stun = true;
        pest = false;
        slow = false;

        clicGround = false;
        SKAnimation = skillsReference.ActiveThisSkill(1);
        UseGas(WGas);
        AllWait();
        CallAnimator(WAT);
        Invoke(nameof(ResetRangeSkill), 1);
    }

    void ELaunch()
    {
        float lvlBaseDamange = lvl * EscaledByLevel;
        float damangeBase = lvlBaseDamange * damange;
        skillDamange = damangeBase * Elvl;
        nextUseTimeE = Time.time + cooldownTimeE;

        stun = false;
        pest = true;
        slow = true;

        clicGround = false;
        SKAnimation = skillsReference.ActiveThisSkill(2);
        UseGas(EGas);
        AllWait();
        CallAnimator(EAT);
        Invoke(nameof(ResetRangeSkill), 1);
    }

    void RLaunch()
    {
        float lvlBaseDamange = lvl * RscaledByLevel;
        float damangeBase = lvlBaseDamange * damange;
        skillDamange = damangeBase * Rlvl;
        nextUseTimeR = Time.time + cooldownTimeR;

        stun = false;
        pest = false;
        slow = false;

        clicGround = false;
        SKAnimation = skillsReference.ActiveThisSkill(3);
        UseGas(RGas);
        AllWait();
        CallAnimator(RAT);
        Invoke(nameof(ResetRangeSkill), 1);
    }

    void OutRangeSkill()
    {
        switch (type)
        {
            case 1:
                WLaunch();
                break;
            case 2:
                ELaunch();
                break;
            case 3:
                RLaunch();
                break;
        }
    }

    public void ResetRangeSkill()
    {
        RangeSkill = false;
    }
    #endregion

    #region DAÑO Y DEFENSA
    public void GetDamange(float value)
    {
        if (!shieldIsActive)
        {
            currentHP -= value;

            if (value > currentHP)
            {
                currentHP = 0;
                isDead = true;
            }

            targetFillAmount = currentHP / maxHP;
        }
    }

    void UseGas(float value)
    {
        if (Time.time >= nextGlobalUse)
        {
            if (currentGas >= 0)
            {
                currentGas -= value;
                targetFillAmountMana = currentGas / maxGas;
            }
        }
    }

    void DEnd()
    {
        shieldIsActive = false;
        ShieldObj.SetActive(false);
    }

    void FEnd()
    {
        speedBoost = false;
        SpeedBstObj.SetActive(false);
    }

    public void Heal()
    {
        if (Time.time >= nextUseHeal)
        {
            float distanceSquared = (transform.position - HealArea.position).sqrMagnitude;

            if (distanceSquared < 100)
            {
                if (!healActive)
                {
                    Invoke(nameof(ResetHealt), healTime);
                }

                healActive = true;

                if (currentHP < maxHP)
                {
                    currentHP += healRate * Time.deltaTime;
                    targetFillAmount = currentHP / maxHP;
                }

                if (currentGas < maxGas)
                {
                    currentGas += healRate * Time.deltaTime;
                    targetFillAmountMana = currentGas / maxGas;
                }
            }
            else
            {
                healActive = false;
            }
        }
    }

    void ResetHealt()
    {
        healActive = false;
        nextUseHeal = Time.time + HealAreaCooldownTime;
    }
    #endregion

    #region UI Y UTILIDADES
    void UpdateBarrs()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetFillAmount, smoothSpeed * Time.deltaTime);
        }

        if (GasBarFill != null)
        {
            GasBarFill.fillAmount = Mathf.Lerp(GasBarFill.fillAmount, targetFillAmountMana, smoothSpeed * Time.deltaTime);
        }
    }

    void HandleCooldownUI()
    {
        float globalCooldownProgress = Mathf.Clamp01((nextGlobalUse - Time.time) / GlobalCooldownTime);

        if (cooldownQ != null)
        {
            float cooldownProgress = Mathf.Clamp01((nextUseTimeQ - Time.time) / cooldownTimeQ);
            cooldownQ.fillAmount = cooldownProgress + globalCooldownProgress;
        }

        if (cooldownQ != null)
        {
            float cooldownProgress = Mathf.Clamp01((nextUseTimeW - Time.time) / cooldownTimeW);
            cooldownW.fillAmount = cooldownProgress + globalCooldownProgress;
        }

        if (cooldownE != null)
        {
            float cooldownProgress = Mathf.Clamp01((nextUseTimeE - Time.time) / cooldownTimeE);
            cooldownE.fillAmount = cooldownProgress + globalCooldownProgress;
        }

        if (cooldownR != null)
        {
            float cooldownProgress = Mathf.Clamp01((nextUseTimeR - Time.time) / cooldownTimeR);
            cooldownR.fillAmount = cooldownProgress + globalCooldownProgress;
        }

        if (cooldownD != null)
        {
            float cooldownProgress = Mathf.Clamp01((nextUseTimeD - Time.time) / cooldownTimeD);
            cooldownD.fillAmount = cooldownProgress;
        }

        if (cooldownF != null)
        {
            float cooldownProgress = Mathf.Clamp01((nextUseTimeF - Time.time) / cooldownTimeF);
            cooldownF.fillAmount = cooldownProgress;
        }

        if (cooldowHeal != null)
        {
            float cooldownProgress = Mathf.Clamp01((nextUseHeal - Time.time) / HealAreaCooldownTime);
            cooldowHeal.fillAmount = 1 - cooldownProgress;
        }
    }

    public void SkillAnimation()
    {
        SKAnimation.gameObject.SetActive(true);
        SKAnimation.SkillPrepare(targetAtk, skillDamange, stun, pest, slow);
        SKAnimation.SkillLaunch();
        currentState = "";
        CallAnimator(IDLE);
    }

    public void GetXP(float get)
    {
        if (lvl >= topLvl) return;

        xp += get;

        if (xp >= topXP)
        {
            lvl++;
            topXP *= 1.2f;
            countSkills++;
            CallSound(LVLUP);
            timerText.text = lvl.ToString();
            xp = 0;
        }
    }
    #endregion

    #region SONIDOS Y ANIMACIONES
    void CallSound(string state)
    {
        sonido.Play(state);
    }

    void CallAnimator(string State)
    {
        if (currentState == State) return;
        animator.Play(State);
        currentState = State;
    }

    void AllWait()
    {
        nextGlobalUse = Time.time + GlobalCooldownTime;
    }
    #endregion
}
