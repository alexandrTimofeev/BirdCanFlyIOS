using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IInput
{
    public event Action<Vector2> OnBegan;
    public event Action<Vector2> OnEnded;
    public event Action<Vector2> OnMoved;

    public event Action<Vector2> OnBeganIgnorePause;
    public event Action<Vector2> OnEndedIgnorePause;
    public event Action<Vector2> OnMovedIgnorePause;
}