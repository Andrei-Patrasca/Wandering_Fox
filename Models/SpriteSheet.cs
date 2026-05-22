// AI-generated
using TheAdventure.Engine;

namespace TheAdventure.Models;

public enum FacingDirection { Down = 0, Left = 1, Right = 2, Up = 3 }

public unsafe class SpriteSheet
{
    private readonly Renderer _renderer;
    private readonly Silk.NET.SDL.Texture* _texture;

    public int FrameWidth  { get; }
    public int FrameHeight { get; }
    public int FrameCount  { get; }

    private int    _currentFrame = 0;
    private double _frameTimer   = 0;

    public double         FrameDurationMs { get; set; } = 120;
    public FacingDirection Direction       { get; set; } = FacingDirection.Down;

    public SpriteSheet(Renderer renderer, Silk.NET.SDL.Texture* texture,
                       int frameWidth, int frameHeight, int frameCount)
    {
        _renderer   = renderer;
        _texture    = texture;
        FrameWidth  = frameWidth;
        FrameHeight = frameHeight;
        FrameCount  = frameCount;
    }

    public void Update(double deltaMs)
    {
        _frameTimer += deltaMs;
        if (_frameTimer >= FrameDurationMs)
        {
            _frameTimer  -= FrameDurationMs;
            _currentFrame = (_currentFrame + 1) % FrameCount;
        }
    }

    public void Reset()
    {
        _currentFrame = 0;
        _frameTimer   = 0;
    }

    public bool IsFinished => _currentFrame == FrameCount - 1;

    public void Draw(int screenX, int screenY, int destSize = 64)
    {
        int srcX = _currentFrame * FrameWidth;
        int srcY = (int)Direction * FrameHeight;

        _renderer.DrawTexture(_texture,
            srcX, srcY, FrameWidth, FrameHeight,
            screenX, screenY, destSize, destSize);
    }
}
// end AI-generated