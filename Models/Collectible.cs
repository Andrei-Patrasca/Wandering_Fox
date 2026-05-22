using TheAdventure.Engine;

namespace TheAdventure.Models;

public unsafe class Collectible : IGameObject
{
    private readonly Renderer _renderer;
    private readonly Silk.NET.SDL.Texture* _texture;

    // Mushroom is 160x240 — single static image
    private const int SrcW    = 160;
    private const int SrcH    = 240;
    private const int DrawW   = 40;
    private const int DrawH   = 60;

    public int  X         { get; }
    public int  Y         { get; }
    public int  Points    { get; }
    public bool Collected { get; private set; } = false;

    public Collectible(Renderer renderer, int tileCol, int tileRow, int points = 100)
    {
        _renderer = renderer;
        _texture  = renderer.LoadTexture("Assets/Items/mushroom.bmp");
        X         = tileCol * 64 + 12;
        Y         = tileRow * 64 + 4;
        Points    = points;
    }

    public void Update(double deltaMs) { } // no animation needed

    public void Draw()
    {
        if (Collected) return;
        _renderer.DrawTexture(_texture,
            0, 0, SrcW, SrcH,
            X, Y, DrawW, DrawH);
    }

    public bool CheckCollect(int playerX, int playerY, int playerSize)
    {
        if (Collected) return false;
        bool hit = playerX < X + DrawW && playerX + playerSize > X &&
                   playerY < Y + DrawH && playerY + playerSize > Y;
        if (hit) Collected = true;
        return hit;
    }
}