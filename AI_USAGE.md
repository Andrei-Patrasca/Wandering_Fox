# AI Usage Disclosure

## Tools used

- **Claude Sonnet 4.6** (Anthropic) — chat-based code suggestions and architecture guidance

## How I used it

I used Claude in a conversational way to help me learn SDL2 and .NET game development
from scratch. The workflow was:

- I described what I wanted to build and asked questions about concepts I didn't understand
- Claude suggested code which I then read, understood, and typed/pasted manually
- I made my own decisions on level design, game mechanics, and what features to include
- I debugged errors myself with Claude's help explaining what was wrong and why
- I redesigned all three levels myself using the tileset reference image

## Files with significant AI assistance

The following files were written with heavy AI guidance. I have read and can explain
every line:

- `Engine/Renderer.cs` — SDL2 texture loading via P/Invoke
- `Engine/HUD.cs` — pixel digit rendering system
- `Engine/InputHandler.cs` — scancode-based input handling
- `Models/SpriteSheet.cs` — spritesheet frame animation logic
- `Models/Portal.cs` — animated portal spritesheet logic
- `Program.cs` — main game loop structure and end screen rendering

## Files I wrote myself / heavily modified

- `Levels/LevelData.cs` — all three level layouts designed by me
- `Models/PlayerObject.cs` — direction mapping fixed and tuned by me
- `Engine/GameState.cs` — straightforward, written with minor suggestions
- `Engine/ScoreBoard.cs` — straightforward file I/O, minor suggestions

## AI-generated asset disclosure

- `Assets/Fox/` — fox sprites sourced online (not AI generated)
- `Assets/Tiles/Map_tiles.png` — tileset sourced online
- `Assets/Items/portal.bmp`, `mushroom.bmp` — sourced online
- `Assets/UI/heart.bmp` — sourced online

No C# source code was copied blindly. All AI suggestions were reviewed,
understood, and often modified before use.