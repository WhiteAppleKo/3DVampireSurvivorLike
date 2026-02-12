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

        base.Awake();
        
        ((ISaveable)this).RegistSaveAble();
    }

    // 모델에 접근할 수 있도록 프로퍼티 추가 (SubscribeManager 등에서 사용)
    public RuntimeDataPlayer Model => model;

    private void Start()
    {
        // 1. 바인딩 먼저 (DLV UI Binding)
        SubscribeManager.Instance.GameStart();

        // 2. 그 다음 로직 시작
        autoAttacker.GameStart();
        
        LoadData();
        autoAttacker.LoadData();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        inputActions.Player.Enable();
        inputActions.UI.Enable();
        
        // DLV Event Subscription
        if (model != null)
        {
            model.OnHpChanged += CheckDeath;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        inputActions.Player.Disable();
        inputActions.UI.Disable();
        
        if (model != null)
        {
            model.OnHpChanged -= CheckDeath;
        }
    }

    private void CheckDeath(int currentHp, int maxHp)
    {
        if (currentHp <= 0)
        {
            Die(0, 0); // 매개변수는 무의미하므로 0 전달
        }
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

    public override void ApplyDamage(int amount)
    {
        model.TakeDamage(amount);
    }

    protected override void Die(int prev, int current)
    {
        Debug.Log("[PlayerController] 플레이어 사망");
        visuals.PlayDamageVisual(); 
        isMoveDisable = true;
    }

    public override float CurrentMoveSpeed => model != null ? model.MoveSpeed : 0f;

    #region 세이브 및 로드 (DLV 모델 연동)
    public void SaveData()
    {
        // TODO: PureData 기반 증강 리스트 저장 로직 추가 필요
        PlayerSaveData saveData = new PlayerSaveData(
            model.CurrentLevel,
            model.CurrentExp,
            model.CurrentHp,
            new List<string>()); // 레거시 ID 리스트 비움
        
        DataHub.Instance.SetPlayerData(saveData);
        if (autoAttacker != null) autoAttacker.SaveData();
    }

    public void LoadData()
    {
        PlayerSaveData saveData = DataHub.Instance.LoadPlayerSaveData();
        if (saveData == null) return;

        // 런타임 모델 데이터 복구 (필요시 구현)
    }
    #endregion
}