<p align="center">
  <img src="./ReadMeAssets/LayOffHeader.gif" alt="LayOff Game Logo" width="200">
</p>
<h1 align="center">LayOff</h1>

<p align="center">
  LayOff is a 4-player trap setting online multiplayer strategy game where players compete to collect all the required keycards and escape through locked doors while avoiding traps set by opponents.
</p>

<h2>Installation</h2>

<p>To set up the project locally, follow these steps:</p>

<pre>
<code>
git clone https://github.com/AlexBuzmion/LayOff.git
cd LayOff
# Go to Unity hub and add the cloned repo folder. 
# Adding the folder should automatically open up the file in Unity. 
# If the file did not open, click the newly added projects' name in Unity Hub. 
</code>
</pre>

<h2>Overview of Core Gameplay</h2>
<p>
  <strong>LayOff</strong> is a strategic, fast-paced, 4-player deathmatch where players take on the role of spies in a high-stakes race to secure keycards and escape. Each match is a thrilling blend of stealth, speed, and strategy. Players must navigate through rooms filled with potential dangers, including traps set by their opponents, while racing against the clock to collect keycards hidden within interactable objects.
</p>
<p>
  The core mechanics include:
</p>
<ul>
  <li><strong>Keycard Collection:</strong> Search rooms and loot furniture to find the keycards necessary to unlock the final door.</li>
  <li><strong>Trap Placement:</strong> Set traps to incapacitate opponents, preventing them from reaching keycards or escaping.</li>
  <li><strong>Movement and Evasion:</strong> Move quickly and carefully through the environment, avoiding traps and outmaneuvering rivals.</li>
  <li><strong>Final Escape:</strong> Use the collected keycards to unlock the final door and secure your escape before time runs out.</li>
</ul>

<h2>About the Project</h2>
<p>
  LayOff is a student project developed at Vancouver Film School, designed to showcase both gameplay design and technical implementation. This project was a collaborative effort where I played multiple roles, including:
</p>
<ul>
  <li><strong>Gameplay Implementation:</strong> Developed the core gameplay mechanics, including the keycard collection system and trap placement logic.</li>
  <li><strong>Network/Online Readiness:</strong> Integrated multiplayer functionality, ensuring smooth gameplay experiences for up to 4 players.</li>
  <li><strong>User Interface and Player HUD:</strong> Designed and implemented the in-game user interface, including HUD elements that keep players informed of their keycard count and remaining time.</li>
</ul>
<p>
  The project demonstrates a strong understanding of game development processes, from concept to execution, and reflects the practical skills gained during my time at Vancouver Film School.
</p>

<!-- PROJECT FILES DESCRIPTION -->
<h2 id="project-files-description"> :floppy_disk: Project Files Description</h2>

### Character Control
| File | Description |
|------|-------------|
| `CustomCharacterAnimations.cs` | Handles custom character animations. |
| `CustomWatchAnimations.cs` | Manages watch gadget animations. |
| `CharacterInputActions.inputactions` | Defines character input actions. |
| `CustomCharacterMovement.cs` | Manages character movement. |
| `CustomPlayerController.cs` | Handles player input and actions. |
| `PlayerStatsManager.cs` | Tracks and updates player stats. |
| `CharacterColors.cs` | Handles character color customization. |
| `InventoryManager.cs` | Controls player inventory management. |

### Traps
| File | Description |
|------|-------------|
| `TrapSpawner.cs` | Spawns and manages traps. |
| `TrapEffect.cs` | Manages trap effects (Base class). |
| `TrapTrigger.cs` | Handles trap triggers (Base class). |
| `BouncingBettyEffect.cs` | Manages Bouncing Betty effects. |
| `ReplayCamTrigger.cs` | Controls replay camera triggers attached to trap triggers. |
| `BouncingBettyTrigger.cs` | Handles Bouncing Betty triggers. |
| `BBHologram.cs` | Controls Bouncing Betty hologram. |
| `CeilingTrapEffect.cs` | Manages ceiling trap effects. |
| `CeilingTrapTrigger.cs` | Controls ceiling trap triggers. |
| `CTHologram.cs` | Manages ceiling trap hologram. |
| `DetonatorEffect.cs` | Manages detonator effects. |
| `DetonatorTrigger.cs` | Handles detonator triggers. |
| `DTHologram.cs` | Controls detonator hologram. |
| `PoisonDartTrigger.cs` | Handles Poison Dart triggers. |
| `PDHologram.cs` | Manages Poison Dart hologram. |
| `VisionSystem.cs` | A component that handles a game objects vision and detection logic. |

