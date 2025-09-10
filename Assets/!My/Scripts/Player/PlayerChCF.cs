using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerChCF : MonoBehaviour
{
    [SerializeField] private PlayerControllerFly controllerFly;
    [SerializeField] private DamageCollider damageCollider;
    [SerializeField] private DamageCollider damageColliderSuperHit;
    [SerializeField] private GrapCollider grapCollider;
    [SerializeField] private PlayerVisual playerVisual;

    [Space]
    [SerializeField] private float InvictibleDelay = 1f;

    [Space]
    [SerializeField] private AudioSource sourcePlayer;
    [SerializeField] private AudioClip clipFly;
    [SerializeField] private GameObject vfxHit;
    [SerializeField] private GameObject vfxGrap;
    [SerializeField] private GameObject vfxTeleport;

    private Vector3 pointStart;

    private IInput input;

    private Tween invictibleTween;
    private bool isInvictible;

    public Action OnDamage;
    public Action<GrapObject> OnGrap;
    public Action<Vector2> OnMove;

    //public DamageCollider PlayerDamageCollider => damageCollider;
    //public GrapCollider PlayerGrapCollider => grapCollider;

    public void Init (IInput input)
    {
        this.input = input;
        controllerFly.Init(input);
        controllerFly.OnMove += MoveOn;

        damageCollider.OnDamage += Damage;
        damageColliderSuperHit.OnDamage += SuperHit;
        grapCollider.OnGrap += GrapOn;

        pointStart = transform.position;
    }

    private void Damage(DamageContainer damageContainer)
    {
        Invictible();
        playerVisual.PlayPunchScale();
        Destroy(Instantiate(vfxHit, transform.position, transform.rotation), 10f);
        if (GameSettings.IsVibrationPlay)
            Handheld.Vibrate();
        OnDamage?.Invoke();
    }
    private void GrapOn(GrapObject grapObject)
    {
        Destroy(Instantiate(vfxGrap, grapObject.transform.position, grapObject.transform.rotation), 10f);
    }

    public void Invictible()
    {
        Invictible(InvictibleDelay);
    }

    public void Invictible(float delay)
    {
        if (invictibleTween != null)        
            invictibleTween.Kill(true);        

        damageCollider.gameObject.SetActive(false);
        playerVisual.StartFlicker();
        isInvictible = true;
        invictibleTween = DOVirtual.DelayedCall(delay, () =>
        {
            damageCollider.gameObject.SetActive(true);
            playerVisual.StopFlicker();
            isInvictible = false;
        });
    }

    private void MoveOn(Vector2 move)
    {
        playerVisual.transform.localScale = new Vector3(Mathf.Sign(move.x),
            playerVisual.transform.localScale.y,
            playerVisual.transform.localScale.z);
        sourcePlayer.pitch = UnityEngine.Random.Range(0.8f, 1.3f);
        sourcePlayer.PlayOneShot(clipFly);
        OnMove?.Invoke(move);
    }

    private void SuperHit(DamageContainer obj)
    {
        transform.position = pointStart;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        Instantiate(vfxTeleport, transform.position, transform.rotation);
    }
}
