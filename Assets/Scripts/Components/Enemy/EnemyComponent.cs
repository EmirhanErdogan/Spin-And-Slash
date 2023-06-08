using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Emir;
using FIMSpace.FProceduralAnimation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private Animator _animator;
    [SerializeField] private Slider _slider;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private RagdollAnimator _ragdoll;
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;
    [SerializeField] private NavMeshAgent Agent;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private GameObject SliderRoot;
    [SerializeField] private float RunRadius;
    [SerializeField] private float AttackRadius;
    [SerializeField] private float AttackDelay;
    [SerializeField] private float DamageAmount;
    [SerializeField] private float Health;
    [SerializeField] private float CurrentHealth;
    [SerializeField] private float WakeUpDelay = 1f;

    #endregion

    #region Private Fields

    private PlayerView player => GameManager.Instance.GetPlayerView();
    private bool IsRun = false;
    private bool IsJump = false;
    private bool IsAttack = false;
    private bool IsGround = true;
    private bool IsTrigger = false;
    private bool ISDeath = false;
    private Ray ray;
    private RaycastHit hit;

    #endregion

    private void Start()
    {
        SliderLookCamera();
        SliderInitialize();
    }

    private void FixedUpdate()
    {
        PlayerPosControl();
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (GameManager.Instance.GetGameState() != EGameState.STARTED) return;
        if (ISDeath is true) return;
        if (IsRun is true)
        {
            Agent.destination = player.transform.position;
            SliderLookCamera();
        }
    }

    private void PlayerPosControl()
    {
        if (GameManager.Instance.GetGameState() != EGameState.STARTED) return;
        if (ISDeath is true) return;
        if (IsJump is true) return;
        if (GameManager.Instance.GetPlayerView().GetIsDeath() is true)
        {
            Sleep();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= AttackRadius)
        {
            Attack();
        }
        else if (distance <= RunRadius)
        {
            Run();
        }
        else
        {
            Sleep();
        }
    }

    public void Damage(float Damage)
    {
        _slider.gameObject.SetActive(true);


        CurrentHealth -= Damage;
        SliderUpdated();
        HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            ISDeath = true;
            GetMesh().material.DOColor(Color.gray, 0.25f);

            LevelComponent.Instance.GetEnemys().Remove(this);
            SliderComponent.Instance.UpdateSlider();
            LevelComponent.Instance.EnemyCountControl();
            GetAnimator().SetTrigger("TriggerDead");
            GetAnimator().SetBool(CommonTypes.ENEMY_BASE_RUN_ANIM, false);
            GetAnimator().SetBool(CommonTypes.ENEMY_BASE_ATTACK_ANIM, false);
            DOVirtual.DelayedCall(3f, () => { Destroy(this.gameObject); });
            Destroy(_ragdoll.Parameters.RagdollDummyBase.gameObject);
        }

        DOVirtual.DelayedCall(1.5f, () => { _slider.gameObject.SetActive(false); });
    }

    public void ForceBack()
    {
        float RandomDistance = Random.Range(2f, 3.5f);
        float RandomJumpPower = Random.Range(1.5f, 2f);
        float RandomDuration = Random.Range(.75f, 1f);
        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform
            .DOJump(transform.position + (-transform.forward * RandomDistance), RandomJumpPower, 0, RandomDuration)
            .SetEase(Ease.OutSine));
        sequence.OnStart(() =>
        {
            _particleSystem.Play();
            Agent.enabled = false;
            GetAnimator().SetBool(CommonTypes.ENEMY_BASE_RUN_ANIM, false);
            GetAnimator().SetBool(CommonTypes.ENEMY_BASE_ATTACK_ANIM, false);
            GetAnimator().SetTrigger("TriggerDead");
            _ragdoll.User_FadeRagdolledBlend(1, 0.175f);
            IsRun = false;
            IsJump = true;
        });
        sequence.OnComplete(() =>
        {
            _ragdoll.User_FadeRagdolledBlend(0, 0.5f);
            GetRagdollAnimator().Parameters.FreeFallRagdoll = false;
            GroundControl();
            if (IsGround is true)
            {
                if (ISDeath is true) return;
                DOVirtual.DelayedCall(WakeUpDelay, () =>
                {
                    Run();
                    Agent.enabled = true;
                    IsJump = false;
                });
            }
            else
            {
                Agent.enabled = false;
                _rigidbody.useGravity = true;
                IsRun = false;
                IsJump = true;
                _ragdoll.User_FadeRagdolledBlend(1, 0.075f);
                GetAnimator().SetBool(CommonTypes.ENEMY_BASE_RUN_ANIM, false);
                GetAnimator().SetBool(CommonTypes.ENEMY_BASE_ATTACK_ANIM, false);
                GetAnimator().SetTrigger("TriggerDead");
                LevelComponent.Instance.GetEnemys().Remove(this);
                SliderComponent.Instance.UpdateSlider();
                LevelComponent.Instance.EnemyCountControl(1f);
                HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
                Destroy(_ragdoll.Parameters.RagdollDummyBase.gameObject);
                DOVirtual.DelayedCall(5f, () => { Destroy(this.gameObject); });
            }
        });

        /// TO DO : geriye doğru fırlatırken dojump ile geriye doğru atıcam on update esnasında shake yapıp sallıycam daha sonra da on complete de
        /// altında platform var mı yok mu diye kontrol edicem daha sonra varsa kapattıklarımı açıcam yoksa rigidbody ile düşürücem
    }

    public void GroundControl()
    {
        ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
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

    private void OnTriggerEnter(Collider other)
    {
        TriggerPlayer(other);
    }

    private void TriggerPlayer(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon") || other.gameObject.CompareTag("Chain"))
        {
            if (WeaponController.Instance.GetIsMove() is false)
                return;
            if (GameManager.Instance.GetPlayerView().GetWeaponIsActive() is false)
                return;
            if (ISDeath is true)
                return;
            if (GameManager.Instance.GetPlayerView().GetIsDeath() is true)
                return;
            if (IsTrigger is true)
                return;
            IsTrigger = true;
            DOVirtual.DelayedCall(0.25f, () => { IsTrigger = false; });
            Damage(10);
            ForceBack();
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

    #region Events

    private void Attack()
    {
        //animasyon çalışacak
        //hasar verilecek
        IsRun = false;

        if (IsAttack is true) return;
        IsAttack = true;
        GetAnimator().SetBool(CommonTypes.ENEMY_BASE_RUN_ANIM, false);
        GetAnimator().SetBool(CommonTypes.ENEMY_BASE_ATTACK_ANIM, true);
        DOVirtual.DelayedCall(AttackDelay / 2, () => { player.Damage(DamageAmount); });
        DOVirtual.DelayedCall(AttackDelay, () => { IsAttack = false; });
    }

    private void Run()
    {
        //animasyon çalışacak 
        //hareket bool change olacak
        if (IsRun is true) return;
        if (ISDeath is true) return;
        IsRun = true;
        IsAttack = false;
        GetAnimator().SetBool(CommonTypes.ENEMY_BASE_ATTACK_ANIM, false);
        GetAnimator().SetBool(CommonTypes.ENEMY_BASE_RUN_ANIM, true);
        Debug.Log("Run");
    }

    private void Sleep()
    {
        //animasyon değişecek
        //hareket boolu change olacak
        IsRun = false;
        IsAttack = false;
        GetAnimator().SetBool(CommonTypes.ENEMY_BASE_RUN_ANIM, false);
        GetAnimator().SetBool(CommonTypes.ENEMY_BASE_ATTACK_ANIM, false);
    }

    #endregion

    #region Getters

    public SkinnedMeshRenderer GetMesh()
    {
        return _meshRenderer;
    }

    public Animator GetAnimator()
    {
        return _animator;
    }

    public RagdollAnimator GetRagdollAnimator()
    {
        return _ragdoll;
    }

    #endregion
}