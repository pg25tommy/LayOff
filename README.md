# Lay-Off

### High Concept

'Lay-Off' is a fast paced 4-player trap placement deathmatch spy showdown! As an underfunded agency's spy, outmaneuver rivals in a race for keycards and survival. Look for keycards inside furniture and fixtures, and earn your job security within the given time. Tick-tock goes the clock - enter retirement in this life or the after. Your mission: be the first spy to get out.

### Key Features
- 💳 Loot furniture inside the rooms and find all required keycards.
- 🪤 Place traps to incapacitate opponents.
- 🏃‍♂️ Move from room to room and evade tripped traps.
- 🚪 Open the final door  with the collected required keycards.
- ⌛ Be the first spy to get out and keep your job before the timer runs out!

### Documents

####[Game Design Document](<https://docs.google.com/document/d/1EOtq6T-koMIXg-mn5ZNDBZTEzfKXDTuH26YQo7-KJ64/edit>)
####[Level Design Document](<https://docs.google.com/document/d/1KrELT-K40ONWjuHZs2KrtfROgE5f_UMldYJYqJhocVI/edit>)
####[Level Design Factors](<https://docs.google.com/document/d/17cBEJz9dr2wRV9rXXayRy6X6Ra9Vw-dgV825XXZ7PW4/edit?usp=sharing>)
####[Media Design Document](<https://miro.com/app/board/uXjVKbGBaEg=/>)
####[Playtest Feedback Form Sheet](<https://docs.google.com/spreadsheets/d/1Xj5_FCRPDRVBHvtLIURjiByfMTiMb0mH42zKIil7grU/edit?resourcekey#gid=1846697056>)

#### Technical Design
- [Milestone Reports Folder](<https://drive.google.com/drive/folders/150YZfvwgw_gTwuQ0dgpr1MvcCDczaLGk?usp=drive_link>)
- [Jira Link](<https://namesarehard.atlassian.net/jira/software/c/projects/NAH/boards/1>)
- [Google Sheets Task List](<https://docs.google.com/spreadsheets/d/1uVZOimqBQL-o7suPPyFsY1GhRVTNn6KUdIc9QktfJSg/edit#gid=1293827372>)
- [Art Asset List](<https://docs.google.com/spreadsheets/d/1uVZOimqBQL-o7suPPyFsY1GhRVTNn6KUdIc9QktfJSg/edit#gid=2032915027>)
- [Audio Asset List](<https://docs.google.com/spreadsheets/d/1uVZOimqBQL-o7suPPyFsY1GhRVTNn6KUdIc9QktfJSg/edit#gid=1887848605>)
- [Feature Matrix](<https://docs.google.com/spreadsheets/d/17C0XLlSLXusYvt8Uwhqp3laTsKmNO_tYWkpz9P3lyqQ/edit#gid=0>)
- [Core Pillars Infographic](<https://miro.com/app/board/uXjVKfU_F2c=/>)
- [Bug Tracker Jira](<https://namesarehard.atlassian.net/jira/software/c/projects/NAH/boards/2>)
- [Builds Link](<file://vfsstorage10/dropbox/GDPGSD/Builds/GD72PG25/LayOff/>)

### Controls

Action               | Keyboard Control  | Gamepad Control
---                  |---                |---
Movement             | WASD              | 
Rotate View          | Mouse position    |
Jump                 | Space             | 
Crouch               | Ctrl / C          |
Dash                 | L-Shift           |
Use Trap             | Num 1 - 4         |
Change Trap Position | Drag Mouse Up/Down|
Interact / Shove     | E                 | 
Shove                | F                 | 
Options Menu         | Esc               |

### Debug tools/controls

press 0 to open the menu
Add Keycard to player 
Spawn Dummy 
Spawn Keycard 
Destroy All Traps
FPS 
Player world position

### Known Issues

* Caveats:
    - Traps:
        - Placing traps on top of existing traps placed should trigger the existing trap and successfully plant the new trap
        - Trap hologram locks and collides with room behavior trigger volume
    - UI/HUD:
        - Timer does not stop after a player wins the game
        - Bullied, Nemesis, and MVP trap not populating accurate data.      
    - Character:
        - Requires polishing
    - Environment:
        - Interactable object highlighter 2's cancel highlighting when another player enters the area
        - Intermittent issues - cannot interact with interactable object 
    - Audio:
        - some SFX does not play for all clients 

### Team:

* Alex Buzmion - Programmer/Project Manager
* Bryan Wu - 3D Character Artist/Charter Guardian
* Chris Conchada - Level Designer/ Environment Artist
* Eduardo Cabello - Programmer/Build Master
* Tiago Corsato - Programmer/Environment Artist/VFX Particle Artist & POC
* Tommy Minter - Programmer/ Shader Programmer


    
