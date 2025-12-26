using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Controller
{
    [Header("Mouse Movement")]
    public LayerMask groundLayerMask;
    public float stopDistance = 0.5f;

    private Vector3 m_Movement;
    private Vector2 m_InputVector;
    private InputSystem_Actions m_InputActions;

    private void Awake()
    {
        base.Awake();
        m_InputActions = new InputSystem_Actions();
    }

    private void Start()
    {
        autoAttacker.GameStart();
        UIManager.Instance.GameStart();
    }

    protected virtual void OnEnable()
    {
        base.OnEnable();
        m_InputActions.Player.Enable();
        m_InputActions.UI.Enable();
    }

    protected virtual void OnDisable()
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
            transform.Translate(m_Movement.normalized * (moveSpeed * Time.deltaTime), Space.World);
        
            Quaternion targetRotation = Quaternion.LookRotation(m_Movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }
}