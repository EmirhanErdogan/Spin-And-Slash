using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Emir;
using FIMSpace.FProceduralAnimation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyFireComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private Transform BulletRoot;
    [SerializeField] private Slider _slider;
    [SerializeField] private SkinnedMeshRenderer Meshrendererer;
    [SerializeField] private ParticleSystem Particle;
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private ParticleSystem deadParticleSystemSystem;
    [SerializeField] private RagdollAnimator _ragdollAnimator;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private NavMeshAgent Agent;
    [SerializeField] private GameObject SliderRoot;
    [SerializeField] private float AttackRadius;
    [SerializeField] private float AttackDelay;
    [SerializeField] private float Health;
    [SerializeField] private float CurrentHealth;
    [SerializeField] private int FireCount;
    [SerializeField] private float JumpPower;
    [SerializeField] private bool IsBoss;

    #endregion

    #region Private Fields

    private PlayerView player => GameManager.Instance.GetPlayerView();
    private bool IsAttack = false;
    private bool IsGround;
    private Ray ray;
    private RaycastHit hit;
    private bool IsDamage = false;
    private bool ISDeath = false;
    private bool IsTrigger = false;
    private float AttackTimer = 0;
    private bool IsGroundFire = false;

    #endregion

    private void Start()
    {
        SliderInitialize();
        SliderLookCamera();
        AttackTimer = Time.time + AttackDelay;
    }

    private void FixedUpdate()
    {
        PlayerPosControl();
    }

    private void PlayerPosControl()
    {
        if (GameManager.Instance.GetGameState() != EGameState.STARTED) return;
        if (ISDeath is true) return;
        if (GameManager.Instance.GetPlayerView().GetIsDeath() is true)
        {
            Sleep();
            return;
        }

        if (Time.time > AttackTimer)
        {
            IsAttack = false;
        }

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= AttackRadius)
        {
            LookRotation();
            Attack();
            return;
        }
        else
        {
            Sleep();
            return;
        }
    }

    public void ForceBack()
    {
        float RandomDistance = Random.Range(2f, 3.5f);
        float RandomJumpPower = Random.Range(1.5f, 2f);
        float RandomDuration = Random.Range(.5f, 1f);
        //if (IsBoss is true) RandomDistance = JumpPower;
        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform
            .DOJump(transform.position + (-transform.forward * RandomDistance), RandomJumpPower, 0, RandomDuration)
            .SetEase(Ease.OutSine));
        sequence.OnStart(() =>
        {
            _particleSystem.Play();
            GetAnimator().SetBool(CommonTypes.ENEMY_BASE_RUN_ANIM, false);
            //GetAnimator().SetBool(CommonTypes.ENEMY_BASE_ATTACK_ANIM, false);
            GetAnimator().SetTrigger("TriggerDead");
            _ragdollAnimator.User_FadeRagdolledBlend(1, 0.175f);
            IsAttack = true;
            IsGroundFire = true;
        });
        // sequence.OnUpdate(() => { transform.DOShakeRotation(1f, 20f, 2); });
        sequence.OnComplete(() =>
        {
            GroundControl();
            if (IsGround is true)
            {
                GetAnimator().SetBool(CommonTypes.ENEMY_BASE_ATTACK_ANIM, false);
                DOVirtual.DelayedCall(0.25f, () =>
                {
                    IsAttack = true;
                    _ragdollAnimator.User_FadeRagdolledBlend(0, 0.5f);
                    GetAnimator().SetBool(CommonTypes.ENEMY_BASE_ATTACK_ANIM, false);
                    IsGroundFire = false;
                });
            }
            else
            {
                _ragdollAnimator.User_FadeRagdolledBlend(0, 0.5f);
                GetRagdollAnimator().Parameters.FreeFallRagdoll = false;
                _rigidbody.useGravity = true;
                Agent.enabled = false;
                GetAnimator().SetBool(CommonTypes.ENEMY_BASE_RUN_ANIM, false);
                GetAnimator().SetBool(CommonTypes.ENEMY_BASE_ATTACK_ANIM, false);
                GetAnimator().SetTrigger("TriggerDead");
                _ragdollAnimator.User_FadeRagdolledBlend(1, 0.075f);
                IsAttack = true;
                LevelComponent.Instance.GetFireEnemys().Remove(this);
                SliderComponent.Instance.UpdateSlider();
                LevelComponent.Instance.EnemyCountControl(1f);
                HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
                DOVirtual.DelayedCall(5f, () => { Destroy(this.gameObject); });
            }
        });
    }

    public void GroundControl()
    {
        ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                IsGround = true;
            }
            else
            {
                IsGround = false;
            }
        }
        else
        {
            IsGround = false;
        }
    }

    #region Events

    public void Damage(float Damage)
    {
        if (IsDamage is true) return;
        IsDamage = true;
        _slider.gameObject.SetActive(true);
        CurrentHealth -= Damage;
        HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            ISDeath = true;
            Meshrendererer.material.DOColor(Color.gray, 0.25f);
            LevelComponent.Instance.GetFireEnemys().Remove(this);
            SliderComponent.Instance.UpdateSlider();
            LevelComponent.Instance.EnemyCountControl();
            GetAnimator().SetTrigger("TriggerDead");
            DOVirtual.DelayedCall(3f, () => { Destroy(this.gameObject); });
        }

        SliderUpdated();
        DOVirtual.DelayedCall(1.5f, () => { _slider.gameObject.SetActive(false); });
        DOVirtual.DelayedCall(0.25f, () => { IsDamage = false; });
    }

    private void LookRotation()
    {
        Vector3 Direction = player.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(Direction);
        SliderLookCamera();
    }

    private void RootLookPlayer()
    {
        Vector3 Direction = player.transform.position - BulletRoot.position;
        BulletRoot.rotation = Quaternion.LookRotation(Direction);
    }

    private async void Fire()
    {
        //mermi oluşturma ve yok etme
        if (LevelComponent.Instance.GetBullets().Count <= 0)
        {
            LevelComponent.Instance.CreateBullet();
            await UniTask.Delay(TimeSpan.FromSeconds(0.11f));
        }

        for (int i = 0; i < FireCount; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            BulletComponent Bullet = LevelComponent.Instance.GetBullet();
            Bullet.transform.position = BulletRoot.transform.position;
            GetAnimator().SetTrigger(CommonTypes.ENEMY_TRIGGER_ATTACK_ANIM);
            GetParticle().Play();
            Bullet.GetParticle().Play();
            Vector3 Direction = (player.transform.position + Vector3.up) - Bullet.transform.position;
            Bullet.transform.rotation = Quaternion.LookRotation(Direction);
            RootLookPlayer();
            Bullet.transform.SetParent(null);
            Bullet.SetIsActive(true);
            Bullet.GetRigidbody().AddForce(Bullet.transform.forward * Bullet.GetBulletSpeed());
            Bullet.BulletTimeInit();
            SoundManager.Instance.Play(CommonTypes.SFX_FIRE);
        }

        //silah ateşleme
    }

    private void Attack()
    {
        if (IsAttack is true) return;
        if (ISDeath is true) return;
        if (IsGroundFire is true) return;
        IsAttack = true;
        AttackTimer = Time.time + AttackDelay;
        GetAnimator().SetBool(CommonTypes.ENEMY_BASE_ATTACK_ANIM, true);
        Fire();
    }

    private void Sleep()
    {
        //animasyon değişecek
        //hareket boolu change olacak
        //IsAttack = false;
        GetAnimator().SetBool(CommonTypes.ENEMY_BASE_ATTACK_ANIM, false);
    }

    #endregion

    #region Getters

    private ParticleSystem GetParticle()
    {
        return Particle;
    }

    public Animator GetAnimator()
    {
        return _animator;
    }

    public RagdollAnimator GetRagdollAnimator()
    {
        return _ragdollAnimator;
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        TriggerPlayer(other);
    }

    private void TriggerPlayer(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon") || other.gameObject.CompareTag("Chain"))
        {
            if (WeaponController.Instance.GetIsMove() is false) return;
            if (GameManager.Instance.GetPlayerView().GetWeaponIsActive() is false) return;
            if (ISDeath is true) return;
            if (GameManager.Instance.GetPlayerView().GetIsDeath() is true) return;
            if (IsTrigger is true) return;
            IsTrigger = true;
            DOVirtual.DelayedCall(0.25f, () => { IsTrigger = false; });
            ForceBack();
            Damage(10);
            EventService.Dispatch(new HitEnemyEvent());
        }
    }

    private void SliderLookCamera()
    {
        SliderRoot.transform.LookAt(CameraManager.Instance.GetVirtualCamera().transform);
    }

    private void SliderInitialize()
    {
        _slider.minValue = 0;
        _slider.maxValue = Health;
        _slider.value = Health;
    }

    private void SliderUpdated()
    {
        _slider.DOValue(CurrentHealth, 0.5f);
    }
}