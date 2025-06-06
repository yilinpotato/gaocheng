using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.EventSystems;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine.UI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player : Entity
{
    private PlayerStats playerStats;
    private PlayerShooting shooting;

    [Header("房间边界")]
    public GameObject room; // 拖入 Room 游戏对象

    private float minX, maxX, minY, maxY;

    //===================== 输入设置 =====================
    [Header("输入设置")]
    [SerializeField] private KeyCode attackKey = KeyCode.Space;   // 攻击键
    [Header("击退设置")]
    [SerializeField] private float knockbackForce = 1f;
    [SerializeField] private float knockbackDuration = 0.2f; // 击退持续时间
    private bool isKnockback;  // 是否处于击退状态
    private Coroutine invincibleCoroutine;
    //===================== 输入检测 =====================
    private Vector2 moveInput;

    private void CheckMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(horizontal, vertical).normalized;
        isMovingX = Mathf.Abs(horizontal) > 0;
        isMovingY = Mathf.Abs(vertical) > 0;

        Move(moveInput);
    }

    //===================== 组件引用 =====================
    private Rigidbody2D rb;
    private PlayerInput playerInput;

    //===================== 状态变量 =====================
    private bool isMovingX = false;  // X轴移动检测
    private bool isMovingY = false;  // Y轴移动检测
    private bool isDashing = false;
    private bool isInvincible = false;
    private float currentDashCooldown = 0;
    private Vector2 moveDirection;

    //===================== 动画设置 =====================
    [Header("动画控制")]
    [SerializeField] private Animator bodyAnimator;
    [SerializeField] private SpriteRenderer bodySpriteRenderer;
    [SerializeField] private Animator headAnimator;
    [SerializeField] private SpriteRenderer headSpriteRenderer;

    //===================== 攻击系统 =====================
    private float nextAttackTime = 0f;

    //===================== 初始化 =====================
    protected override void Start()
    {

        base.Start();
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        playerStats = GetComponent<PlayerStats>();
        playerStats.InitializeSkills();
        shooting = GetComponent<PlayerShooting>();
        InitializeRoomBounds();
        // 初始化技能冷却
        foreach (var skill in playerStats.Skills)
        {
            skill.currentCooldown = 0;
        }
    }

    //===================== 输入处理 =====================
    private void CheckAttackInput()
    {
        if (Input.GetKeyDown(attackKey) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + shooting.GetAttackCooldown();
            headAnimator.SetTrigger("AttackTrigger"); //只触发一次攻击动画
            shooting.Shoot();
        }
    }

    private void CheckSkillInput()
    {
        foreach (var skill in playerStats.Skills)
        {
            if (Input.GetKeyDown(skill.triggerKey) && skill.currentCooldown <= 0)
            {
                switch (skill.skillName)
                {
                    case "Dash":
                        StartCoroutine(PerformDash(skill));
                        break;
                    case "Invincibility":  
                        StartCoroutine(ActivateInvincibility(skill));
                        break;
                }
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        CheckMovementInput();
        UpdateCooldowns();
        UpdateAnimations();
        CheckAttackInput();
        CheckSkillInput();
        ClampPosition();
        Debug.Log($"血量: {currentHP}/{maxHP}");
    }

    //===================== 单例继承 =====================
    public static Player Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        // 单例初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }



    //===================== 移动系统 =====================
    public override void Move(Vector2 direction)
    {
        if (isDead || isDashing || isKnockback) return; // 添加 isKnockback 检查

        // 使用插值实现平滑移动
        Vector2 targetVelocity = direction * moveSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, Time.deltaTime * 10f);
    }
    private void InitializeRoomBounds()
    {
        if (room == null)
        {
            Debug.LogError("房间对象未设置！");
            return;
        }

        // 使用渲染器边界而非碰撞器边界（更直观）
        Renderer roomRenderer = room.GetComponent<Renderer>();
        if (roomRenderer != null)
        {
            Bounds bounds = roomRenderer.bounds;

            // 获取玩家的渲染器边界
            Renderer playerRenderer = GetComponent<Renderer>();
            Bounds playerBounds = playerRenderer != null ? playerRenderer.bounds : new Bounds(transform.position, Vector3.one);

            // 计算边界时考虑玩家尺寸
            minX = bounds.min.x + playerBounds.extents.x;
            maxX = bounds.max.x - playerBounds.extents.x;
            minY = bounds.min.y + playerBounds.extents.y;
            maxY = bounds.max.y - playerBounds.extents.y;

            Debug.Log($"房间边界: X({minX},{maxX}), Y({minY},{maxY})");
        }
        else
        {
            Debug.LogError("Room对象未找到Renderer组件！");
        }
    }

    private void ClampPosition()
    {
        if (minX >= maxX || minY >= maxY)
        {
            // 边界无效时跳过限制
            return;
        }

        Vector2 currentPos = transform.position;
        Vector2 clampedPos = new Vector2(
            Mathf.Clamp(currentPos.x, minX, maxX),
            Mathf.Clamp(currentPos.y, minY, maxY)
        );

        // 只在位置越界时应用限制（避免每帧都强制更新位置）
        if (currentPos != clampedPos)
        {
            transform.position = clampedPos;
        }
    }
    private IEnumerator PerformDash(PlayerStats.SkillData skill)
    {
        isDashing = true;
        skill.currentCooldown = skill.cooldown;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(moveInput.normalized * playerStats.DashForce, ForceMode2D.Impulse);

        SetInvincible(true);
        yield return new WaitForSeconds(skill.duration);  // 使用技能设定的持续时间
        SetInvincible(false);

        yield return new WaitForSeconds(0.2f);  // 停顿后恢复控制
        isDashing = false;
    }
    public void ReverseAnimation()
    {
        AnimatorStateInfo stateInfo = bodyAnimator.GetCurrentAnimatorStateInfo(0);
        bodyAnimator.Play(stateInfo.fullPathHash, 0, 1 - stateInfo.normalizedTime);
        bodyAnimator.SetFloat("SpeedMultiplier", -1); // 反转播放速度
    }
    public override void TakeDamage(float damage)
    {
        if (!isInvincible && !isDead)
        {
            base.TakeDamage(damage);
            StartCoroutine(InvincibleFlash());
            // 更新UI
        }
    }
    public void TakeDamageWithKnockback(float damage, Vector2 knockbackDirection)
    {
        if (!isInvincible && !isDead)
        {
            Debug.Log("击退触发！");
            base.TakeDamage(damage); // 调用基类基础扣血逻辑
            ApplyKnockback(knockbackDirection); // 执行击退
            StartCoroutine(InvincibleFlash());// 更新UI
            if (invincibleCoroutine != null)
            {
                StopCoroutine(invincibleCoroutine);
                invincibleCoroutine = StartCoroutine(InvincibleFlash());
            }
            
        }
    }

    //===================== 技能系统 =====================
    private IEnumerator ActivateInvincibility(PlayerStats.SkillData skill)
    {
        skill.currentCooldown = skill.cooldown;

        // 执行无敌逻辑
        SetInvincible(true);
        StartCoroutine(SkillInvincibleFlash());

        // 持续时间内保持无敌
        yield return new WaitForSeconds(skill.duration);

        // 结束无敌（InvincibleFlash会自动关闭无敌状态）
    }
    private IEnumerator SkillInvincibleFlash()
    {
        
        Debug.Log("无敌开始");
        SetInvincible(true);
        float timer = 0;

        while (timer < playerStats.InvincibleDuration)
        {
            // 同时设置 body 和 head 的透明度
            bodySpriteRenderer.color = new Color(1, 0.92f, 0.016f, 1);
            headSpriteRenderer.color = new Color(1, 0.92f, 0.016f, 1);

            yield return new WaitForSeconds(0.1f);

            bodySpriteRenderer.color = Color.white;
            headSpriteRenderer.color = Color.white;

            yield return new WaitForSeconds(0.1f);

            timer += 0.2f;
        }

        SetInvincible(false);
        Debug.Log("无敌结束");
    }
    //===================== 辅助方法 =====================
    public void ApplyKnockback(Vector2 direction)
    {
        if (!isKnockback)
        {

            if (!isInvincible && !isDead)
            {
                Debug.Log("执行击退方向：" + direction);
                StartCoroutine(KnockbackRoutine(direction));
            }

        }
    }

    private IEnumerator KnockbackRoutine(Vector2 direction)
    {
        Debug.Log("KnockbackRoutine 施加了速度：" + direction.normalized * knockbackForce);

        isKnockback = true;

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = direction.normalized * knockbackForce;

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }

    private void UpdateCooldowns()
    {
        for (int i = 0; i < playerStats.Skills.Length; i++)
        {
            if (playerStats.Skills[i].currentCooldown > 0)
            {
                playerStats.Skills[i].currentCooldown -= Time.deltaTime;
            }
        }

        if (currentDashCooldown > 0)
        {
            currentDashCooldown -= Time.deltaTime;
        }
    }

    private void UpdateAnimations()
    {
        if (bodyAnimator == null) return;

        bodyAnimator.SetFloat("MoveX", moveInput.x);
        bodyAnimator.SetFloat("MoveY", moveInput.y);
        bodyAnimator.SetBool("isMoving", moveInput.magnitude > 0);
        if (moveInput.x != 0)
        {
            Vector3 scale = bodySpriteRenderer.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (moveInput.x > 0 ? 1 : -1);
            bodySpriteRenderer.transform.localScale = scale;
        }
    }

    private IEnumerator InvincibleFlash()
    {
        Debug.Log("无敌开始");
        SetInvincible(true);
        float timer = 0;

        while (timer < playerStats.InvincibleDuration)
        {

            // 同时设置 body 和 head 的透明度
            bodySpriteRenderer.color = new Color(1, 1, 1, 0.5f);
            headSpriteRenderer.color = new Color(1, 1, 1, 0.5f);

            yield return new WaitForSeconds(0.1f);

            bodySpriteRenderer.color = Color.white;
            headSpriteRenderer.color = Color.white;

            yield return new WaitForSeconds(0.1f);

            timer += 0.2f;
        }

        SetInvincible(false);
        Debug.Log("无敌结束");
    }

    private void SetInvincible(bool state)
    {
        isInvincible = state;
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy"),
            state
        );
    }

    //===================== 死亡处理 =====================
    protected override void Die()
    {
        // 先执行基础的死亡逻辑，但不调用Entity的Die()来避免直接场景切换
        isDead = true;
        StopAllCoroutines();

        // 触发死亡事件
        OnDeath?.Invoke();

        // 通知事件系统
        EventBus.Publish(new DeathEvent(this));

        // Boss特有逻辑：显示结算面板而不是直接返回地图
        ShowVictoryPanel();

        // 禁用游戏对象
        gameObject.SetActive(false);
    }

    //===================== 调试辅助 =====================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
    }


    private void ShowVictoryPanel()
    {
        // 直接加载"结束面板"场景
        try
        {
            Debug.Log("玩家死亡 前往结算 scene...");
            UnityEngine.SceneManagement.SceneManager.LoadScene("结算面板");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load victory scene '结算面板': {e.Message}");
            // 如果加载结束面板失败，回退到直接返回地图
            ReturnToMap();
        }
    }

    private void ReturnToMap()
    {
        forTestButton buttonController = FindObjectOfType<forTestButton>();
        if (buttonController != null)
        {
            buttonController.ReturnToMapScene();
        }
        else
        {
            Debug.LogWarning("ForTestButton not found, cannot return to map");
        }
    }
}
