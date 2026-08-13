using UnityEngine;

// TData는 반드시 클래스 형태(SaveData 내부의 서브 클래스들)여야 하므로 상속 제약을 겁니다.
public abstract class TempManagerBase<TClass, TData> : MonoBehaviour, ISaveable
    where TClass : MonoBehaviour //자식 클래스 자신이어야 함
    where TData : class, new()
{
    // 부모가 자식 타입(TClass)의 싱글톤 생성
    public static TClass Instance { get; private set; }

    // 자식 매니저들이 실시간으로 다룰 임시 데이터 데이터 (인스펙터 확인을 위해 Protected)
    protected TData tempValues;

    protected virtual void Awake()
    {
        if (Instance == null) Instance = this as TClass;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if(MasterSaveManager.Instance == null)
        {
            return;
        }
        HandleLoadData(MasterSaveManager.Instance.currentSaveData);
    }

    protected virtual void OnEnable()
    {
        // 총괄 매니저의 로드 완료 이벤트 구독
        //MasterSaveManager.OnSaveDataLoaded += HandleLoadData;
    }

    protected virtual void OnDestroy()
    {
        // 메모리 누수 방지를 위한 구독 해제
        //MasterSaveManager.OnSaveDataLoaded -= HandleLoadData;
    }

    // [로드] 총괄 매니저가 이벤트를 쐈을 때 실행되는 공통 로직
    private void HandleLoadData(SaveData masterSaveData)
    {
        // 부모가 마스터 데이터 보따리에서 "내 영역의 데이터"만 쏙 골라옵니다.
        TData extractedData = GetMyDataFromMaster(masterSaveData);

        // 만약 세이브 파일에 내 데이터가 없다면 새 객체를 만들어 줍니다.
        tempValues = extractedData ?? new TData();

        // 데이터 채우기가 끝났으니, 자식 매니저에게 후속 처리(Instantiate 등)를 하라고 지시합니다.
        OnDataInitialized(tempValues);
    }

    // [세이브] 총괄 매니저가 인터페이스로 호출했을 때 실행되는 공통 로직
    public void BindSaveData(SaveData masterSaveData)
    {
        if (tempValues == null) tempValues = new TData();

        // 부모가 자식의 데이터를 마스터 보따리의 알맞은 위치에 꽂아 넣습니다.
        SetMyDataToMaster(masterSaveData, tempValues);
    }

    // --- 자식 매니저들이 "반드시" 구현해야 하는 추상 함수들 ---

    // 1. 마스터 데이터에서 데이터 로드
    protected abstract TData GetMyDataFromMaster(SaveData masterSaveData);

    // 2. 마스터 데이터에 데이터 저장
    protected abstract void SetMyDataToMaster(SaveData masterSaveData, TData currentTempData);

    // 3. 로드가 완료되어 tempValues가 채워진 직후, 인게임 오브젝트들에게 전파할 로직
    protected abstract void OnDataInitialized(TData initializedData);
}