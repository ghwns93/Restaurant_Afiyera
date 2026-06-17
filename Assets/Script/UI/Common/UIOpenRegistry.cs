using UnityEngine;

public static class UIOpenRegistry 
{
    private static int activeBlockingUI = 0;

    public static void RegisterUI() => activeBlockingUI++;
    public static void UnregisterUI() => activeBlockingUI = Mathf.Max(0, activeBlockingUI - 1);

    public static bool CanOpenOption => activeBlockingUI == 0;
}
