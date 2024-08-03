using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using CharacterMovement;
using Sirenix.OdinInspector;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.Polybrush;
using FMODUnity;

public class TrapSpawner : MonoBehaviourPun
{
    // Define a Trap class to hold each trap's prefab, cooldown, and track the last time it was spawned.
    [System.Serializable]
    public class Trap
    {
        public GameObject prefab; // The trap prefab to spawn.
        public float cooldown; // Time in seconds before the trap can be spawned again.
        private float lastSpawnTime = float.NegativeInfinity; // Track when the trap was last spawned.
        public GameObject trapInstance;

        // Property to check if the trap's cooldown has elapsed and it's ready to spawn again.
        public bool IsReadyToSpawn => Time.time - lastSpawnTime >= cooldown;

        // Method to spawn the trap if its cooldown has elapsed.
        [PunRPC]
        public void Spawn(Vector3 position, Quaternion rotation)
        {
            if (IsReadyToSpawn)
            {
                GameObject instance = PhotonNetwork.Instantiate(prefab.name, position, rotation); // Network instantiate for multiplayer environment.
                trapInstance = instance;
                lastSpawnTime = Time.time; // Update the last spawn time.
            }
        }
    }
    static CustomCharacterMovement CCMInstance;

    [BoxGroup("VFX"), SerializeField] private GameObject vfxTrapSpawn;
    [BoxGroup("VFX"), SerializeField] private float vfxLifetime = 4f;
    [field: SerializeField, BoxGroup("SFX")] public EventReference TrapSpawnSFX { get; protected set; }
    [field: SerializeField, BoxGroup("SFX")] public EventReference TrapPreviewCancelSFX { get; protected set; }

    [Header("Traps Configuration")]
    // Serialize fields for each trap type so they can be assigned in the Unity Editor.
    [SerializeField] private Trap poisonDartTrap;
    [SerializeField] private Trap bouncingBettyTrap;
    [SerializeField] private Trap detonatorTrap;
    [SerializeField] private Trap ceilingTrap;

    [Header("Hologram Settings")]
    // Serialize fields for each trap hologram type so they can be assigned in the Unity Editor.
    [SerializeField] private GameObject poisonDartHologram;
    [SerializeField] private GameObject bouncingBettyHologram;
    [SerializeField] private GameObject detonatorHologram;
    [SerializeField] private GameObject ceilingTrapHologram;
    [HideInInspector] public GameObject currentHologram;
    [SerializeField] private GameObject cancelSign;

    [Header("Line Renderer Settings")]
    // Serialize fields for the LineRenderer component and its width settings.
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float startWidth = 0.05f;
    [SerializeField] private float endWidth = 0.05f;
    [SerializeField] private Color lineStartColor = Color.white;
    [SerializeField] private Color lineEndColor = Color.white;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float playerHeight = 2.0f; // Height of the player.

    //private CustomPlayerController playerController;
    private PlayerStatsManager playerStatsManager;
    private Dictionary<KeyCode, Trap> trapKeyMap; // A dictionary to map keyboard inputs to their respective trap types.
    private Trap currentTrap; // The currently selected trap based on the last key press.
    private bool isLineVisible = false; // Visibility state of the line renderer
    public bool isPlacingTrap = false;
    public bool canPlaceTrap;

    private void Awake()
    {
        CCMInstance = GetComponent<CustomCharacterMovement>();
        //playerController = GetComponent<CustomPlayerController>();
        playerStatsManager = GetComponentInParent<PlayerStatsManager>();
    }

    private void Start()
    {
        SetupLineRenderer(); // Setup the line renderer properties on start.
        trapKeyMap = new Dictionary<KeyCode, Trap> // Initialize key mapping for trap types.
        {
            { KeyCode.Alpha1, poisonDartTrap },
            { KeyCode.Alpha2, bouncingBettyTrap },
            { KeyCode.Alpha3, ceilingTrap },
            { KeyCode.Alpha4, detonatorTrap }
        };
    }

