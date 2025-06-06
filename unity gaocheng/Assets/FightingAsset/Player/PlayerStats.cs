using UnityEngine;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    private Entity playerEntity;
    
    // 独立的属性存储（当没有Entity组件时使用）
    [Header("独立属性存储")]
    [SerializeField] private float independentMaxHP = 100f;
    [SerializeField] private float independentCurrentHP = 100f;
    [SerializeField] private float independentAttackPower = 10f;
    [SerializeField] private float independentMoveSpeed = 5f;

    private void Awake()
    {
        playerEntity = GetComponent<Entity>();
        if (playerEntity == null)
        {
            Debug.LogWarning($"PlayerStats on {gameObject.name}: 没有找到Entity组件！将使用独立属性存储。");
            // 初始化独立属性
            InitializeIndependentStats();
        }
    }

    private void InitializeIndependentStats()
    {
        independentMaxHP = 100f;
        independentCurrentHP = 100f;
        independentAttackPower = 10f;
        independentMoveSpeed = 5f;
    }

    // 封装访问器 - 支持独立存储
    public float MaxHP
    {
        get => playerEntity != null ? playerEntity.MaxHP : independentMaxHP;
        set 
        { 
            if (playerEntity != null) 
            {
                playerEntity.SetMaxHP(value);
            }
            else
            {
                independentMaxHP = value;
                Debug.Log($"独立设置MaxHP: {value}");
            }
        }
    }

    public float CurrentHP
    {
        get => playerEntity != null ? playerEntity.CurrentHP : independentCurrentHP;
        set 
        { 
            if (playerEntity != null) 
            {
                playerEntity.SetCurrentHP(value);
            }
            else
            {
                independentCurrentHP = value;
                Debug.Log($"独立设置CurrentHP: {value}");
            }
        }
    }

    public float AttackPower
    {
        get => playerEntity != null ? playerEntity.AttackPower : independentAttackPower;
        set 
        { 
            if (playerEntity != null) 
            {
                playerEntity.SetAttackPower(value);
            }
            else
            {
                independentAttackPower = value;
                Debug.Log($"独立设置AttackPower: {value}");
            }
        }
    }

    public float MoveSpeed
    {
        get => playerEntity != null ? playerEntity.MoveSpeed : independentMoveSpeed;
        set 
        { 
            if (playerEntity != null) 
            {
                playerEntity.SetMoveSpeed(value);
            }
            else
            {
                independentMoveSpeed = value;
                Debug.Log($"独立设置MoveSpeed: {value}");
            }
        }
    }

    [Header("玩家基础属性")]
    [SerializeField] private float dashForce = 15f;        // 冲刺力度
    [SerializeField] private float invincibleDuration = 1f; // 无敌时间
    [SerializeField] private float attackSpeed = 1f;       // 攻击速度
    [SerializeField] private float shotSpread = 0.1f;      // 射击扩散
    [SerializeField] private float shotSpeed = 10f;        // 子弹速度
    [Header("技能设置")]
    [SerializeField] private SkillData[] skills;           // 技能数组
    public float SkillInvincibleDuration = 2f;  // 无敌持续时间
    public float SkillInvincibleCooldown = 10f; // 技能冷却时间
    // 计算属性的公共接口
    public float DashForce => dashForce;
    public float InvincibleDuration => invincibleDuration;
    public float AttackSpeed
    {
        get => attackSpeed;
        set => attackSpeed = value;
    }// 攻击速度
    public float ShotSpread => shotSpread;    // 射击扩散
    public float ShotSpeed => shotSpeed;      // 子弹速度
    public float CurrentAttackCooldown => 1f / Mathf.Max(attackSpeed, 0.1f);//子弹冷却
    public SkillData[] Skills => skills;

    [System.Serializable]
    public class SkillData
    {
        public string skillName;     // 技能名称
        public KeyCode triggerKey;  // 技能触发按键
        public float duration;      // 技能持续时间
        public float cooldown;      // 技能冷却时间
        [HideInInspector] public float currentCooldown; // 当前技能冷却时间
    }

    // 初始化技能冷却时间
    public void InitializeSkills()
    {
        if (skills != null)
        {
            foreach (var skill in skills)
            {
                skill.currentCooldown = 0;
            }
        }
    }
    public void RestoreHealth()
    {
        // 直接将当前生命值设置为最大生命值
        CurrentHP = MaxHP;

        Debug.Log($"生命值已恢复至 {MaxHP}");
    }
}