### Environment
| File | Description |
|------|-------------|
| `KeyCard.cs` | Represents keycards. |
| `WorldButtonInteractable.cs` | Manages world button interactions. |
| `InteractableObject.cs` | Defines interactive objects. |
| `DoorTriggerVolume.cs` | Triggers door interactions. |
| `FinalDoorBehavior.cs` | Controls final door mechanics. |
| `ReplayCam.cs` | Captures and manages replay footage. |
| `RoomBehavior.cs` | Manages room-specific behaviors. |


### Game Management
| File | Description |
|------|-------------|
| `GameManager.cs` | Core script for game flow and state management. |
| `FinalRoomManager.cs` | Manages final room gameplay. |
| `TimerManager.cs` | Controls the game’s timer. |
| `LayOffGameEventListener.cs` | Listens for and triggers game events. |
| `GameEventTypes.cs` | Defines game event types. |
| `OwnerNetworkComponent.cs` | Synchronizes network components. |
| `PunLauncher.cs` | Manages Photon network connection. |
| `CloudSave.cs` | Manages cloud save data. |

### HUD
| File | Description |
|------|-------------|
| `HUDManager.cs` | Manages the players HUD elements. |
| `RoomHighlighter.cs` | Highlights objects or areas in rooms. |
| `KeyCardRingAnimator.cs` | Animates keycard rings on HUD Minimap. |
| `KillFeed.cs` | Displays recent kills. |
| `KillFeedEntry.cs` | Represents entries in the kill feed. |
| `MinimapFollowCam.cs` | Controls minimap camera. |
| `PlayerStatsEntry.cs` | Displays player stats on HUD. |
| `ReplayDisplayWindow.cs` | Manages replay display window. |
| `RoomPlayersKeycards.cs` | Tracks keycards collected by players. |
| `SelectedTrapHighlighter.cs` | Highlights selected trap on HUD. |

### UI
| File | Description |
|------|-------------|
| `ButtonAnimator.cs` | Animates UI buttons. |
| `ButtonBehavior.cs` | Manages button interactions. |
| `ModalAnimator.cs` | Animates modal dialogs. |
| `RoomListEntry.cs` | Represents entries in the room list. |
| `TextRenderer.cs` | Renders text in the UI. |
| `UIEndGame.cs` | Handles end game UI elements. |
| `UIManager.cs` | Manages overall UI elements. |
| `PlayerRoomEntry.cs` | Represents player entries in room UI. |
| `PlayerRoomEntryList.cs` | Manages the list of player entries in room UI. |
| `EndGameScreen.cs` | Manages the end game screen. |
| `LocalPlayerStatEntry.cs` | Displays local player stats at game end. |
| `OtherPlayerStatEntry.cs` | Displays other players' stats at game end. |

### Visual Effects
| File | Description |
|------|-------------|
| `CameraLookAt.cs` | Controls camera focus during events. |
| `FadeInOut.cs` | Manages fade transitions. |
| `FPSDisplay.cs` | Displays FPS on HUD. |
| `MaterialData.cs` | Scriptable object that stores material-related data. |

<h2>Usage</h2>

<p>Launch the game, click `Create Room` and update the number of players that will join (1-4 only). After the game countdown, all players will be spawned in the office/world. Use the traps to hinder your opponents and be the first to escape!</p>

<h2>Script Logic and Flow Explained</h2>

<!-- SCENARIO1 -->
<h3 id="scenario1"> :small_orange_diamond: Game Initialization</h3>