    public Material hologramMaterial;
    public Color canPlaceColor;
    public Color cantPlaceColor;

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (isPlacingTrap)
            {
                CCMInstance.CanMove = true;
                UIManager.Instance.ToggleControlsUI(false, true);
            }
            else
            {
                UIManager.Instance.ToggleControlsUI(true, false);
                cancelSign.SetActive(false);
                GetComponent<CustomPlayerController>().DisableText();
            }
        }

        CheckInputAndSpawnTrap(); // Check for user input to spawn traps.
        if (!isPlacingTrap) return; // 
        // Update line renderer position if it's visible
        if (isLineVisible)
        {
            UpdateSpawnLine();
        }

        if (canPlaceTrap)
        {
            hologramMaterial.color = canPlaceColor;
            cancelSign.SetActive(false);
        }
        else
        {
            hologramMaterial.color = cantPlaceColor;
            cancelSign.SetActive(true);

        }

        if (!isPlacingTrap)
        {
            hologramMaterial.color = canPlaceColor;
            cancelSign.SetActive(false);
        }
    }

    [SerializeField] private int trapIndex;
    // Checks if a key corresponding to a trap is pressed or released.
    public void CheckInputAndSpawnTrap()
    {
        if (GetComponentInParent<PhotonView>().IsMine) // Only control traps if the PhotonView is owned by this client
        {
            if (CCMInstance.IsCrouching) return;
            foreach (var entry in trapKeyMap)
            {
                // On key press, set the current trap and show the spawn line.
                if (Input.GetKeyDown(entry.Key))
                {
                    //playerController.EnableLookInCameraDirection();
                    canPlaceTrap = true;
                    // TextRenderer.TRInstance.UpdateText("");
                    currentTrap = entry.Value;
                    trapIndex = KeyCodeToIndex(entry.Key);
                    if (HUDManager.Instance.GetCD(trapIndex))
                    {
                        // todo add trap icon CD notifier in the HUD    
                        return;
                    }
                    EnterTrapPlacementMode();
                    SelectedTrapHighlighter.Instance.UpdateTrapSelected(trapIndex);
                }

                // Confirm Trap if player clicked left click
                if (Input.GetMouseButtonDown(0) && isPlacingTrap && canPlaceTrap && !HUDManager.Instance.GetCD(trapIndex)) //todo: a check to make sure it only goes in this block if successful in spawning a trap i.e. trap not on cooldown
                {
                    if (currentTrap == detonatorTrap)
                    {
                        //currentHologram.GetComponent<DTHologram>();
                        currentHologram.GetComponent<DTHologram>().hit.collider.gameObject.GetComponent<InteractableObject>().hasTrap = true;
                        InteractableObject io = currentHologram.GetComponent<DTHologram>().hit.collider.gameObject.GetComponent<InteractableObject>();
                        currentTrap.Spawn(currentHologram.transform.position, currentHologram.transform.rotation);
                        io.trapOwnerID = PhotonNetwork.LocalPlayer.ActorNumber;
                        currentTrap.trapInstance.transform.SetParent(io.transform);
                        io.trapInstance = currentTrap.trapInstance;
                        io.replayCamInstance
                    }
                    else
                    {
                        currentTrap.Spawn(currentHologram.transform.position, currentHologram.transform.rotation);
                    }
                    SpawnVFX(vfxTrapSpawn, currentHologram.transform, vfxLifetime);
                    // Spawn Trap
                    RuntimeManager.PlayOneShot(TrapSpawnSFX, transform.position);
                    // FMOD Sound event playing
                    // TextRenderer.TRInstance.UpdateText($"{currentTrap.prefab.name} is set!");
                    //! updates specific trap set
                    playerStatsManager.UpdateStat($"trap{trapIndex}", "add");
                    //! updates all traps set
                    playerStatsManager.UpdateStat(PlayerStatsManager.TrapsSet, "add");

                    HUDManager.Instance.StartTrapCooldown(trapIndex, currentTrap.cooldown); // Start cooldown in GameManager.

                    ExitTrapMode();

                    CCMInstance.LogAction($"Trap{trapIndex}Set"); // log this specific movement 
                    #region CloudSave Data
                    if (CloudSave.Instance.playerStats.TrapsSet <= 0)
                    {
                        CloudSave.Instance.playerStats.timeOfFirstTrapSet = Time.timeSinceLevelLoad;
                    }
                    CloudSave.Instance.playerStats.TrapsSet += 1;
                    CloudSave.Instance.playerStats.TrapsSetByType[$"Trap{trapIndex}"] += 1;
                    #endregion
                }

                // cancel trap spawning check  if the player clicked right click
                if (Input.GetMouseButtonDown(1) && isPlacingTrap)
                {
                    // TextRenderer.TRInstance.UpdateText($"Setting{currentTrap.prefab.name} cancelled!");
                    RuntimeManager.PlayOneShot(TrapPreviewCancelSFX, transform.position);
                    // FMOD Sound event playing
                    ExitTrapMode();
                    currentTrap = null;
                    currentHologram.SetActive(false);
                }
            }
        }
    }

    // Helper function to convert KeyCode to an integer index for cooldown management.
    int KeyCodeToIndex(KeyCode key)
    {
        return key - KeyCode.Alpha1 + 1; // Convert key code to trap index.
    }

    public void ExitTrapMode()
    {
        SelectedTrapHighlighter.Instance.Hide();
        isLineVisible = false;
        lineRenderer.enabled = false;
        isPlacingTrap = false;
        isSnapping = false;

        currentTrap = null;
        if (currentHologram != null)
        {
            currentHologram.SetActive(false);
        }

        //playerController.DisableLookInCameraDirection();

        CCMInstance.CanMove = true;
        CCMInstance.RestoreSpeed();
    }

    // Toggles the visibility of the LineRenderer and HologramTrap and sets its positions.
    private void EnterTrapPlacementMode()
    {
        isLineVisible = true;
        lineRenderer.enabled = true; // Enable or disable the line renderer based on 'show'.
        isPlacingTrap = true;

        CCMInstance.CanMove = false;
        CCMInstance.ReduceSpeed();

        if (currentHologram != null)
        {
            currentHologram.SetActive(false);
        }

        switch (trapIndex)
        {
            case 1:
                currentHologram = poisonDartHologram;
                break;
            case 2:
                currentHologram = bouncingBettyHologram;
                break;
            case 3:
                currentHologram = ceilingTrapHologram;
                break;
            case 4:
                currentHologram = detonatorHologram;
                break;
            default:
                currentHologram = null;
                break;
        }

        if (isPlacingTrap)
        {
            if (currentHologram != null)
            {
                currentHologram.SetActive(true);
            }
        }
        else
        {
            if (currentHologram != null)
            {
                currentHologram.SetActive(false);
            }
        }
    }

    [SerializeField] private GameObject hitPointTransform;

    [BoxGroup("Trap Placement Settings")] public Transform castOrigin;
    [BoxGroup("Trap Placement Settings")] public float castRadius = 1f;
    [BoxGroup("Trap Placement Settings")] public float castDistance = 10f;
    [BoxGroup("Trap Placement Settings")] public LayerMask doorMask;
    [BoxGroup("Trap Placement Settings")] public LayerMask wallMask;
    [BoxGroup("Trap Placement Settings")] public LayerMask floorMask;
    [BoxGroup("Trap Placement Settings")] public LayerMask propsMask;
    [BoxGroup("Trap Placement Settings")] public LayerMask interactablesMask;
    [BoxGroup("Trap Placement Settings")] public LayerMask ceilingMask;
    [BoxGroup("Trap Placement Settings")] public LayerMask trapsMask;
    [BoxGroup("Trap Placement Settings")] public LayerMask playerMask;
    [BoxGroup("Trap Placement Settings")] public LayerMask roomBehaviour;
    [BoxGroup("Trap Placement Settings")] public LayerMask ragdoll;
    [BoxGroup("Trap Placement Settings")] public Vector3 trapOffSet = new Vector3(0f, 3f, 0f);
    [BoxGroup("Trap Placement Settings")] public float distanceToPlaceTrap = 5f;
    public RaycastHit cameraHit;
    public Camera camObj;
    public float yPos = 0;
    public Vector3 startPoint;
    public Vector3 pointOnCurve;
    public Vector3 endPoint;
    public bool isSnapping;

    // Updates the LineRenderer positions to create a curved line in front of the player.
    private void UpdateSpawnLine()
    {
        startPoint = transform.position + new Vector3(0, playerHeight / 2, 0);
        endPoint = transform.position + transform.forward * 3;
        Vector3 controlPoint = (startPoint + endPoint) / 2 + Vector3.up;

        lineRenderer.positionCount = 100;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            float t = i / (float)(lineRenderer.positionCount - 1);
            pointOnCurve = (1 - t) * (1 - t) * startPoint + 2 * (1 - t) * t * controlPoint + t * t * endPoint;
            lineRenderer.SetPosition(i, pointOnCurve);

            endPoint = currentHologram.transform.position;

            camObj = GetComponentInParent<OwnerNetworkComponent>().camObj;

            if (!isSnapping)
            {
                if (Physics.Raycast(camObj.transform.position, camObj.transform.forward, out cameraHit, 50f, floorMask + ~propsMask + ~interactablesMask + ~ceilingMask + ~playerMask + ~roomBehaviour))
                {
                    // Debug.Log("Hit: " + cameraHit.collider.gameObject.name);
                    Vector3 newPosition = new Vector3(cameraHit.point.x, 0.06f, cameraHit.point.z);
                    float distance = Vector3.Distance(transform.position, newPosition);

                    if (distance <= distanceToPlaceTrap)
                    {
                        currentHologram.transform.position = newPosition;
                        //canPlaceTrap = true;
                    }
                    else
                    {
                        // Clamp the position within the maximum distance
                        Vector3 direction = (newPosition - transform.position).normalized;
                        currentHologram.transform.position = transform.position + direction * distanceToPlaceTrap;
                        //canPlaceTrap = true;
                    }
                }
                else
                {
                    currentHologram.transform.position = new Vector3(cameraHit.point.x, 0.06f, cameraHit.point.z);
                    canPlaceTrap = false;
                }
            }
            else
            {
                endPoint = currentHologram.transform.position;
            }
        }
    }

    // Configures the LineRenderer component based on the serialized settings.
    private void SetupLineRenderer()
    {
        if (lineRenderer != null)
        {
            lineRenderer.material = lineMaterial; // Assign the specified material.
            lineRenderer.startWidth = startWidth; // Set the start width.
            lineRenderer.endWidth = endWidth; // Set the end width.
            lineRenderer.positionCount = 2; // LineRenderer requires at least two points.
            lineRenderer.enabled = false; // Initially hide the LineRenderer.
            lineRenderer.startColor = lineStartColor; // Set the start color.
            lineRenderer.endColor = lineEndColor; // Set the end color.
        }
    }

    public void SpawnVFX(GameObject vfxInstance, Transform vfxTransform, float vfxLifetime)
    {
        if (vfxInstance is not null)
        {
            GameObject instance = Instantiate(vfxInstance, vfxTransform.position, vfxTransform.rotation);
            Destroy(instance, vfxLifetime);
        }
    }
}