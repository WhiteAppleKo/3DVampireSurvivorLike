using System.Collections.Generic;
using _02.Scripts.Augment.BaseAugment;
using _02.Scripts.Managers.Save;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Controller, ISaveable
{
    [Header("Mouse Movement")]
    public LayerMask groundLayerMask;
    public float stopDistance = 0.5f;

    private Vector3 m_Movement;
    private Vector2 m_InputVector;
    private InputSystem_Actions m_InputActions;


    protected override void Awake()
    {
        baseStats.playerStats = new PlayerStats();
        baseStats.playerStats.exp = new ClampInt(0, 100, 0);
        base.Awake();
        m_InputActions = new InputSystem_Actions();
        ((ISaveable)this).RegistSaveAble();
    }

    private void Start()
    {
        autoAttacker.GameStart();
        SubscribeManager.Instance.GameStart();
        LoadData();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        m_InputActions.Player.Enable();
        m_InputActions.UI.Enable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_InputActions.Player.Disable();
        m_InputActions.UI.Disable();
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        // 1. 키보드 입력을 먼저 확인한다.
        m_InputVector = m_InputActions.Player.Move.ReadValue<Vector2>();

        // 2. 키보드 입력이 있으면, 키보드 방향으로 움직인다.
        if (m_InputVector.sqrMagnitude > 0.01f)
        {
            m_Movement = new Vector3(m_InputVector.x, 0f, m_InputVector.y);
        }
        // 3. 키보드 입력이 없으면, 마우스 홀드 상태를 확인한다.
        else if (m_InputActions.UI.RightClick.IsPressed())
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayerMask))
            {
                Vector3 targetPosition = hit.point;
            
                // 커서가 캐릭터와 너무 가까우면 멈춘다 (진동 방지).
                if (Vector3.Distance(transform.position, targetPosition) < stopDistance)
                {
                     m_Movement = Vector3.zero;
                }
                else
                {
                    Vector3 directionToTarget = (targetPosition - transform.position).normalized;
                    m_Movement = new Vector3(directionToTarget.x, 0, directionToTarget.z);
                }
            }
            else
            {
                // 커서가 지면 위에 없으면 움직이지 않는다.
                m_Movement = Vector3.zero;
            }
        }
        // 4. 아무 입력이 없으면, 움직임을 멈춘다.
        else
        {
            m_Movement = Vector3.zero;
        }

        // 이동 및 회전 적용
        if (m_Movement.sqrMagnitude > 0.01f)
        {
            transform.Translate(m_Movement.normalized * (FinalStats.moveSpeed * Time.deltaTime), Space.World);
        
            //회전 로직 비활성화
            /*Quaternion targetRotation = Quaternion.LookRotation(m_Movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, FinalStats.turnSpeed * Time.deltaTime);*/
        }
    }
    
    #region 증강
    private List<StatAbility> m_Augments = new List<StatAbility>();
    /// <summary>
    /// 새로운 증강을 추가합니다.
    /// </summary>
    public void AddAugment(StatAbility augment)
    {
        Debug.LogWarning($"[Controller] ID: {this.GetInstanceID()} / AddAugment 호출됨. 증강: {augment?.abilityName}");
        m_Augments.Add(augment);
        Debug.Log(m_Augments.Count);
        RecalculateStats();
    }

    /// <summary>
    /// 증강을 제거합니다.
    /// </summary>
    public void RemoveAugment(StatAbility augment)
    {
        m_Augments.Remove(augment);
        RecalculateStats();
    }
    

    /// <summary>
    /// 기본 스탯부터 시작하여 모든 증강을 적용해 최종 스탯을 다시 계산합니다.
    /// </summary>
    protected void RecalculateStats()
    {
        // 1. 최종 스탯을 기본 스탯으로 초기화 (객체 재사용)
        if (FinalStats == null)
        {
            FinalStats = new BaseStats(baseStats);
        }
        else
        {
            FinalStats.ResetTo(baseStats);
        }
        
        // 2. 모든 증강의 스탯 수정치를 순서대로 적용
        foreach (var augment in m_Augments)
        {
            augment.Apply(FinalStats);
        }
    }

    public List<StatAbility> SaveAbilityData()
    {
        return m_Augments;
    }
    #endregion

    public void SaveData()
    {
        List<string> augmentsID = new List<string>();
        foreach (var augment in m_Augments)
        {
            augmentsID.Add(augment.abilityID);
        }
        
        PlayerSaveData saveData = new PlayerSaveData(
            FinalStats.playerStats.level,
            FinalStats.playerStats.exp.Current,
            FinalStats.hp.Current,
            augmentsID);
        
        SaveManager.Instance.SetPlayerData(saveData);
        autoAttacker.SaveData();
    }

    public void LoadData()
    {
        m_Augments.Clear();
        PlayerSaveData saveData = SaveManager.Instance.LoadPlayerSaveData();
        if (saveData == null)
        {
            return;
        }

        FinalStats.playerStats.level = saveData.playerLevel;
        FinalStats.playerStats.exp.IncreaseMaxValue(saveData.playerLevel * 10);
        FinalStats.playerStats.exp.Increase(saveData.currentExp);
        m_Augments = SaveManager.Instance.GetStatAbilities(saveData.statAugments);
        RecalculateStats();
        FinalStats.hp.LoadValue(saveData.currentHp);
        RecalculateStats();
    }
}