using CharacterMovement;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomWatchAnimations : MonoBehaviour
{
    protected Animator _animator;
    protected TrapSpawner _trapSpawner;
    PhotonView photonView;

    public void Awake()
    {
        _animator = GetComponent<Animator>();
        _trapSpawner = GetComponentInParent<TrapSpawner>();
        photonView = GetComponent<PhotonView>();
    }

    public void Update()
    {
        //Set Trap Placement state
        _animator.SetBool("IsPlacingTrap", _trapSpawner.isPlacingTrap);
    }
}
