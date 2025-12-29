using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// MonoBehaviour를 상속받는 클래스를 위한 제네릭 싱글톤 베이스 클래스입니다.
/// 이 클래스를 상속받으면 해당 클래스는 씬 내에서 유일한 인스턴스를 가지게 됩니다.
/// </summary>
/// <typeparam name="T">싱글톤으로 만들 MonoBehaviour 타입</typeparam>
public class SingletoneBase<T> : MonoBehaviour where T : MonoBehaviour
{
    // 싱글톤 인스턴스를 저장하는 정적 변수
    private static T m_Instance;

    private static readonly object M_LOCK = new object();
    public static bool isApplicationQuitting = false;
    public static bool HasInstance => m_Instance != null;

    // 외부에서 싱글톤 인스턴스에 접근하기 위한 프로퍼티
    public static T Instance
    {
        get
        {
            if (isApplicationQuitting == true)
            {
                return null;
            }

            lock (M_LOCK)
            {
                // 인스턴스가 아직 할당되지 않았다면 씬에서 찾거나 새로 생성합니다.
                if (m_Instance == null)
                {
                    // 씬에서 해당 타입의 오브젝트를 찾습니다.
                    m_Instance = FindObjectOfType<T>();

                    // 씬에서도 찾지 못했다면 새로운 게임 오브젝트를 생성하고 컴포넌트를 추가합니다.
                    if (m_Instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<T>();
                        // 게임 오브젝트 이름을 해당 타입 이름으로 설정하여 찾기 쉽게 합니다.
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                    }
                }
                return m_Instance;
            }
        }
    }

    // 씬 전환 시 이 싱글톤 오브젝트를 파괴하지 않을지 설정하는 옵션
    public bool dontDestroyOnLoad = false;

    // Unity 라이프사이클 메서드: 오브젝트가 로드될 때 호출됩니다.
    protected virtual void Awake()
    {
        // 씬에 이미 이 타입의 다른 인스턴스가 존재하고, 그 인스턴스가 자신이 아니라면
        if (m_Instance != null && m_Instance != this)
        {
            // 중복된 인스턴스를 파괴합니다.
            Debug.LogWarning($"Singleton: An instance of {typeof(T).Name} already exists, destroying duplicate on GameObject '{gameObject.name}'.");
            Destroy(gameObject);
            return;
        }

        // 현재 인스턴스를 유일한 싱글톤 인스턴스로 설정합니다.
        m_Instance = this as T;

        // 씬 전환 시 파괴되지 않도록 설정합니다.
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }

    // Unity 라이프사이클 메서드: 오브젝트가 파괴될 때 호출됩니다.
    protected virtual void OnDestroy()
    {
        // 현재 인스턴스가 싱글톤 인스턴스였다면, 참조를 null로 설정하여 정리합니다.
        if (m_Instance == this)
        {
            m_Instance = null;
        }
    }
}