<p>The game initialization process ensures that players are properly connected to the Photon network and Unity Cloud Services before they can interact with the main menu.</p>

<p align="center"> 
<img src="./ReadMeAssets/mainMenuConnect.gif" alt="Game Initialization">
</p>

<p>As soon as the game is started, all main menu UI elements are disabled, with a "Connecting" text rendered to indicate the game's connection status. The initialization sequence involves establishing connections to both the Photon network and Unity Cloud Services:</p>

<ul>
  <li>The <code>PunLauncher.cs</code> script establishes the connection to the Photon network.</li>
  <li>The <code>CloudSave.cs</code> script connects to Unity Cloud Services and initializes a new <code>PlayerClass</code> data structure. This structure will be used to store player information throughout the game and will be saved to the cloud at the end of the game.</li>
</ul>

<p>Once the connection to both services is confirmed:</p>

<ul>
  <li>The <code>UIManager.cs</code> script enables the menu elements.</li>
  <li>The <code>ButtonAnimator.cs</code> script animates the buttons, making them fly in from right to left. The animation and button positioning are designed to be scalable across different device aspect ratios.</li>
</ul>

<p>The initialization sequence is triggered automatically when the game starts:</p>

<pre><code>$ Launch the game application
$ The "Connecting" text is displayed via UIManager.cs
$ Photon network and Unity Cloud Services connections are established via PunLauncher.cs and CloudSave.cs
$ Button animation plays once connected, handled by ButtonAnimator.cs
</code></pre>

<!-- SCENARIO2 -->
<h2 id="scenario2"> :small_orange_diamond: Lobby Setup</h2>

<p>The lobby setup process is divided into two main paths: creating a room and joining a room. Each path involves specific steps and scripts to manage the UI and player interactions.</p>

<p align="center"> 
<img src="./ReadMeAssets/createAndJoin.gif" alt="Lobby Setup">
</p>

### **Creating a Room**

<p>When a player chooses to create a room, the following sequence occurs:</p>

