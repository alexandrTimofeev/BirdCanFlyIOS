using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerFly : MonoBehaviour
{
    [SerializeField] private float forceSide = 5f;
    [SerializeField] private float forceUp = 5f;
    private new Rigidbody2D rigidbody;

    [Space]
    [SerializeField] private GameObject vfxFly;

    public Action<Vector2> OnMove;

    public void Init(IInput input)
    {
        input.OnBegan += ClickPoint;
        rigidbody = GetComponent<Rigidbody2D>();
        GamePause.OnPauseChange += SetPause;
    }

    private void ClickPoint(Vector2 point)
    {
        if (GamePause.IsPause)
            return;

        float directionSide = 0f;
        if (point.x < Screen.width / 2f)
            directionSide = -1f;
        else
            directionSide = 1f;

        if (vfxFly)        
            Destroy(Instantiate(vfxFly, transform.position, transform.rotation), 10f);        

        Vector2 moveForce = new Vector2(directionSide * forceSide, forceUp);
        rigidbody.AddForce(moveForce, ForceMode2D.Impulse);
        OnMove?.Invoke(moveForce);
    }

    private void SetPause(bool isPause)
    {
        rigidbody.simulated = !isPause;
    }

    private void OnDestroy()
    {
        GamePause.OnPauseChange -= SetPause;
    }
}
