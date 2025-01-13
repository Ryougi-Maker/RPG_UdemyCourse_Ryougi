using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Major stats")]
    public Stat strength;
    public Stat agility;
    public Stat intelligence;
    public Stat vitality;

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic Stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;


    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;

    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;


    private float igniteDmageCooldown = .3f;
    private float igniteDmageTimer;
    private int igniteDamage;



    [SerializeField] private int currentHealth;
    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = maxHealth.GetValue();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        igniteDmageTimer -= Time.deltaTime;


        if (ignitedTimer < 0)
        {
            isIgnited = false;
        }
        if (chilledTimer < 0)
        {
            isChilled = false;
        }
        if (shockedTimer < 0)
        {
            isShocked = false;
        }
        if (igniteDmageTimer < 0 && isIgnited)
        {
            Debug.Log("take burn damage" + igniteDamage);
            currentHealth -= igniteDamage;
            if (currentHealth < 0)
                Die();
            igniteDmageTimer = igniteDmageCooldown;
        }
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {

        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        DoMagicalDamage(_targetStats);
    }
    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int _totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();

        _totalMagicalDamage = CheckTargetResistance(_targetStats, _totalMagicalDamage);
        _targetStats.TakeDamage(_totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
        {
            return;
        }
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Applied fire");
                return;
            }
            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Applied ice");
                return;
            }
            if (Random.value < .5f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Applied lighting");
                return;
            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);



    }

    private static int CheckTargetResistance(CharacterStats _targetStats, int _totalMagicalDamage)
    {
        _totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        _totalMagicalDamage = Mathf.Clamp(_totalMagicalDamage, 0, int.MaxValue);
        return _totalMagicalDamage;
    }

    public void ApplyAilments(bool _ignite,bool _chill,bool _shock)
    {
        if(isIgnited || isChilled || isShocked)
        {
            return;
        }
        if (_ignite)
        {
            isIgnited = _ignite;
            ignitedTimer = 2;
        }
        if (_chill)
        {
            isChilled = _chill;
            chilledTimer = 2;
        }
        if (_shock)
        {
            isShocked = _shock;
            shockedTimer = 2;
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public virtual void TakeDamage(int _damage)
    {
        currentHealth -= _damage;
        Debug.Log(_damage);
        if (currentHealth < 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        //throw new NotImplementedException();
    }
    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }
    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();
        if (isShocked)
        {
            totalEvasion += 20;
        }

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }
        return false;
    }

    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }
        return false;
    }
    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

}
