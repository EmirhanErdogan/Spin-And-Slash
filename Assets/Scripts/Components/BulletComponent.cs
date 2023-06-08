using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using Emir;
using UnityEngine;

public class BulletComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private ParticleSystem Particle;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _transform;
    [SerializeField] private int BulletDestroyTime;
    [SerializeField] private float DamageAmount;
    [SerializeField] private float BulletSpeed;

    #endregion

    #region Private Fields

    private float Timer = 0;
    private bool IsActive = true;

    #endregion

    private void OnEnable()
    {
        BulletTimeInit();
    }

    private void Update()
    {
        BulletTimeControl();
    }


    #region Destroy Bullet

    public void BulletTimeInit()
    {
        Timer = Time.time + BulletDestroyTime;
    }

    private void BulletTimeControl()
    {
        if (GetIsActive() is false) return;
        if (Time.time > Timer)
        {
            //destroy Bullet
            SetIsActive(false);
            DestroyBullet();
        }
    }

    private void DestroyBullet(float Delay = 0.75f)
    {
        DOVirtual.DelayedCall(Delay, () => { Destroy(this.gameObject); });
    }

    #endregion


    #region Getters

    public Rigidbody GetRigidbody()
    {
        return _rigidbody;
    }

    public float GetBulletSpeed()
    {
        return BulletSpeed;
    }

    public ParticleSystem GetParticle()
    {
        return Particle;
    }

    public Transform GetTransform()
    {
        return _transform;
    }

    public bool GetIsActive()
    {
        return IsActive;
    }

    #endregion

    #region Setters

    public void SetIsActive(bool value)
    {
        IsActive = value;
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter(Collider other)
    {
        TriggerPlayer(other);
    }

    private void TriggerPlayer(Collider TargetCollider)
    {
        if (TargetCollider.gameObject.CompareTag(CommonTypes.PLAYER_TAG))
        {
            // if (IsActive is false) return;
            //playera hasar verdi
            GameManager.Instance.GetPlayerView().Damage(DamageAmount);
            //çarpma efekti çalıştı
            GameManager.Instance.GetPlayerView().DamageEffect();
            HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
            SetIsActive(false);
            Debug.Log("asddsdananısikeemm");
            //bullet yok edildi
            DestroyBullet(0.0001f);
        }
    }

    #endregion
}