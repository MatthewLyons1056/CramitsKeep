# CramitsKeep
Repository featuring some highlight scripts from the steam release "Cramits Keep"

AIDestinationSetter - controls A* pathfinding by assigning targets and creating conditions for targets to be assigned.

DontDestroyOnLoad - Does not delete attached objects on new scene. Creates unique object ID by converting the object name + position to a string, then compares the string the new objects in the scene. If the string matches, the new object is deleted. This is to keep player progression, and other variables that are needed to be global/

EnemyAI - Main class for referncing enemy logic 

Melee - a tool for adding melee combat functions to enemies (swings, charge attacks, dash attacks) Includes animations and audio controls

Ranged - a tool for adding ranged combat funtions to enemies (Projectiles, Barrage attacks) Includes animations and audio controls

LevelManager - controls level spawning, room transitions, and how powerful enemies scale in each room

MainSceneManager - Controls scene transitions, start and pause functionality

Load Progress - Simple script to preload the game before it "starts"

