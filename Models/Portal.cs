using TheAdventure.Engine;

namespace TheAdventure.Models;

public unsafe class Portal : IGameObject
{
    private readonly Renderer _renderer;
    private readonly Silk.NET.SDL.Texture* _texture;

    // portalRings1.png = 128x160, grid 2 cols x 5 rows
    private const int SheetCols   = 4;
    private const int SheetRows   = 5;
    private const int FrameW      = 32;  // 128 / 4
    private const int FrameH      = 32;  // 160 / 5
    private const int TotalFrames = 20;
    private const int DrawSize  = 80;
    private const double FrameMs = 100;

    private int    _frame      = 0;
    private double _frameTimer = 0;

    public int X { get; }
    public int Y { get; }

    public Portal(Renderer renderer, int tileCol, int tileRow)
    {
        _renderer = renderer;
        _texture  = renderer.LoadTexture("Assets/Items/portal.bmp");
        X         = tileCol * 64 - 8;
        Y         = tileRow * 64 - 8;
    }

    public void Update(double deltaMs)
    {
        _frameTimer += deltaMs;
        if (_frameTimer >= FrameMs)
        {
            _frameTimer -= FrameMs;
            _frame = (_frame + 1) % TotalFrames;
        }
    }

    public void Draw()
    {
        // Convert linear frame index to col/row in the sheet
        int col = _frame % SheetCols;
        int row = _frame / SheetCols;

        _renderer.DrawTexture(_texture,
            col * FrameW, row * FrameH, FrameW, FrameH,
            X, Y, DrawSize, DrawSize);
    }

    public bool PlayerOnPortal(int playerX, int playerY, int playerSize)
    {
        return playerX < X + DrawSize && playerX + playerSize > X &&
               playerY < Y + DrawSize && playerY + playerSize > Y;
    }
}