using System;
using System.IO;
using UnityEngine;

public class FileLogger : MonoBehaviour
{
    public static FileLogger Instance { get; private set; }

    private string logFilePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLogger();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeLogger()
    {
        // 빌드 환경에서도 안전하게 접근 가능한 저장 경로 지정
        // Windows 기준: C:/Users/<사용자>/AppData/LocalLow/<회사명>/<프로젝트명>/Debug.txt
        logFilePath = Path.Combine(Application.persistentDataPath, "Debug.txt");

        try
        {
            // 프로그램 시작 시 기존 Debug.txt 내용을 덮어쓰고(초기화) 헤더 작성
            using (StreamWriter writer = new StreamWriter(logFilePath, append: false))
            {
                writer.WriteLine($"=== Debug Log Started: {DateTime.Now} ===");
                writer.WriteLine($"Platform: {Application.platform}");
                writer.WriteLine($"Path: {logFilePath}");
                writer.WriteLine("==========================================\n");
            }

            Debug.Log($"[FileLogger] Debug log initialized at: {logFilePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FileLogger] Failed to initialize log file: {ex.Message}");
        }
    }

    /// <summary>
    /// Debug.txt 파일에 메시지를 기록합니다.
    /// </summary>
    public static void Log(string message)
    {
        if (Instance == null || string.IsNullOrEmpty(Instance.logFilePath)) return;

        try
        {
            // append: true로 설정하여 기존 내용 뒤에 덧붙임
            using (StreamWriter writer = new StreamWriter(Instance.logFilePath, append: true))
            {
                string timeStamp = DateTime.Now.ToString("HH:mm:ss.fff");
                writer.WriteLine($"[{timeStamp}] {message}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FileLogger] Failed to write log: {ex.Message}");
        }
    }

    // 유니티 자체 Debug.Log / Debug.LogError 도 자동으로 파일에 수집하고 싶다면 활성화
    private void OnEnable()
    {
        Application.logMessageReceived += HandleUnityLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleUnityLog;
    }

    private void HandleUnityLog(string logString, string stackTrace, LogType type)
    {

        if (type == LogType.Error || type == LogType.Exception)
        {
            Log($"[{type}] {logString}\n{stackTrace}");
        }
    }
}
