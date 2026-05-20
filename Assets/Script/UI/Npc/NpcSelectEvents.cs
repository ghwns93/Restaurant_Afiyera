using System;
using System.Collections.Generic;
using UnityEngine;

public static class NpcSelectEvents
{
    // NPC가 감지되었을 때 UI에 알림 (NPC 객체 자체를 넘김)
    public static Action<BasicNpcScript> OnNPCDetected;
    // NPC가 범위를 벗어났을 때 UI에 알림
    public static Action<BasicNpcScript> OnNPCLost;
}