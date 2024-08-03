using CharacterMovement;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCharacterAnimations : CharacterAnimations
{

    [Header("Tuning")]
    [SerializeField] private float _damping = 0.1f;

    protected CustomCharacterMovement _customCharacterMovement;
    protected TrapSpawner _trapSpawner;
    PhotonView photonView;

    [SerializeField] private Rigidbody _rigidbody;

    protected override void Awake()
    {
        base.Awake();
        _customCharacterMovement = GetComponentInParent<CustomCharacterMovement>();
        _trapSpawner = GetComponentInParent<TrapSpawner>();
        photonView = GetComponent<PhotonView>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected override void Update()
    {
        base.Update();
        // set crouched state
        _animator.SetBool("IsCrouching", _customCharacterMovement.IsCrouching);
        //Set Interacting state
        _animator.SetBool("IsInteracting", _customCharacterMovement.IsInteracting);
        //Set dashing state
        _animator.SetBool("IsDashing", _customCharacterMovement.IsDashing);
        //Set shoving state
        _animator.SetBool("IsShoving", _customCharacterMovement.IsShoving);
        //Set Trap Placement state
        _animator.SetBool("IsPlacingTrap", _trapSpawner.isPlacingTrap);

        Vector3 flattenedVelocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        // Fixed world => into local velocity
        Vector3 localVelocity = transform.InverseTransformVector(flattenedVelocity);
        _animator.SetFloat("Forward", localVelocity.z, _damping, Time.deltaTime);
        _animator.SetFloat("Right", localVelocity.x, _damping, Time.deltaTime);
    }
}
