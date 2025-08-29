using System;
using UnityEngine;

public class TutorialWindow : MonoBehaviour
{
    private IInput input;
    public event Action OnClick;

    public void Init (IInput input)
    {
        this.input = input;

        input.OnBeganIgnorePause += Tap;
        InterfaceManager.MainBlackScreen.Show();
    }

    private void Tap(Vector2 obj)
    {
        Close();
    }

    private void Close()
    {
        input.OnBeganIgnorePause -= Tap;
        InterfaceManager.MainBlackScreen.Hide();
        gameObject.SetActive(false);

        OnClick?.Invoke();
    }
}