<ul>
  <li>The player is routed to a screen where they are prompted to enter the maximum number of players and the room name.</li>
  <li>Once confirmed and the player clicks "Create," they are taken to the lobby UI, where they wait for the other players to join.</li>
  <li>The <code>UIManager.cs`</code> script controls the appearance and disappearance of UI elements, updating the room name text in the lobby.</li>
  <li><code>PunLauncher.cs</code> handles the room creation and makes it visible for other clients to join.</li>
  <li><code>PlayerRoomEntryList.cs</code> creates an in-room entry prefab that is fed information such as player name, ready state, and avatar through <code>PlayerRoomEntry.cs</code>.</li>
</ul>

<p>Steps to create a room:</p>

<pre><code>$ Click "Create Room"
$ Enter max players and room name
$ Click "Create"
$ Wait in the lobby for players to join, managed by UIManager.cs and PlayerRoomEntryList.cs
</code></pre>

### **Joining a Room**

<p>For players choosing to join a room, the following sequence occurs:</p>

<ul>
  <li>The player is asked to enter the room name they wish to join, or they can select from available rooms displayed in the scroll view panel.</li>
  <li>In both cases, the room name text is automatically updated, and the player clicks "Join" to enter the lobby.</li>
  <li>Once in the lobby, a text renderer script displays "Waiting for other players."</li>
</ul>

<p>Steps to join a room:</p>

<pre><code>$ Click "Join Room"
$ Enter or select the room name
$ Click "Join"
$ Enter the lobby, with UI and player entries managed by <code>UIManager.cs</code> and PlayerRoomEntryList.cs
</code></pre>

### **Ready Button and Game Start**

<p>The "Ready" button sends a trigger to update the <code>isPlayerReady</code> boolean for all clients in the room, including the master client. Every time a player enters or clicks "Ready," the <code>CheckPlayersReady</code> function from <code>PlayerRoomEntryList.cs</code></p> is called.</p>
<p>If the master client detects that all players are ready, it triggers a countdown coroutine and calls <code>PhotonNetwork.LoadLevel</code> to load the game scene.</p>

<pre><code>$ Click "Ready"
$ Master client triggers game start if all players are ready, handled by <code>PlayerRoomEntryList.cs</code> and `PhotonNetwork.LoadLevel`
</code></pre>

<!-- SCENARIO3 -->
<h2 id="scenario3"> :small_orange_diamond: Transition to the Game Scene</h2>
<p>The game start sequence is critical, as it sets up all necessary components for the gameplay, including player spawning, keycard initialization, HUD setup, and more.</p>
<p align="center"> 
<img src="./ReadMeAssets/gameScene.gif" alt="Game Scene">
</p>
Player Spawning and Initialization
<p>When the game starts, the following key actions occur:</p>
<ul>
  <li>Players are spawned into the game world, with <code>PlayerSpawner.cs</code> handling the spawning locations.</li>
  <li>The <code>PlayerStatsManager.cs</code> is initialized to ensure that each player has a container for tracking custom properties such as traps spawned, keycards collected, actor number, kills, deaths, etc.</li>
  <li>The <code>OwnerNetworkComponent.cs</code> is also initialized to ensure that cameras and game objects not owned by the local client are turned off, maintaining performance and preventing interference.</li>
</ul>
<p>Steps to initialize player spawning:</p>
<pre><code>$ Players are instantiated at spawn points
$ PlayerStatsManager.cs initializes player properties
$ OwnerNetworkComponent.cs configures client-specific objects
</code></pre>
Keycard Spawning and Room Highlighting
<p>The first keycard is spawned by <code>KeyCardManager.cs</code>, which assigns the keycard to an interactable object.</p>
<ul>
  <li>Once assigned, the interactable object triggers a function that shoots a raycast down to find the room's <code>RoomBehavior.cs</code>.</li>
  <li>If the room behavior is found, it triggers the <code>RoomHighlighter.cs</code> to display the room highlight on the minimap HUD, guiding players to the keycard.</li>
</ul>
<p>Steps to spawn and highlight the first keycard:</p>
<pre><code>$ KeyCardManager.cs spawns the first keycard
$ InteractableObject.cs assigns and triggers room highlighting
$ RoomBehavior.cs and RoomHighlighter.cs highlight the keycard's room on the minimap
</code></pre>
Minimap Initialization
<p>The <code>MinimapFollowCam.cs</code> is initialized to render game objects with the minimap layers in the HUD, ensuring players can track room locations and keycard positions.</p>
<pre><code>$ MinimapFollowCam.cs starts rendering minimap HUD elements
</code></pre>
HUD Setup
<p>The HUD is updated to display critical information:</p>
<ul>
  <li>Each player's keycard count is displayed using the <code>PlayerStatsEntry.cs</code>, which pulls data from the player's custom properties initialized in <code>PlayerStatsManager.cs</code>.</li>
  <li>Players' names and other stats are also visible, giving a clear overview of the game's progress.</li>
</ul>
<p>Steps to set up the HUD:</p>
<pre><code>$ PlayerStatsEntry.cs updates keycard count on the HUD
$ Player names and stats are displayed via HUDManager.cs
</code></pre>

<!-- SCENARIO4 -->
<h2 id="scenario4"> :small_orange_diamond: Keycard Search</h2>
<p>The primary goal of the game is for players to collect keycards hidden within lootable props scattered throughout the environment. The keycard search process is driven by player interactions with these props and triggers several game mechanics that enhance gameplay and immersion.</p>
<p align="center"> 
<img src="./ReadMeAssets/keycardSearch.gif" alt="Keycard Search GIF">
</p>
Sequence of Events
<p>When a player approaches a prop with an <code>InteractableObject.cs</code> script attached, the following sequence occurs:</p>
<ul>
  <li>The player enters the trigger volume of the interactable object (IO), activating the highlighter, and a HUD element appears, informing the player to press a specific button to search the prop.</li>
  <li>Inside the <code>InteractableObject.cs</code> script, the <code>Interact()</code> method is called when the player presses the search button. This method performs a series of checks:</li>
  <ul>
    <li>First, it checks if the <code>hasTrap</code> boolean is true. If true, the trap sequence is initiated:</li>
    <ul>
      <li>A beeping red light game object within the IO is instantiated, signaling an imminent explosion.</li>
      <li>The trap effect is triggered, controlled by <code>TrapEffect.cs</code>, which manages the explosion sequence.</li>
    </ul>
    <li>If the <code>hasTrap</code> boolean is false, the method proceeds to the next check:</li>
    <li>If the <code>hasKeycard</code> boolean is true, the keycard collection sequence begins:</li>
    <ul>
      <li>A visual effect (VFX) is rendered to indicate that a keycard has been collected.</li>
      <li>The <code>PlayerStatsManager.cs</code> updates the player's custom property for the keycard count, which automatically triggers an update to the keycard HUD.</li>
      <li>The HUD element updates the keycard highlighter and increases the keycard count.</li>
    </ul>
  </ul>
</ul>
<p>Steps to search and collect keycards:</p>
<pre><code>$ Approach a prop with InteractableObject.cs attached
$ The highlighter and HUD prompt appear
$ Press the button to search the prop
$ If a trap is present, the TrapEffect is triggered
$ If a keycard is present, it is collected and the HUD is updated
</code></pre>
Inventory Check and Final Door Activation
<p>Each time a keycard is collected, the <code>InventoryManager.cs</code> performs a callback to check if all required keycards have been gathered. If all keycards are collected, the following events are triggered:</p>
<ul>
  <li>A game event signals the <code>FinalDoorBehavior.cs</code> to change the laser doors from red to green, indicating they are now unlocked.</li>
  <li>The exit sign lights throughout the level change from green to red, signaling the available exit path.</li>
  <li>The minimap HUD listens for this game event and highlights the center room with an exit sign, guiding players to the final door.</li>
</ul>
<p>Steps after collecting all keycards:</p>
<pre><code>$ InventoryManager.cs checks if all keycards are collected
$ FinalDoorBehavior.cs changes the laser doors to green
$ Exit sign lights change color, and the minimap highlights the exit room
</code></pre>

<!-- SCENARIO5 -->
<h2 id="scenario5"> :small_orange_diamond: Trap Placement</h2>
<p>Trap placement is a strategic gameplay mechanic that allows players to hinder opponents by setting traps at key locations. The process involves several scripts that manage the selection, positioning, confirmation of trap placement, and visual feedback on the HUD.</p>
<p align="center"> 
<img src="./ReadMeAssets/trapPlacement.gif" alt="Trap Placement GIF">
</p>
Sequence of Events
<p>When a player presses keys 1-4, the <code>TrapSpawner.cs</code> script registers the selected trap and initiates the trap placement sequence:</p>
<ul>
  <li>The selected trap is rendered as a game object along with a line spawner that helps the player visualize where the trap will be placed.</li>
  <li>The corresponding trap hologram is also displayed at the end of the line spawner, indicating the potential placement location.</li>
</ul>
<p>During this sequence:</p>
<ul>
  <li>The HUD is updated via <code>HUDManager.cs</code> to highlight the selected trap, showing a small detail section that includes the trap's name, where it can be set, and tips on how to avoid it.</li>
  <li>The player's rotation, controlled by the mouse, allows repositioning of the trap hologram within the environment.</li>
</ul>
Trap Hologram Positioning
<p>The trap's hologram script plays a crucial role in determining the validity of the placement location:</p>
<ul>
  <li>The hologram script shoots a raycast with specific layers it is looking for (e.g., floors, walls, or interactable objects).</li>
  <li>If the raycast detects a valid placement area, the hologram is displayed in green; if the area is invalid, it appears gray.</li>
  <li>Some traps are limited to being set on floors, some on walls, and others inside interactable objects only.</li>
</ul>
Confirming or Cancelling Trap Placement
<p>Once the player finds a valid placement location:</p>
<ul>
  <li>The player can confirm the placement by left-clicking, which triggers the trap instantiation via <code>TrapEffect.cs</code> and <code>TrapTrigger.cs</code>.</li>
  <li>If the player decides not to place the trap, they can right-click to cancel the placement, returning to normal gameplay.</li>
</ul>
<p>Additionally:</p>
<ul>
  <li>If the player performs any movement other than directional movement—such as dashing, crouching, shoving, or jumping—the trap placement mode is automatically canceled. This is handled by the <code>Enter/ExitTrapPlacementMode</code> method within <code>TrapSpawner.cs</code>.</li>
</ul>
Post-Placement Actions
<p>If a trap is successfully placed, several actions are triggered:</p>
<ul>
  <li>The player's custom property for traps set is updated, specifying the trap type that was spawned, increasing the count of that specific trap type and the overall traps set.</li>
  <li>The game automatically exits the trap placement mode after successfully setting the trap.</li>
  <li>The trap's <code>TrapTrigger</code> script is instantiated, with specific traps inheriting from the base class <code>TrapTrigger</code> (e.g., <code>DetonatorTrap.cs</code>, <code>BouncingBetty.cs</code>, <code>CeilingTrap.cs</code>, <code>PoisonDart.cs</code>).</li>
  <li>At instantiation, the owner ID of the trap is set using <code>PhotonNetwork.LocalActor.ActorNr</code>. This ID is passed to the <code>TrapEffect</code> child classes later when the trap is triggered to identify who inflicted damage.</li>
  <li>The base class <code>TrapTrigger</code> starts a coroutine to enable the trap after a 3-second delay, during which the trap is initially disabled. This is visually demonstrated by a radial wheel filling up, showing the trap indicator above the trap for the trap owner, while making the game object invisible to the other players.</li>
  <li>Upon successful trap placement, a game event is sent to the <code>HUDManager.cs</code> to notify it of the trap type used. The <code>HUDManager</code> then runs a coroutine to simulate the visual feedback of the trap's cooldown by adjusting the image's fill on the HUD.</li>
</ul>
<p>Steps to place or cancel a trap:</p>
<pre><code>$ Press 1-4 to select a trap
$ The trap is rendered, and HUD updates with trap details via HUDManager.cs
$ Rotate the mouse to reposition the trap hologram
$ Confirm placement with left-click, or cancel with right-click or any non-directional movement
$ If placed, custom properties are updated, and TrapTrigger is instantiated
$ HUDManager.cs receives the trap type and runs a cooldown visual feedback
$ The trap becomes active after 3 seconds, indicated by a radial wheel and trap indicator
</code></pre>

<!-- SCENARIO6 -->
<h2 id="scenario6"> :small_orange_diamond: Trap Interactions with Other Game Objects</h2>
<p>Once a trap is instantiated and activated, it can interact with other game objects, primarily players and other traps. The interaction sequence is managed through several base class functions and RPCs to ensure synchronization across all clients.</p>
<p align="center"> 
<img src="./ReadMeAssets/trapInteractions.gif" alt="Trap Interactions GIF">
</p>
Trap Trigger Activation
<p>When a trap is instantiated and activated, it can interact with two types of objects: players and other traps.</p>
<p>The interaction process begins when a player enters the trigger box of a trap:</p>
<ul>
  <li>The base class function <code>HandleTrapTrigger()</code> is called, but only the master client manages this function since all instantiated traps are owned by the master client.</li>
  <li><code>HandleTrapTrigger()</code> initiates a coroutine that spawns the pre-effect for the trap, which is a unique effect based on the trap type. This pre-effect is synchronized across all clients via an RPC call:</li>
  <ul>
    <li>For the <code>PoisonDart</code>, this could be a dart shooting animation.</li>
    <li>For the <code>BouncingBetty</code>, it could be an animation of the mine springing up and rotating.</li>
    <li>For the <code>DetonatorTrap</code>, it could be a beeping sound and visual light effect signaling an imminent explosion.</li>
  </ul>
  <li>Once the pre-effect coroutine is completed, another coroutine is triggered to spawn the actual effect that can inflict damage on a player.</li>
</ul>
Trap Effect and Player Interaction
<p>The spawned effect has a component that inherits from the <code>TrapEffect</code> base class, which handles the following:</p>
<ul>
  <li>It initializes the <code>trapOwnerId</code> by passing the owner ID from the trap trigger.</li>
  <li>It includes a base <code>OnTriggerEnter()</code> or <code>OnCollisionEnter()</code> function that checks upon collision if the object that made contact has the layer "Player". If not, the function returns.</li>
  <li>If a player is detected, the following sequence occurs:</li>
  <ul>
    <li>The <code>Death()</code> function in <code>CustomCharacterMovement.cs</code> is called to simulate the player's death.</li>
    <li>An RPC is sent to all clients to trigger the specific character's <code>PerformDeath()</code> function, which handles the death animation and related effects:</li>
    <ul>
      <li><code>PerformDeath()</code> first checks if <code>isDead</code> is true to ensure the death sequence is only triggered once. If false, it sets <code>isDead</code> to true.</li>
      <li>The function updates the player's death totals and death by specific trap type in <code>PlayerStatsManager.cs</code>.</li>
      <li>The player's ragdoll is activated to animate the death sequence.</li>
      <li>If the player has more than 0 keycards, a call to <code>KeycardManager.cs</code> is made to send an RPC targeting all clients to spawn a physical keycard in the world space at the player's position.</li>
      <li>The spawned keycard, which has a <code>Keycard.cs</code> component, is animated to float and hover, making it easily visible to other players.</li>
      <li>The keycard also includes an <code>OnTriggerEnter()</code> function that, when collected by a player, updates their keycard count, similar to when keycards are collected from props.</li>
    </ul>
    <li>A visual and sound effect (VFX and SFX) is spawned to notify other players of the death.</li>
    <li>The function then calls <code>SendKillUpdate()</code>, passing the player game object as a parameter:</li>
    <ul>
      <li>It retrieves the <code>photonViewID.OwnerActorNr</code> of the victim and packages this information with the <code>trapOwnerId</code> and <code>trapType</code>.</li>
      <li>This data is sent to <code>PlayerStatsManager.cs</code> to parse and update the custom properties of the player for kill totals and specific trap type kills.</li>
      <li>The <code>PlayerStatsManager</code> also triggers a callback to <code>KillFeed.cs</code> to update the HUD, displaying who killed who, and with which trap.</li>
      <li>The <code>PlayerStatsManager</code> has a callback to <code>CloudSave.cs</code> to update the specific kill and specific trap type kills as well.</li>
    </ul>
    <li>After the kill update, <code>PlayerSpawner.cs</code> updates the player character's transform position to one of the registered spawn points, effectively respawning the player.</li>
  </ul>
</ul>
<p>Steps for trap interaction and player death:</p>
<pre><code>$ Player enters the trap trigger box, HandleTrapTrigger() is called by the master client
$ Pre-effect is spawned and synchronized via RPC
$ TrapEffect component checks for player collision, triggering the Death() function in CustomCharacterMovement.cs
$ PerformDeath() handles death animation, keycard drop, and updates PlayerStatsManager.cs
$ SendKillUpdate() updates player stats, triggers KillFeed.cs for HUD update, and syncs with CloudSave.cs
$ PlayerSpawner.cs respawns the player at a new spawn point
</code></pre>

<!-- SCENARIO7 -->
<h2 id="scenario7"> :small_orange_diamond: Winning, Losing, and Game Over</h2>
<p>This scenario covers the final stages of the game, from determining the winning player to displaying the final stats. It involves several scripts that manage the win/loss conditions, trigger the end-of-game sequence, and display detailed player statistics on the end game screen.</p>
<p align="center"> 
<img src="./ReadMeAssets/gameOver.gif" alt="Game Over GIF">
</p>
Winning and Losing Conditions
<p>The win/loss conditions are managed primarily through the following scripts:</p>
<ul>
  <li><code>GameManager.cs</code>: Manages the overall game flow, including the conditions under which the game ends.</li>
  <li><code>TimerManager.cs</code>: Tracks the game timer, determining if the game ends due to time running out.</li>
  <li><code>WorldButtonInteractable.cs</code>: Attached to the winning button, this script handles player interactions to trigger the win condition. The script uses the <code>Interact()</code> method to raise a game event when the win button is pressed, signaling the end of the game.</li>
  <li><code>HUDManager.cs</code>: Updates the HUD to reflect the game's end status, including which player won.</li>
</ul>
<p>The sequence begins when a player has gone through the final doors and interacts with the winning button:</p>
<ul>
  <li>Upon pressing the win button, the <code>WorldButtonInteractable.cs</code> script triggers the <code>onWinButtonPressed</code> event, which notifies the <code>GameManager.cs</code> to end the game.</li>
  <li>If the game ends due to time running out, <code>TimerManager.cs</code> sends a notification to the <code>GameManager.cs</code>, which then determines the winning player based on the game’s criteria (e.g., most keycards collected).</li>
  <li><code>HUDManager.cs</code> is updated to reflect the end-game state, showing a banner or message indicating the winning player or team.</li>
</ul>
<p>Steps to determine win/loss:</p>
<pre><code>$ Player interacts with the win button via WorldButtonInteractable.cs
$ GameManager.cs handles the win condition and ends the game
$ HUDManager.cs updates the HUD with the game’s final status
</code></pre>
Game Over and Stats Display
<p>After the win/loss conditions are met, the game transitions to the end game screen, displaying detailed player stats:</p>
<ul>
  <li><code>UIEndGame.cs</code>: Manages the display of data on the end game screen, including player kills, deaths, traps set, and MVP status.</li>
  <li><code>PlayerStatsManager.cs</code>: Provides the statistical data for each player, which is then formatted and displayed by <code>UIEndGame.cs</code>.</li>
  <li><code>LocalPlayerStatEntry.cs</code>: Displays the local player's detailed stats, including total kills, deaths, traps set, and MVP status.</li>
  <li><code>OtherPlayerStatEntry.cs</code>: Displays the stats for other players, similar to the local player but with slightly less emphasis.</li>
</ul>
<p>The process for displaying stats is as follows:</p>
<ul>
  <li><code>UIEndGame.cs</code> triggers the <code>InitEndGameScreenData()</code> method, which retrieves all players' stats from <code>PlayerStatsManager.cs</code> and prepares them for display.</li>
  <li>The <code>DisplayLocalPlayerStats()</code> method populates the local player's stat card with their performance data, including kills, deaths, traps set, and their MVP trap (if applicable).</li>
  <li>Other players' stats are populated in similar fashion using the <code>OtherPlayerStatEntry.cs</code> script.</li>
  <li>The <code>GameManager.cs</code> determines the winner based on the criteria set (e.g., most keycards, kills, etc.), and this information is displayed prominently on the end game screen.</li>
  <li>The <code>UIEndGame.cs</code> script also animates the end game banner and displays additional accolades, such as the "Top Agent" or "Ex-Employee" status, depending on the outcome.</li>
</ul>
<p>Steps to display game over stats:</p>
<pre><code>$ UIEndGame.cs initializes the end game screen data via InitEndGameScreenData()
$ PlayerStatsManager.cs provides the necessary player statistics
$ LocalPlayerStatEntry.cs and OtherPlayerStatEntry.cs populate the player stat cards
$ GameManager.cs determines the winner, and UIEndGame.cs displays this on the screen
$ UIEndGame.cs animates the end game screen and displays final accolades
</code></pre>

<h2>Contributing</h2>
<p>Fork the repository and submit a pull request with your improvements or bug fixes.</p>

<h2>License</h2>
<p>This project is licensed under the <a href="LICENSE">MIT License</a>.</p>

<h2>Screenshots</h2>
<p align="center">
  <img src="screenshot.png" alt="Gameplay Screenshot" width="600">
</p>

<h2>Contact</h2>
<p>
  For any questions, reach out to <a href="mailto:youremail@example.com">Your Name</a> or visit <a href="https://linkedin.com/yourprofile">Your LinkedIn</a>.
</p>


    
