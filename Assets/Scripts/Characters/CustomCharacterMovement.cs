using System;
using System.Collections;
using Photon.Pun;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
using UnityEngine.VFX;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CharacterMovement
{
    public class CustomCharacterMovement : CharacterMovement3D, IPunObservable
    {
        public PhotonView photonView;
        private InventoryManager IMInstance;
        private Animator animator;
        private PlayerInput movementAction;
        private CapsuleCollider PlayerCC;
        private PlayerStatsManager playerStatsManager;
        private TrapSpawner trapSpawner;
        private OwnerNetworkComponent ownerNetworkComponent;
        private bool attemptingToDropKeycard = false;
        private Color playerColor;
        private InteractableObject currentInteractable;
        private WorldButtonInteractable endGameButton;
        [SerializeField] private GameObject spotlight; 
        private MaterialColorChanger mCC; 
        [Header("Custom Character Parameters")]
        [SerializeField] private Transform _RFP;
        [SerializeField] private Transform _followLocation;
        [SerializeField] private Transform _trapSettingFollowPoint;

        [BoxGroup("Debug"), SerializeField] private Rigidbody _shoveTarget;

        public LayOffGameEvent onDeathEvent;

        [BoxGroup("Shove Stats"), SerializeField, ShowInInspector] private GameObject _shoveBox;
        [BoxGroup("Shove Stats"), SerializeField, ShowInInspector] private float _shoveForce = 3f;
        [BoxGroup("Shove Stats"), SerializeField, ShowInInspector] private float _shoveBoxWidth = 0.65f;
        [BoxGroup("Shove Stats"), SerializeField, ShowInInspector] private float _shoveBoxHeight = 1.8f;
        [BoxGroup("Shove Stats"), SerializeField, ShowInInspector] private float _shoveBoxLength = 0.5f;
        [BoxGroup("Shove Stats"), SerializeField, ShowInInspector] private float _shoveDelay = 5f;

        [BoxGroup("PlayerTimers"), SerializeField] private float _respawnTimer = 5f;
        [BoxGroup("PlayerTimers"), SerializeField] private float _interactTimer = 1f;

        [BoxGroup("Dash Settings"), ShowInInspector] public bool CanDash = true;
        [BoxGroup("Dash Settings"), ShowInInspector] public int dashCoolDown { get; protected set; } = 5;
        [BoxGroup("Dash Settings"), SerializeField] private float _launchSpeed = 20f;
        [BoxGroup("Dash Settings"), SerializeField] private float dashCameraEffectDuration = 0.4f;

        public bool IsCrouching { get; protected set; }
        public bool CanInteract { get; protected set; }
        public bool CanShove = true;
        public bool IsInteracting { get; protected set; }
        public bool IsShoving { get; protected set; }
        public bool IsDashing = false;
        public bool IsDead { get; protected set; }
        public bool IsJumping { get; protected set; } = false;

        private bool speedReduced = false; 

        [BoxGroup("SFX"), SerializeField] protected EventReference JumpSFX;
        [BoxGroup("SFX"), SerializeField] protected EventReference DashSFX;
        [BoxGroup("SFX"), SerializeField] protected EventReference CrouchSFX;
        [BoxGroup("SFX"), SerializeField] protected EventReference ShoveSFX;
        [BoxGroup("SFX"), SerializeField] protected EventReference ShovesuccessSFX;
        [BoxGroup("SFX"), SerializeField] protected EventReference DeathSFX;

        [BoxGroup("CharacterVA"), SerializeField] protected EventReference _characterDeathShortVA;
        [BoxGroup("CharacterVA"), SerializeField] protected EventReference _characterDeathLongVA;
        [BoxGroup("CharacterVA"), SerializeField] protected EventReference _characterBreathingVA;
        [BoxGroup("CharacterVA"), SerializeField] protected EventReference _characterJumpVA;
        [BoxGroup("CharacterVA"), SerializeField] protected EventReference _characterLandingVA;
        [BoxGroup("CharacterVA"), SerializeField] protected EventReference _characterKeycardTwoVA;
        [BoxGroup("CharacterVA"), SerializeField] protected EventReference _characterKeycardOneVA;
        [BoxGroup("CharacterVA"), SerializeField] protected float _jumpVoiceDelay;


        [BoxGroup("VFX")] public TrailRenderer playerTrail;
        [BoxGroup("VFX")] public GameObject vfxSpawnPosition;
        [BoxGroup("VFX")] public GameObject vfxSpawnPosition_2;
        [BoxGroup("VFX")] public GameObject vfxSpawnPosition_3;
        [BoxGroup("Spawnable VFX")] public GameObject vfxSuccessLootPrefab;
        [BoxGroup("Spawnable VFX")] public GameObject vfxEmptyLootPrefab;
        [BoxGroup("Spawnable VFX")] public GameObject vfxDeathPrefab;
        [BoxGroup("Spawnable VFX")] public GameObject vfxShoveSuccessPrefab;
        [BoxGroup("Spawnable VFX")] public GameObject vfxShoveFailurePrefab;
        [BoxGroup("Spawnable VFX")] public GameObject vfxDashPrefab;
        [BoxGroup("Spawnable VFX")] public GameObject vfxCrouchPrefab;
        [BoxGroup("Spawnable VFX")] public GameObject vfxJumpPrefab;
        [BoxGroup("Spawnable VFX")] public GameObject vfxWalkingPrefab;
        [BoxGroup("Activatable VFX")] public GameObject vfxPromptGameObject;
        [BoxGroup("Activatable VFX")] public GameObject dashCameraEffect;
        [BoxGroup("Activatable VFX")] public GameObject dashTrailEffect;
        [BoxGroup("Activatable VFX")] public GameObject keycardCollectionEffect;
        [BoxGroup("Activatable VFX")] public float keycardEffectDuration = 1.2f;

        [SerializeField, Header("Foosteps Layer")] public LayerMask _foostepsLayers;

        [SerializeField] private Rigidbody[] ragdollRBs; 

        protected override void Awake()
        {
            base.Awake();
            mCC = GetComponentInChildren<MaterialColorChanger>(); 
            playerColor = GameManager.Instance.GetPlayerColor(PhotonNetwork.LocalPlayer.ActorNumber);
            playerStatsManager = GetComponentInParent<PlayerStatsManager>();
            photonView = GetComponent<PhotonView>();
            animator = GetComponent<Animator>();
            if (movementAction == null)
            {
                movementAction = GetComponent<PlayerInput>();
            }
            IMInstance = GetComponent<InventoryManager>();
            trapSpawner = GetComponent<TrapSpawner>();
            ownerNetworkComponent = GetComponentInParent<OwnerNetworkComponent>();
            PlayerCC = GetComponent<CapsuleCollider>();
            _RFP.position = GetComponent<Transform>().position;
            _shoveBox.GetComponent<BoxCollider>().size.Set(_shoveBoxWidth, _shoveBoxHeight, _shoveBoxLength);
            if (MinimapFollowCam.Instance != null && photonView.IsMine)
            {
                MinimapFollowCam.Instance.AssignPlayer(this.gameObject.transform);
            }
            if(photonView.IsMine)
            {
                Material newTrailMaterial = new Material(playerTrail.material);
                newTrailMaterial.color = GameManager.Instance.GetPlayerColor(PhotonNetwork.LocalPlayer.ActorNumber);
                playerTrail.material = newTrailMaterial;
            }
        }
        private float originalSpeed;
        private float originalHeight;
        private Vector3 originalCenter;

        void Start()
        {
            originalSpeed = _speed; // Store the original speed
            originalHeight = PlayerCC.height; // Store the original height
            originalCenter = PlayerCC.center; // Store the original center

        }

        protected override void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                if (HasMoveInput || trapSpawner.isPlacingTrap)
                {
                    CanTurn = true;
                }
                else
                {
                    CanTurn = false;
                }
                base.FixedUpdate();
            }

            if (IsDead)
            {
                ownerNetworkComponent.camFl.LookAt = _RFP;
                _RFP.position = _followLocation.position;
                trapSpawner.canPlaceTrap = false;
                IsCrouching = false; // Reset crouching state
                RestoreSpeed(); // Reset speed
                PlayerCC.height = originalHeight; // Reset height
                PlayerCC.center = originalCenter; // Reset center
                trapSpawner.ExitTrapMode();
                return;
            }

            if (trapSpawner.isPlacingTrap)
            {
                ownerNetworkComponent.camFl.LookAt = _trapSettingFollowPoint;
                return;
            }

            _RFP.position = GetComponent<Transform>().position;
            ownerNetworkComponent.camFl.LookAt = _RFP;
        }

        #region Character Stats Management
        public void ReduceSpeed()
        {
            if (!speedReduced) {
                speedReduced = true;
                _speed = _speed - 2f;
            }
        }

        public void RestoreSpeed()
        {
            if (speedReduced) {
                speedReduced = false;
                _speed = originalSpeed;
            }
        }
        #endregion

        public void SpawnWalkingVFX()
        {
            if (photonView.IsMine)
                SpawnVFX(vfxWalkingPrefab, vfxSpawnPosition_2.transform, true);
        }

        #region Crouch Control 
        // calls the Crouch action from the character movement
        public void OnCrouch(InputValue value)
        {
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(RPC_PerformCrouch), RpcTarget.All, value.isPressed);
            }
        }

        [PunRPC]
        public void RPC_PerformCrouch(bool isPressed)
        {
            PerformCrouch(isPressed);
        }

        public void PerformCrouch(bool isPressed)
        {
            // Toggle crouch state
            IsCrouching = isPressed;
            animator.SetBool(nameof(IsCrouching), IsCrouching);

            if (isPressed)
            {
                if (photonView.IsMine)
                {
                    #region CloudSave Data
                    LogAction("Crouch"); // log this specific movement 
                    if (CloudSave.Instance != null) {
                        if (CloudSave.Instance.playerStats.TimesCrouched <= 0)
                        {
                            CloudSave.Instance.playerStats.timeOfFirstCrouch = Time.timeSinceLevelLoad; ;
                        }
                        CloudSave.Instance.playerStats.TimesCrouched += 1;
                    }
                    #endregion
                    SpawnVFX(vfxCrouchPrefab, vfxSpawnPosition_2.transform, false); // Spawn Crouch VFX
                    RuntimeManager.PlayOneShot(CrouchSFX, transform.position); // FMOD Sound event playing
                }
                if (trapSpawner.isPlacingTrap)
                {
                    trapSpawner.ExitTrapMode();
                }
                ReduceSpeed();
                PlayerCC.height = PlayerCC.height / 1.6f;
                PlayerCC.center = PlayerCC.center / 1.6f;
            }
            else
            {
                RestoreSpeed(); // Reset speed
                PlayerCC.height = originalHeight; // Reset height
                PlayerCC.center = originalCenter; // Reset center
            }


        }

        // Calls the action of crouching and check if the player is currently crouching
        // public void Crouch()
        // {
        //     if (!CanMove && !IsGrounded) return;
        //         IsCrouching = IsCrouching ? false : true;
        // }
        #endregion

        #region Dash Control 
        // calls the input of Dash and adds force to the player
        public void OnDash(InputValue value)
        {
            if (photonView.IsMine)
            {
                if (!CanDash || IsCrouching || !IsGrounded) return;
                if (!IsDashing)
                {
                    IsDashing = true;

                    #region CloudSave Data
                    LogAction("Dash"); // log this specific movement 
                    if (CloudSave.Instance != null) {
                        if (CloudSave.Instance.playerStats.TimesDashed <= 0)
                        {
                            CloudSave.Instance.playerStats.timeOfFirstDash = Time.timeSinceLevelLoad; ;
                        }
                        CloudSave.Instance.playerStats.TimesDashed += 1;
                    }
                    #endregion

                    photonView.RPC(nameof(RPC_PerformDash), RpcTarget.All, photonView.ViewID); // sends RPC to sync movement
                }
            }
        }

        [PunRPC]
        private void RPC_PerformDash(int viewID)
        {
            PhotonView targetPV = PhotonView.Find(viewID);
            if (targetPV != null)
            {
                CustomCharacterMovement targetCCM = targetPV.GetComponent<CustomCharacterMovement>();
                if (targetCCM != null)
                {
                    targetCCM.PerformDash();
                }
            }
        }

        public void PerformDash()
        { // actual dash logic here
            IsDashing = true;
            trapSpawner.ExitTrapMode();
            animator.SetBool("IsDashing", IsDashing);
            if (photonView.IsMine)
            {
                // local client-specific actions
                SpawnVFX(vfxDashPrefab, vfxSpawnPosition_2.transform, false); // spawn dash text VFX
                //vfxDashLines.Play();
                RuntimeManager.PlayOneShot(DashSFX, transform.position); // FMOD Sound event playing
                StartCoroutine(DashCameraEffect());
                HUDManager.Instance.StartTrapCooldown(5, dashCoolDown); // update HUD only for local client
                CanDash = false;
            }
            // actions that need to be performed on all clients
            if (IsGrounded && !IsCrouching)
            {
                StartCoroutine(ApplyDelay(dashCoolDown)); // start the coroutine for delay
                if (MoveInput.magnitude > 0)
                {
                    _rigidbody.AddForce(MoveInput * _launchSpeed, ForceMode.Impulse);
                    return;
                }
                _rigidbody.AddForce(transform.forward * _launchSpeed, ForceMode.Impulse);
            }
        }

        private IEnumerator DashCameraEffect()
        {
            dashTrailEffect.SetActive(true);
            dashCameraEffect.SetActive(true);
            yield return new WaitForSeconds(dashCameraEffectDuration);
            dashCameraEffect.SetActive(false);
            dashTrailEffect.SetActive(false);
        }
        #endregion

        #region Shove Control
        // handle shove input.
        public void OnShove(InputValue value)
        {
            // Ensure this is the local player initiating the shove
            if (photonView.IsMine)
            {
                if (!CanShove || IsCrouching || !IsGrounded) return;
                if (!IsShoving)
                {
                    trapSpawner.ExitTrapMode();
                    IsShoving = true;
                    animator.SetBool("IsShoving", IsShoving);
                    RuntimeManager.PlayOneShot(ShoveSFX, transform.position); // FMOD Sound event playing
                    #region CloudSave Data
                    LogAction("Shove"); // log this specific movement 
                    if (CloudSave.Instance != null) {
                        if (CloudSave.Instance.playerStats.TimesShoved <= 0)
                        {
                            CloudSave.Instance.playerStats.timeOfFirstShove = Time.timeSinceLevelLoad;
                        }
                        CloudSave.Instance.playerStats.TimesShoved += 1;
                    }
                    #endregion
                    HUDManager.Instance.StartTrapCooldown(6, _shoveDelay); // update HUD only for local client
                    if (_shoveTarget != null)
                    {
                        int targetViewID = _shoveTarget.GetComponent<PhotonView>().ViewID;
                        photonView.RPC("ApplyShove", RpcTarget.All, targetViewID); // Use an RPC to ensure the shove force is applied on the correct client
                    }
                    else
                    {
                        //Debug.Log("Shove failed, no target.");
                        if (photonView.IsMine)
                            SpawnVFX(vfxShoveFailurePrefab, this.vfxSpawnPosition_3.transform, false);
                    }
                    CanShove = false;
                    StartCoroutine(ApplyDelay(_shoveDelay));
                }
            }
        }

        // RPC to apply shove force to the target player.
        [PunRPC]
        public void ApplyShove(int targetViewID)
        {
            LogAction("WasShoved"); // log this specific movement 
            PhotonView targetPhotonView = PhotonView.Find(targetViewID);
            if (targetPhotonView != null)
            {
                Rigidbody targetRigidbody = targetPhotonView.GetComponent<Rigidbody>();
                if (targetRigidbody != null)
                {
                    Vector3 forceDirection = targetRigidbody.transform.position - transform.position;
                    forceDirection.y = 0;
                    forceDirection.Normalize();

                    targetRigidbody.AddForce(forceDirection * _shoveForce, ForceMode.Impulse);
                    //Debug.Log($"Shove applied to: {targetRigidbody.gameObject.name} with force: {forceDirection * _shoveForce}");

                    if (photonView.IsMine)
                        SpawnVFX(vfxShoveSuccessPrefab, this.vfxSpawnPosition_3.transform, true);
                        RuntimeManager.PlayOneShot(ShovesuccessSFX, transform.position); // FMOD Sound event playing

                }
            }
        }
        #endregion

        #region Jump Control

        public void OnJump()
        {
            if (!IsGrounded || IsCrouching) return;
            if (photonView.IsMine)
            {

                Jump();
                SpawnVFX(vfxJumpPrefab, vfxSpawnPosition_2.transform, false);  // Spawn Jump VFX
                IsJumping = true;
                RuntimeManager.PlayOneShot(JumpSFX, transform.position); // FMOD Sound event playing
                RuntimeManager.PlayOneShot(_characterJumpVA, transform.position);
                StartCoroutine(ApplyDelay(_jumpVoiceDelay));
            }
        }
        public override void Jump()
        {
            if (!photonView.IsMine) return;
            trapSpawner.ExitTrapMode();
            LogAction("Jump"); // log this specific movement 
            //animator.applyRootMotion = true;
            base.Jump();
            //animator.applyRootMotion = false;
        }

        #endregion

        #region Death Control
        // Activates ragdoll on player death and makes the camera follow it, and drops player's keycards
        public void OnDeath(int playerNr, int trapType)
        {
            if (photonView.IsMine)
            {
                LogAction("Died"); // log this specific movement 
                if(trapType <= 1 || trapType >= 4)
                {
                    RuntimeManager.PlayOneShot(_characterDeathLongVA, transform.position);
                }
                else
                {
                    RuntimeManager.PlayOneShot(_characterDeathShortVA, transform.position);
                }
                HUDManager.Instance.ApplyFadeInOut();
                photonView.RPC(nameof(RPC_ToggleRBKinematics), RpcTarget.All, photonView.ViewID, false);
                photonView.RPC(nameof(RPC_PerformDeath), RpcTarget.All, photonView.ViewID, playerNr, trapType);
            }
        }

        [PunRPC]
        public void RPC_PerformDeath(int viewID, int playerNr, int trapType)
        {
            PhotonView targetPhotonView = PhotonView.Find(viewID);
            if (targetPhotonView != null)
            {
                CustomCharacterMovement target = targetPhotonView.GetComponent<CustomCharacterMovement>();
                if (target != null)
                {
                    target.PerformDeath(playerNr, trapType);
                }
            }
        }

        public void PerformDeath(int playerNr, int trapType)
        {
            if (IsDead) return;
            IsDead = true;
            // instantiate a keycard
            if (photonView.IsMine && !attemptingToDropKeycard)
            {
                attemptingToDropKeycard = true;
                spotlight.SetActive(false);
                playerStatsManager.UpdateDeaths(playerNr, trapType); //? add 1 to death
                int currentKeyCardCount = playerStatsManager.GetCustomProperty(PlayerStatsManager.KeycardsCollected);
                if (currentKeyCardCount > 0)
                {
                    playerStatsManager.SetCustomProperty(PlayerStatsManager.KeycardsCollected, currentKeyCardCount - 1);
                    KeycardManager.Instance.DropKeyCard(this.transform.position);
                }
            }

            ToggleRBKinematics(false);

            // animator.enabled = false; // turn on ragdoll
            if (movementAction != null) {
                movementAction.enabled = false; // disable any input from the player
            }
                    
            if (photonView.IsMine) {
                SpawnVFX(vfxDeathPrefab, vfxSpawnPosition.transform, true);
                StartCoroutine(PlaySoundWithDelay(0.2f)); // FMOD Sound event playing                      
            }
            StartCoroutine(ApplyDelay(_respawnTimer)); // wait for 3 seconds before respawning with coroutine
            IEnumerator PlaySoundWithDelay(float delay)
            {
                yield return new WaitForSeconds(delay); // 0.2sec
                RuntimeManager.PlayOneShot(DeathSFX, transform.position); // FMOD event playing
            }
        }
        #endregion

        public void EnableKeycardVFX()
        {
            Debug.Log(IMInstance.keycardsRequired);
            Debug.Log(IMInstance.keycardsCollected);
            if(IMInstance.keycardsCollected == 2) RuntimeManager.PlayOneShot(_characterKeycardOneVA, transform.position);
            if(IMInstance.keycardsCollected == 1) RuntimeManager.PlayOneShot(_characterKeycardTwoVA, transform.position);
            StartCoroutine(KeycardVFX());
        }

        public IEnumerator KeycardVFX()
        {
            //Debug.Log("Keycard!");
            keycardCollectionEffect.SetActive(true);
            yield return new WaitForSeconds(keycardEffectDuration);
            keycardCollectionEffect.SetActive(false);
        }

        public void OnCheckStats(InputValue inputValue)
        {
            // get the input value (pressed)
            bool isPressed = inputValue.isPressed;
            if (isPressed)
            {
                // toggle zoom state
                if (MinimapFollowCam.Instance.isZoomedOut)
                {
                    MinimapFollowCam.Instance.ResetZoom();
                    return;
                }
                MinimapFollowCam.Instance.ZoomOut();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<InteractableObject>() && currentInteractable == null)
            {
                currentInteractable = other.GetComponent<InteractableObject>();
                CanInteract = true;
                if (photonView.IsMine)
                {
                    HUDManager.Instance.disableOverlay.enabled = false;
                }
                return;
            }
            if (endGameButton == null && other.GetComponent<WorldButtonInteractable>())
            {
                endGameButton = other.GetComponent<WorldButtonInteractable>();
            }
        }

        // When the player leaves the range of an interactable object, turns off the CanInteract check.
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<InteractableObject>() && currentInteractable == other.GetComponent<InteractableObject>() && currentInteractable)
            {
                currentInteractable = null;
                CanInteract = false;
                if (photonView.IsMine)
                {
                    HUDManager.Instance.disableOverlay.enabled = true;
                }
                return;
            }
            endGameButton = null;
        }

        // When the player stays in collision with another player, sets the target for shoving.
        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _shoveTarget = other.collider.attachedRigidbody;
                //Debug.Log("Shove target acquired: " + _shoveTarget.gameObject.name);
            }
        }

        // When the player exits collision with another player, clears the shove target.
        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _shoveTarget = null;
            }
        }

        // handle interaction input.
        public void OnInteract(InputValue value)
        {
            if (endGameButton != null)
            {
                endGameButton.Interact(this);
            }
            if (!CanInteract) return;
            if (!photonView.IsMine) return;
            if (currentInteractable != null)
            {
                Debug.Log($"Pressed Interact");
                currentInteractable.Interact();
                IsInteracting = true;
                CanInteract = true;
                if (movementAction != null)
                {
                    movementAction.enabled = false; // disable/enable character movement while interacting
                }
                StartCoroutine(ApplyDelay(_interactTimer)); // start delay timer
            }
        }

        //  handle player respawn.
        public void OnRespawn()
        {
            // ToggleRBKinematics(true);
            LogAction("Spawned"); // log this specific movement 
            attemptingToDropKeycard = false;
            // animator.enabled = true; // turn off ragdoll
            photonView.RPC(nameof(RPC_ToggleRBKinematics), RpcTarget.All, photonView.ViewID, true);
            mCC.EnableDisableCloth();
            spotlight.SetActive(true);
            if (movementAction != null)
            {
                movementAction.enabled = true; // enable input from the player
            }
        }

        // Coroutine to apply delay for various actions.
        private IEnumerator ApplyDelay(float delay)
        {
            if (IsDashing || IsShoving) yield return new WaitForSeconds(.25f);
            if(!CanDash) RuntimeManager.PlayOneShot(_characterBreathingVA, transform.position);
            IsDashing = false;
            IsShoving = false; // Shoving
            yield return new WaitForSeconds(delay);
            if (IsDead && !IsJumping) // Death
            {
                onDeathEvent.Raise(this.gameObject);
                OnRespawn();
                IsDead = false;
            }
            if(IsJumping)
            {
                RuntimeManager.PlayOneShot(_characterLandingVA, transform.position);
                IsJumping = false;
            }
            CanDash = true; // Dash
            if (movementAction != null)
            {
                movementAction.enabled = true; // Enables actions and stops interacting
            }
            IsInteracting = false; // Interact
            CanInteract = false;
            CanShove = true;
        }

        public void EnableVFXPrompt()
        {
            vfxPromptGameObject.SetActive(true);
        }

        public void DisableVFXPrompt()
        {
            vfxPromptGameObject.SetActive(false);
        }

        // Can be called to spawn any VFX with a prefab, spawn position and life time.
        private void SpawnVFX(GameObject vfxPrefab, Transform vfxSpawnTransform, bool setAsChild)
        {
            if (vfxPrefab != null)
            {
                GameObject vfxInstance = PhotonNetwork.Instantiate(vfxPrefab.name, vfxSpawnTransform.position, Quaternion.identity);
                vfxInstance.GetComponentInChildren<Image>().color = playerColor;
                if (setAsChild)
                    vfxInstance.transform.SetParent(vfxSpawnTransform);
            }
            else
            {
                Debug.LogWarning($"{vfxPrefab.name} has not been assigned in the inspector!");
            }
        }

        // Local function to log player action in CloudSave for data tracking 
        public void LogAction(string action)
        {
            CloudSave.FloatPair position = new CloudSave.FloatPair(
                MathF.Round(transform.position.x * 100f) / 100f,
                MathF.Round(transform.position.z * 100f) / 100f
            );
            float time = Time.timeSinceLevelLoad;
            if (CloudSave.Instance != null)
            {
                CloudSave.Instance.playerStats.PlayerActions.Add(new CloudSave.PlayerActionTracking(action, position, time));
            }
        }

        #region Photon Callbacks
        // ensure IsDead for all clients is synced across the network
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!stream.IsWriting)
            {
                IsCrouching = (bool)stream.ReceiveNext();
                IsGrounded = (bool)stream.ReceiveNext();
                IsDead = (bool)stream.ReceiveNext();
                attemptingToDropKeycard = (bool)stream.ReceiveNext();
                animator.enabled = (bool)stream.ReceiveNext();
                if (movementAction != null)
                {
                    movementAction.enabled = (bool)stream.ReceiveNext();
                }
                return;
            }
            stream.SendNext(IsCrouching);
            stream.SendNext(IsDead);
            stream.SendNext(IsGrounded);
            stream.SendNext(attemptingToDropKeycard);
            stream.SendNext(animator.enabled);
            if (movementAction != null)
            {
                stream.SendNext(movementAction.enabled);
            }
        }
        #endregion

        #region Ground Checks
        protected override bool CheckGrounded()
        {
            bool result = base.CheckGrounded();
            RaycastHit floorHitInfo;
            if(Physics.Raycast(_groundCheckStart, -base.transform.up, out floorHitInfo, _groundCheckDistance, _foostepsLayers))
            {
                // Debug.Log($"This is what is showing: {floorHitInfo.collider.gameObject}");
                base.SurfaceObject = floorHitInfo.collider.gameObject;
            }
            return result;
        }
        #endregion

        [PunRPC]
        private void RPC_ToggleRBKinematics(int viewID, bool newVal) {
            PhotonView targetPV = PhotonView.Find(viewID);
            if (targetPV != null) {
                CustomCharacterMovement targetCCM = targetPV.GetComponent<CustomCharacterMovement>();
                Debug.Log($"Playing Kinematics bool on {targetPV}, turning kinematics to {newVal}");
                if (targetCCM != null) {
                    targetCCM.ToggleRBKinematics(newVal);
                } else {
                    Debug.LogWarning($"CustomCharacterMovement component not found on PhotonView {viewID}");
                }
            }
        }

        private void ToggleRBKinematics(bool newVal) {
            for(int i = 0; i < ragdollRBs.Length; i++)  {
                ragdollRBs[i].isKinematic = newVal;
                Debug.Log($"Rigidbody {ragdollRBs[i].name} kinematic set to: " + newVal);
            }
            animator.enabled = newVal;
        }
    }
}
