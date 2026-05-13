# The Adventure

A top-down 2D game built with .NET 10 and SDL2 (via Silk.NET).

## How to run
-open terminal in the project folder and run:
dotnet build
dotnet run

## Controls

| Key | Action |
|-----|--------|
| WASD / Arrow keys | Move fox |
| R | Restart current level |

## Gameplay

- Collect mushrooms for 100 points each
- Avoid water and lava — costs a life
- Find the portal to advance to the next level
- 3 lives total — lose them all and it's Game Over
- Beat all 3 levels to win
- High score is saved to `highscore.txt` between runs

## Project structure

| Folder | Contents |
|--------|----------|
| `Engine/` | Renderer, InputHandler, HUD, GameState, ScoreBoard |
| `Models/` | PlayerObject, SpriteSheet, Portal, Collectible, IGameObject |
| `Levels/` | Level, LevelData |
| `Assets/` | Sprites and tiles |

## Tech used

- .NET 10
- Silk.NET.SDL (SDL2 bindings)
- No game engine