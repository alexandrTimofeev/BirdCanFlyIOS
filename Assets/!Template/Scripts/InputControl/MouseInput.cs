using UnityEngine;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// ПК-ввод через мышь.
/// </summary>
public class MouseInput : MonoBehaviour, IInput
{
    public event Action<Vector2> OnBegan;
    public event Action<Vector2> OnMoved;
    public event Action<Vector2> OnEnded;
    public event Action<Vector2> OnBeganIgnorePause;
    public event Action<Vector2> OnEndedIgnorePause;
    public event Action<Vector2> OnMovedIgnorePause;

    private Vector2 _lastPosition;
    private bool _isDragging;

    void Update()
    {
        bool isOverUI = EventSystem.current.IsPointerOverGameObject();

        // Нажатие левой кнопки
        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
            _lastPosition = Input.mousePosition;
            if(!isOverUI)
                OnBegan?.Invoke(_lastPosition);
            OnBeganIgnorePause?.Invoke(_lastPosition);
        }

        // Движение мыши при удержании
        if (_isDragging && Input.GetMouseButton(0))
        {
            Vector2 current = Input.mousePosition;
            Vector2 delta = current - _lastPosition;
            if (delta != Vector2.zero)
            {
                if (!isOverUI)
                    OnMoved?.Invoke(delta);
                OnMovedIgnorePause?.Invoke(delta);
                _lastPosition = current;
            }
        }

        // Отпускание кнопки
        if (_isDragging && Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            Vector2 endPos = Input.mousePosition;
            if (!isOverUI)
                OnEnded?.Invoke(endPos);
            OnEndedIgnorePause?.Invoke(endPos);
        }
    }
}
