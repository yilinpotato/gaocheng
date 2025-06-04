using UnityEngine;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    private Entity playerEntity;

    private void Awake()
    {
        playerEntity = GetComponent<Entity>();
    }

    // 封装访问器
    public float MaxHP
    {
        get => playerEntity.MaxHP;
        set => playerEntity.SetMaxHP(value);
    }

    public float CurrentHP
    {
        get => playerEntity.CurrentHP;
        set => playerEntity.SetCurrentHP(value);
    }

    public float AttackPower
    {
        get => playerEntity.AttackPower;
        set => playerEntity.SetAttackPower(value);
    }

    public float MoveSpeed
    {
        get => playerEntity.MoveSpeed;
        set => playerEntity.SetMoveSpeed(value);
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
    public float AttackSpeed => attackSpeed;  // 攻击速度
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
        foreach (var skill in skills)
        {
            skill.currentCooldown = 0;
        }
    }
    public void RestoreHealth()
    {
        // 直接将当前生命值设置为最大生命值
        CurrentHP = MaxHP;

        Debug.Log($"生命值已恢复至 {MaxHP}");
    }
}
