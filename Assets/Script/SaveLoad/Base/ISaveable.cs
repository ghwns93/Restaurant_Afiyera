public interface ISaveable
{
    // 총괄 매니저가 "데이터 내놔!" 하고 호출할 함수
    // 마스터 데이터(masterSaveData)를 받아서, 각 임시 매니저가 들고 있던 데이터를 거기에 직접 채워 넣습니다.
    void BindSaveData(SaveData masterSaveData);
}