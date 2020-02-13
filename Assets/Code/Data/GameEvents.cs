﻿using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StringEvent : UnityEvent<string> { }

// note events are triggered and handled programmatically (via listeners and invocations)
public static class GameEvents
{
    public static StringEvent onPaddleHit       = new StringEvent();
    public static StringEvent onVerticalWallHit = new StringEvent();
    public static UnityEvent  onScoreChanged    = new UnityEvent();
}