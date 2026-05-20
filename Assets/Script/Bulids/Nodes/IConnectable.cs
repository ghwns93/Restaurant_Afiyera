public interface IConnectable
{
    int MinConnectionCount { get; } // 최소 연결 필요 개수
    void OnConnectionFailed();      // 연결 개수 부족 시 실행할 로직
    void OnConnectionSuccess(int totalCount); // 연결 성공 시 실행할 로직 (그룹 행동 등)
}