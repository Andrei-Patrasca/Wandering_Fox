using TheAdventure.Engine;

namespace TheAdventure.Models;

public static unsafe class SpriteSheetFactory
{
    private const int FoxFrameSize = 32;

    public static SpriteSheet CreateIdle(Renderer renderer)
    {
        var tex = renderer.LoadTexture("Assets/Fox/Fox_Idle_with_shadow.bmp");
        return new SpriteSheet(renderer, tex, FoxFrameSize, FoxFrameSize, 4)
        { FrameDurationMs = 150 };
    }

    public static SpriteSheet CreateWalk(Renderer renderer)
    {
        var tex = renderer.LoadTexture("Assets/Fox/Fox_walk_with_shadow.bmp");
        return new SpriteSheet(renderer, tex, FoxFrameSize, FoxFrameSize, 6)
        { FrameDurationMs = 120 };
    }

    public static SpriteSheet CreateRun(Renderer renderer)
    {
        var tex = renderer.LoadTexture("Assets/Fox/Fox_Run_with_shadow.bmp");
        return new SpriteSheet(renderer, tex, FoxFrameSize, FoxFrameSize, 6)
        { FrameDurationMs = 80 };
    }

    public static SpriteSheet CreateDeath(Renderer renderer)
    {
        var tex = renderer.LoadTexture("Assets/Fox/Fox_Death_with_shadow.bmp");
        return new SpriteSheet(renderer, tex, FoxFrameSize, FoxFrameSize, 6)
        { FrameDurationMs = 100 };
    }
}