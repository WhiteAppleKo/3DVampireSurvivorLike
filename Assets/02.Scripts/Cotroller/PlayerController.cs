using System.Collections.Generic;
using _02.Scripts.Augment.BaseAugment;
using _02.Scripts.Cotroller;
using Features.Player;
using _02.Scripts.Managers.Save;
using UnityEngine;
using UnityEngine.InputSystem;

// [L] Logic System for Player
[RequireComponent(typeof(IPlayerVisualizer))]
public class PlayerController : Controller, ISaveable
{
    [Header("DLV Data")]
    [SerializeField] private PureDataPlayer pureData;
    
    [Header("Movement Settings")]
    public LayerMask groundLayerMask;
    public float stopDistance = 0.5f;

    private RuntimeDataPlayer model;
    private IPlayerVisualizer visuals;
    private InputSystem_Actions inputActions;
    private Camera mainCamera;

    protected override void Awake()
    {
        // 1. Visual & Data Binding
        visuals = GetComponent<IPlayerVisualizer>();
        mainCamera = Camera.main;
        inputActions = new InputSystem_Actions();

        if (pureData == null)
        {
            pureData = ScriptableObject.CreateInstance<PureDataPlayer>();
            Debug.LogWarning("[PlayerController] PureData가 설정되지 않았습니다.");
        }

        model = new RuntimeDataPlayer(pureData);

        // 2. Base 호환성 (기존 시스템을 위해 남겨둠)
        baseStats.usePlayerStats = true; // 플레이어 스탯 사용 활성화
        baseStats.playerStats = new PlayerStats();
        baseStats.playerStats.exp = new ClampInt(0, 100, 0); // 경험치 객체 생성
        
        base.Awake();
        
        ((ISaveable)this).RegistSaveAble();
    }

    // 모델에 접근할 수 있도록 프로퍼티 추가 (SubscribeManager 등에서 사용)
    public RuntimeDataPlayer Model => model;

    private void Start()
    {
        autoAttacker.GameStart();
        SubscribeManager.Instance.GameStart();
        LoadData();
        autoAttacker.LoadData();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        inputActions.Player.Enable();
        inputActions.UI.Enable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        inputActions.Player.Disable();
        inputActions.UI.Disable();
    }

    private void Update()
    {
        // 1. Input -> Pure Data (Rule 1)
        var cmd = GatherInput();

        // 2. Decision & Command
        ProcessMovement(cmd);
    }

    private PlayerInputCommand GatherInput()
    {
        Vector2 inputVec = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(inputVec.x, 0f, inputVec.y);
        
        bool isRightClick = inputActions.UI.RightClick.IsPressed();
        Vector3 mouseWorldPos = Vector3.zero;

        if (isRightClick)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayerMask))
            {
                mouseWorldPos = hit.point;
            }
        }

        return new PlayerInputCommand(moveDir, isRightClick, mouseWorldPos);
    }

    private void ProcessMovement(PlayerInputCommand cmd)
    {
        Vector3 finalMoveDir = Vector3.zero;

        // 로직 판단: 키보드 입력 우선
        if (cmd.MoveDirection.sqrMagnitude > 0.01f)
        {
            finalMoveDir = cmd.MoveDirection;
        }
        else if (cmd.IsRightClickPressed && cmd.MouseWorldPosition != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, cmd.MouseWorldPosition) > stopDistance)
            {
                finalMoveDir = (cmd.MouseWorldPosition - transform.position).normalized;
                finalMoveDir.y = 0;
            }
        }

        // 3. Render (Command Visualizer)
        visuals.Move(finalMoveDir, model.MoveSpeed, Time.deltaTime);
        visuals.SetMoveVisual(finalMoveDir.magnitude);
    }

    protected override void ApplyDamage(int amount)
    {
        base.ApplyDamage(amount);
        model.TakeDamage(amount);
    }

    protected override void Die(int prev, int current)
    {
        Debug.Log("[PlayerController] 플레이어 사망");
        // 사망 연출 호출 (Shapes 기반)
        visuals.PlayDamageVisual(); 
        isMoveDisable = true;
    }

    #region 증강 & 세이브 (기존 로직 유지하며 모델과 연동)
    private List<StatAbility> m_Augments = new List<StatAbility>();

    public void AddAugment(StatAbility augment)
    {
        m_Augments.Add(augment);
        RecalculateStats();
    }

    public void RemoveAugment(StatAbility augment)
    {
        m_Augments.Remove(augment);
        RecalculateStats();
    }

    protected void RecalculateStats()
    {
        // 기존 호환성 유지
        if (FinalStats == null) FinalStats = new BaseStats(baseStats);
        FinalStats.ResetTo(baseStats);
        
        // DLV Model 업데이트
        // 증강 데이터를 순회하며 수치를 합산하여 모델에 주입
        int hpAdd = 0;
        float speedMult = 1.0f;
        // foreach(var augment in m_Augments) { ... } 
        
        model.UpdateStats(hpAdd, speedMult);
    }

    public void SaveData()
    {
        List<string> augmentsID = new List<string>();
        foreach (var augment in m_Augments)
        {
            if (augment != null) augmentsID.Add(augment.abilityID);
        }
        
        PlayerSaveData saveData = new PlayerSaveData(
            model.CurrentLevel,
            model.CurrentExp,
            model.CurrentHp,
            augmentsID);
        
        SaveManager.Instance.SetPlayerData(saveData);
        if (autoAttacker != null) autoAttacker.SaveData();
    }

    public void LoadData()
    {
        PlayerSaveData saveData = SaveManager.Instance.LoadPlayerSaveData();
        if (saveData == null) return;

        // model 데이터 로드 및 초기화
        // model.Load(saveData); 
    }
    #endregion
}