using UnityEngine;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// Мобильный ввод через сенсорный экран.
/// </summary>
public class TouchInput : MonoBehaviour, IInput
{
    public event Action<Vector2> OnBegan;
    public event Action<Vector2> OnMoved;
    public event Action<Vector2> OnEnded;

    public event Action<Vector2> OnBeganIgnorePause;
    public event Action<Vector2> OnEndedIgnorePause;
    public event Action<Vector2> OnMovedIgnorePause;

    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            // Игнорировать, если палец над UI
            bool isOverUI = EventSystem.current.IsPointerOverGameObject(touch.fingerId);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if(!isOverUI)
                        OnBegan?.Invoke(touch.position);
                    OnBeganIgnorePause?.Invoke(touch.position);
                    break;

                case TouchPhase.Moved:
                    // touch.deltaPosition — это смещение с прошлого кадра
                    if (touch.deltaPosition != Vector2.zero)
                    {
                        if (!isOverUI)
                            OnMoved?.Invoke(touch.deltaPosition);
                        OnMovedIgnorePause?.Invoke(touch.deltaPosition);
                    }
                    break;

                case TouchPhase.Ended:
                    if (!isOverUI)
                        OnEnded?.Invoke(touch.position);
                    OnEndedIgnorePause?.Invoke(touch.position);
                    break;

                // Canceled зафиксируем тоже как окончание
                case TouchPhase.Canceled:
                    if (!isOverUI)
                        OnEnded?.Invoke(touch.position);
                    OnEndedIgnorePause?.Invoke(touch.position);
                    break;
            }
        }
    }
}
