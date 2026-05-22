using System.Diagnostics;
using Silk.NET.SDL;
using Silk.NET.Maths;
using TheAdventure.Engine;
using TheAdventure.Models;
using TheAdventure.Levels;

namespace TheAdventure;

using GameRenderer = TheAdventure.Engine.Renderer;
using SdlRenderer  = Silk.NET.SDL.Renderer;

public static class Program
{
    public static void Main()
    {
        var scoreBoard = new ScoreBoard();
        var sdl = new Sdl(new SdlContext());
        sdl.Init(Sdl.InitVideo | Sdl.InitEvents | Sdl.InitTimer);

        unsafe
        {
            var window = sdl.CreateWindow("The Adventure",
                Sdl.WindowposUndefined, Sdl.WindowposUndefined, 800, 600,
                (uint)(WindowFlags.Resizable | WindowFlags.AllowHighdpi));

            var sdlRenderer = sdl.CreateRenderer(window, -1,
                (uint)(RendererFlags.Accelerated | RendererFlags.Presentvsync));

            var renderer  = new GameRenderer(sdl, sdlRenderer);
            var input     = new InputHandler();
            var gameState = new GameState();
            var hud       = new HUD(sdl, sdlRenderer, renderer);

            // Build levels: (tiles, portalCol, portalRow, spawnCol, spawnRow, mushrooms)
            var levels = new Level[]
            {
                new Level(renderer, LevelData.Level1,
                    portalCol: 10, portalRow: 3,
                    spawnCol: 1,  spawnRow: 1,
                    mushrooms: new() { (3,1),(7,2),(2,6),(6,4) }),

                new Level(renderer, LevelData.Level2,
                    portalCol: 10, portalRow: 6,
                    spawnCol: 1,  spawnRow: 1,
                    mushrooms: new() { (5,3),(6,3),(2,2),(9,2),(4,7) }),

                new Level(renderer, LevelData.Level3,
                    portalCol: 10, portalRow: 7,
                    spawnCol: 1,  spawnRow: 1,
                    mushrooms: new() { (5,1),(6,6),(2,3),(9,7),(5,4) }),

                new Level(renderer, LevelData.Level4,
                    portalCol: 1, portalRow: 9, // Portal safe at the bottom-left map finish line
                    spawnCol: 1,  spawnRow: 1,  // Starts player safely at the top-left corner
                    mushrooms: new() { (3,2), (11,1), (9,6), (2,5) }) // Hidden along safe stone corridors
            };

            int   currentLevel = 0;
            var   player       = new PlayerObject(renderer,
                                     levels[0].SpawnX, levels[0].SpawnY);

            var   timer = new Stopwatch();
            timer.Start();
            var   ev   = new Event();
            bool quit = false;

            while (!quit)
            {
                // ── Event polling ──────────────────────────────────────────
                while (sdl.PollEvent(ref ev) != 0)
                {
                    switch ((EventType)ev.Type)
                    {
                        case EventType.Quit:
                            quit = true;
                            break;
                        case EventType.Keydown:
                            input.KeyDown((int)ev.Key.Keysym.Scancode);

                            if (ev.Key.Keysym.Scancode == Scancode.ScancodeR)
                            {
                                player.SetSpawn(levels[currentLevel].SpawnX,
                                                levels[currentLevel].SpawnY);
                                player.Respawn();
                            }

                            // On Game Over or Win screen — any key quits
                            if (gameState.GameOver || gameState.Won)
                                quit = true;
                            break;
                        case EventType.Keyup:
                            input.KeyUp((int)ev.Key.Keysym.Scancode);
                            break;
                    }
                }

                var deltaMs = timer.Elapsed.TotalMilliseconds;
                timer.Restart();

                // ── Game Over / Won screens ────────────────────────────────
                if (gameState.GameOver || gameState.Won)
                {
                    scoreBoard.Save(gameState.Score);
                    renderer.Clear(0, 0, 0);
                    DrawEndScreen(sdl, sdlRenderer, gameState, scoreBoard.HighScore);
                    renderer.Present();
                    continue;
                }

                // ── Update ─────────────────────────────────────────────────
                var level = levels[currentLevel];
                level.Update(deltaMs);
                player.Update(deltaMs, input);

                // Hazard check
                var tileType = level.GetTileAt(
                    player.X + player.Size / 2,
                    player.Y + player.Size / 2);

                if (level.IsHazard(tileType) && !player.IsDead)
                {
                    player.Kill();
                    gameState.LoseLife();
                    if (gameState.GameOver) continue;
                }

                // Mushroom collect
                foreach (var collectible in level.Collectibles)
                {
                    if (collectible.CheckCollect(player.X, player.Y, player.Size))
                        gameState.AddScore(collectible.Points);
                }

                // Portal check
                if (!player.IsDead &&
                    level.Portal.PlayerOnPortal(player.X, player.Y, player.Size))
                {
                    if (currentLevel < levels.Length - 1)
                    {
                        currentLevel++;
                        gameState.NextLevel();
                        player.SetSpawn(levels[currentLevel].SpawnX,
                                        levels[currentLevel].SpawnY);
                        player.Respawn();
                    }
                    else
                    {
                        gameState.SetWon();
                    }
                }

                // ── Render ─────────────────────────────────────────────────
                renderer.Clear(0, 0, 0);
                level.Draw();
                player.Draw();
                hud.Draw(gameState.Lives, gameState.Score, 800);
                renderer.Present();
            }

            renderer.Dispose();
            sdl.DestroyRenderer(sdlRenderer);
            sdl.DestroyWindow(window);
        }

        sdl.Quit();
    }
// AI-generated
    private static unsafe void DrawEndScreen(Sdl sdl, SdlRenderer* sdlRenderer, GameState state, int highScore)
    {
        var overlay = new Rectangle<int>(0, 0, 800, 600);
        sdl.SetRenderDrawColor(sdlRenderer, 0, 0, 0, 220);
        sdl.RenderFillRect(sdlRenderer, ref overlay);

        if (state.Won)
            DrawBigText(sdl, sdlRenderer, "YOU WON!", 800/2 - 120, 140, 255, 215, 0);
        else
            DrawBigText(sdl, sdlRenderer, "YOU DIED", 800/2 - 120, 140, 200, 0, 0);

        DrawBigText(sdl, sdlRenderer, "SCORE",     800/2 - 120, 220, 255, 255, 255);
        DrawBigText(sdl, sdlRenderer, state.Score.ToString("D5"), 800/2 - 60, 265, 255, 215, 0);

        DrawBigText(sdl, sdlRenderer, "BEST",      800/2 - 120, 320, 255, 255, 255);
        DrawBigText(sdl, sdlRenderer, highScore.ToString("D5"),  800/2 - 60, 365, 0, 255, 100);

        DrawSmallText(sdl, sdlRenderer, "press any key to quit", 800/2 - 110, 440, 150, 150, 150);
    }

    

