# Prida
<img width="939" height="528" alt="image" src="https://github.com/user-attachments/assets/2346d2c8-c911-4fb5-9295-9b1ddefee970" />

**Prida** is a cross-platform 2D platformer game developed in Unity as part of a bachelor's graduation project.

The project is a playable prototype for **Windows** and **Android**. It includes story levels, enemies, bosses, puzzles, weapons, ultimate abilities, checkpoints, progress saving, a challenge mode, New Game+, and platform-specific controls.

## Features

* Story Mode with multiple levels
* Challenge Mode with enemy waves
* New Game+ unlocked after completing the game
* Main menu with Story Mode, Challenge Mode, Options, and Exit
* Difficulty selection before starting the story mode
* Player movement, jumping, double jump, dash, attack, parry, interaction, and ledge climbing
* Keyboard controls for Windows
* Touch controls for Android
* Interaction with objects using the `E` key or a mobile interaction button
* Four weapon types: Sword, Heavy Sword, Bow, and Spear
* Ultimate abilities, including time rewind
* Normal and rare chests
* Melee enemies, ranged enemies, flying enemies, mini-bosses, and a final boss
* Checkpoint system
* Level puzzles: code door, torches, boxes, emblems, beam, and crystal
* Android gyroscope-based gameplay area
* Two different endings depending on the final player choice
* Volume settings in the Options menu
  
<img width="530" height="494" alt="image" src="https://github.com/user-attachments/assets/68c0caf7-d936-47fc-979f-5a9bb259737f" />

## Technologies

* Unity 2022.3.62f3
* C#
* Visual Studio 2022
* Unity UI
* Rigidbody2D
* Collider2D
* Animator
* Windows
* Android
  
<img width="960" height="457" alt="image" src="https://github.com/user-attachments/assets/756248f6-c564-4533-abe4-3a5b5ba99b0f" />

## Project Structure

```text
Assets/           Game scenes, scripts, sprites, prefabs, UI, animations, and resources
Packages/         Unity package dependencies
ProjectSettings/  Unity project settings
diplomchik.exe    Windows executable build
```

## How to Open the Project in Unity

1. Install **Unity 2022.3.62f3**.
2. Download or clone this repository.
3. Open Unity Hub.
4. Click **Add project from disk**.
5. Select the project folder.
6. Wait until Unity finishes importing assets.
7. Open the `MainMenu` scene.
8. Press **Play** to start the game in the Unity Editor.

## How to Run the Windows Build

To launch the ready Windows version, run:

```text
diplomchik.exe
```

## Controls

### Windows

```text
A / D        Move left / right
Space        Jump
Left Shift   Dash
J / LMB      Attack
Z            Parry
E            Interact
X            Climb ledge
```

### Android

The Android version uses on-screen buttons:

```text
Movement     Move character
Jump         Jump
Dash         Dash
Attack       Attack
Parry        Parry
Interact     Interact with objects
Ultimate     Use ultimate ability
```

## Game Modes

### Story Mode

The main game mode. The player goes through story levels, fights enemies and bosses, solves puzzles, collects rewards, and reaches the final boss.

### Challenge Mode

A separate mode where the player fights waves of enemies and tests combat mechanics outside the main story.

### New Game+

New Game+ becomes available after completing the game. In this mode, the player starts a new playthrough with unlocked weapons and ultimate abilities.

## Gameplay Systems

### Player

The player character supports movement, jumping, double jumping, dashing, attacking, parrying, interacting with objects, and climbing ledges.

### Combat

The combat system includes melee attacks, ranged attacks, dash evasion, parry, and ultimate abilities. Different weapons have different damage, range, and cooldown values.

### Weapons

The game includes four weapon types:

```text
Sword
Heavy Sword
Bow
Spear
```

### Ultimate Abilities

The project includes several ultimate abilities. One of the key abilities is time rewind, which saves previous player positions and returns the character to an earlier state when activated.

### Enemies and Bosses

The game includes several enemy types:

```text
Melee enemies
Ranged enemies
Flying enemies
Mini-bosses
Final boss
```

The final boss has several attack types: melee attack, projectile attack, dash attack, area attack, and a second stage.

### Puzzles

The game contains different puzzle mechanics:

```text
Code door
Torch puzzle
Two-box puzzle
Three-emblem puzzle
Beam and crystal puzzle
Gyroscope-based Android puzzle
```

### Checkpoints

The checkpoint system saves the player's respawn position. After death, the player returns to the last activated checkpoint instead of restarting the whole level.

## Cross-Platform Support

The project uses one shared codebase for Windows and Android.
The main gameplay logic remains the same, while the input method changes depending on the platform:

```text
Windows  -> keyboard and mouse input
Android  -> touch buttons and gyroscope input
```

## Implementation Highlights

* Modular script structure
* Scene loading through `SceneManager`
* Player input support for both Windows and Android
* Combat system with melee and ranged attacks
* Boss behavior based on multiple attack routines
* Ultimate ability based on saving and restoring previous player states
* Visual warning before the flying mini-boss beam attack
* Progress saving for New Game+
* UI implemented with Unity Canvas and Canvas Scaler
* Volume control through the Options menu

## Status

The project is a working prototype of a cross-platform 2D platformer game. It can be further developed by adding more levels, enemies, animations, sound effects, visual effects, balance improvements, and mobile optimization.



## License

This project is intended for educational and demonstration purposes.
