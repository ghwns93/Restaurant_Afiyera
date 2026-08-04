using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDicManager<TSelf, TKey, TValue> : MonoBehaviour
    where TSelf : BaseDicManager<TSelf, TKey, TValue>
{
    public static TSelf Instance { get; protected set; }

    [SerializeField] protected List<TValue> dataList;
    protected Dictionary<TKey, TValue> dataDic = new Dictionary<TKey, TValue>();

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as TSelf;
            DontDestroyOnLoad(gameObject);
            InitDictionary(); // Awake 시점에 자동 세팅
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 자식 클래스에서 ID를 어떻게 추출할지 정의하도록 추상화
    protected abstract TKey GetKey(TValue data);

    private void InitDictionary()
    {
        dataDic.Clear();
        foreach (TValue data in dataList)
        {
            if (data == null) continue;

            TKey key = GetKey(data);
            if (!dataDic.ContainsKey(key))
            {
                dataDic.Add(key, data);
            }
            else
            {
                Debug.LogWarning($"{typeof(TValue).Name} 중복 키 발견: {key}");
            }
        }
    }

    // 외부에서 데이터를 가져올 공통 메서드
    public TValue GetData(TKey key)
    {
        if (dataDic.TryGetValue(key, out TValue value))
            return value;

        return default;
    }

    public List<TValue> GetAllDataList()
    {
        // 원본 리스트 보호를 위해 새로운 리스트로 복사해서 전달
        return new List<TValue>(dataList);
    }
}

public interface IHasId
{
    int Id { get; }
}