    private static readonly Dictionary<char, bool[,]> Letters = BuildFont();

    private static unsafe void DrawBigText(Sdl sdl, SdlRenderer* r,
        string text, int x, int y, byte red, byte green, byte blue)
    {
        int scale = 5;
        int cx = x;
        foreach (char c in text.ToUpper())
        {
            if (c == ' ') { cx += scale * 4; continue; }
            if (!Letters.TryGetValue(c, out var glyph)) { cx += scale * 4; continue; }
            for (int row = 0; row < 5; row++)
            for (int col = 0; col < 3; col++)
            {
                if (!glyph[row, col]) continue;
                var rect = new Rectangle<int>(cx + col*scale, y + row*scale, scale, scale);
                sdl.SetRenderDrawColor(r, red, green, blue, 255);
                sdl.RenderFillRect(r, ref rect);
            }
            cx += scale * 4;
        }
    }

    private static unsafe void DrawSmallText(Sdl sdl, SdlRenderer* r,
        string text, int x, int y, byte red, byte green, byte blue)
    {
        int scale = 2;
        int cx = x;
        foreach (char c in text.ToUpper())
        {
            if (c == ' ') { cx += scale * 4; continue; }
            if (!Letters.TryGetValue(c, out var glyph)) { cx += scale * 4; continue; }
            for (int row = 0; row < 5; row++)
            for (int col = 0; col < 3; col++)
            {
                if (!glyph[row, col]) continue;
                var rect = new Rectangle<int>(cx + col*scale, y + row*scale, scale, scale);
                sdl.SetRenderDrawColor(r, red, green, blue, 255);
                sdl.RenderFillRect(r, ref rect);
            }
            cx += scale * 4;
        }
    }

// AI-generated
    private static Dictionary<char, bool[,]> BuildFont() => new()
    {
        ['A'] = new bool[,]{{false,true,false},{true,false,true},{true,true,true},{true,false,true},{true,false,true}},
        ['B'] = new bool[,]{{true,true,false},{true,false,true},{true,true,false},{true,false,true},{true,true,false}},
        ['C'] = new bool[,]{{false,true,true},{true,false,false},{true,false,false},{true,false,false},{false,true,true}},
        ['D'] = new bool[,]{{true,true,false},{true,false,true},{true,false,true},{true,false,true},{true,true,false}},
        ['E'] = new bool[,]{{true,true,true},{true,false,false},{true,true,false},{true,false,false},{true,true,true}},
        ['F'] = new bool[,]{{true,true,true},{true,false,false},{true,true,false},{true,false,false},{true,false,false}},
        ['G'] = new bool[,]{{false,true,true},{true,false,false},{true,false,true},{true,false,true},{false,true,true}},
        ['H'] = new bool[,]{{true,false,true},{true,false,true},{true,true,true},{true,false,true},{true,false,true}},
        ['I'] = new bool[,]{{true,true,true},{false,true,false},{false,true,false},{false,true,false},{true,true,true}},
        ['K'] = new bool[,]{{true,false,true},{true,false,true},{true,true,false},{true,false,true},{true,false,true}},
        ['L'] = new bool[,]{{true,false,false},{true,false,false},{true,false,false},{true,false,false},{true,true,true}},
        ['N'] = new bool[,]{{true,false,true},{true,true,true},{true,true,true},{true,false,true},{true,false,true}},
        ['O'] = new bool[,]{{false,true,false},{true,false,true},{true,false,true},{true,false,true},{false,true,false}},
        ['P'] = new bool[,]{{true,true,false},{true,false,true},{true,true,false},{true,false,false},{true,false,false}},
        ['R'] = new bool[,]{{true,true,false},{true,false,true},{true,true,false},{true,false,true},{true,false,true}},
        ['S'] = new bool[,]{{false,true,true},{true,false,false},{false,true,false},{false,false,true},{true,true,false}},
        ['T'] = new bool[,]{{true,true,true},{false,true,false},{false,true,false},{false,true,false},{false,true,false}},
        ['U'] = new bool[,]{{true,false,true},{true,false,true},{true,false,true},{true,false,true},{false,true,false}},
        ['W'] = new bool[,]{{true,false,true},{true,false,true},{true,true,true},{true,true,true},{true,false,true}},
        ['Y'] = new bool[,]{{true,false,true},{true,false,true},{false,true,false},{false,true,false},{false,true,false}},
        ['0'] = new bool[,]{{true,true,true},{true,false,true},{true,false,true},{true,false,true},{true,true,true}},
        ['1'] = new bool[,]{{false,true,false},{false,true,false},{false,true,false},{false,true,false},{false,true,false}},
        ['2'] = new bool[,]{{true,true,true},{false,false,true},{true,true,true},{true,false,false},{true,true,true}},
        ['3'] = new bool[,]{{true,true,true},{false,false,true},{true,true,true},{false,false,true},{true,true,true}},
        ['4'] = new bool[,]{{true,false,true},{true,false,true},{true,true,true},{false,false,true},{false,false,true}},
        ['5'] = new bool[,]{{true,true,true},{true,false,false},{true,true,true},{false,false,true},{true,true,true}},
        ['6'] = new bool[,]{{true,true,true},{true,false,false},{true,true,true},{true,false,true},{true,true,true}},
        ['7'] = new bool[,]{{true,true,true},{false,false,true},{false,false,true},{false,false,true},{false,false,true}},
        ['8'] = new bool[,]{{true,true,true},{true,false,true},{true,true,true},{true,false,true},{true,true,true}},
        ['9'] = new bool[,]{{true,true,true},{true,false,true},{true,true,true},{false,false,true},{true,true,true}},
        [':'] = new bool[,]{{false,false,false},{false,true,false},{false,false,false},{false,true,false},{false,false,false}},
        ['!'] = new bool[,]{{false,true,false},{false,true,false},{false,true,false},{false,false,false},{false,true,false}},
    };
}
//AI-generated stop