using TheAdventure.Engine;
using TheAdventure.Models;

namespace TheAdventure.Levels;

public unsafe class Level
{
    private readonly Renderer _renderer;
    private readonly Silk.NET.SDL.Texture* _tileset;
    
    public bool AllCollected => Collectibles.All(c => c.Collected);

    private const int TileSize    = 48;
    private const int DisplaySize = 64;

    private readonly int[,] _tiles;
    public int Rows    { get; }
    public int Columns { get; }

    public Portal              Portal       { get; }
    public List<Collectible>   Collectibles { get; }

    public int SpawnX { get; }
    public int SpawnY { get; }

    private static readonly (int col, int row)[] TileDefs = new[]
    {
        (2, 0),  //  0 = grass
        (2, 1),  //  1 = bottom grass
        (4, 0),  //  2 = stone
        (4, 1),  //  3 = stone bottom
        (4, 0),  //  4 = dark grass
        (6, 0),  //  5 = light grass
        (6, 1),  //  6 = light grass bottom
        (0, 6),  //  7 = lava
        (1, 6),  //  8 = lava2
        (0, 0),  //  9 = water
        (4, 6),  //  10 = dirt path
        // (4, 1),  // 10 = stone brick 2
        // (5, 0),  // 11 = stone variant
        // (6, 0),  // 12 = tile variant
        // (7, 0),  // 13 = tile variant
        // (0, 3),  // 14 = brown floor
        // (1, 3),  // 15 = brown floor 2
    };

    public Level(Renderer renderer, int[,] tiles,
                 int portalCol, int portalRow,
                 int spawnCol,  int spawnRow,
                 List<(int col, int row)>? mushrooms = null)
    {
        _renderer    = renderer;
        _tiles       = tiles;
        Rows         = tiles.GetLength(0);
        Columns      = tiles.GetLength(1);
        SpawnX       = spawnCol * DisplaySize;
        SpawnY       = spawnRow * DisplaySize;
        _tileset     = renderer.LoadTexture("Assets/Tiles/Map_tiles.bmp");
        Portal       = new Portal(renderer, portalCol, portalRow);

        Collectibles = new List<Collectible>();
        if (mushrooms != null)
            foreach (var (c, r) in mushrooms)
                Collectibles.Add(new Collectible(renderer, c, r, 100));
    }

    public void Update(double deltaMs)
    {
        Portal.Update(deltaMs);
        foreach (var c in Collectibles)
            c.Update(deltaMs);
    }

    public void Draw()
    {
        for (int row = 0; row < Rows; row++)
        for (int col = 0; col < Columns; col++)
        {
            int t = _tiles[row, col];
            if (t < 0 || t >= TileDefs.Length) continue;
            var (tc, tr) = TileDefs[t];
            _renderer.DrawTexture(_tileset,
                tc * TileSize, tr * TileSize, TileSize, TileSize,
                col * DisplaySize, row * DisplaySize, DisplaySize, DisplaySize);
        }

        foreach (var c in Collectibles) c.Draw();
        Portal.Draw();
    }

    public int GetTileAt(int px, int py)
    {
        int col = px / DisplaySize;
        int row = py / DisplaySize;
        if (row < 0 || row >= Rows || col < 0 || col >= Columns) return -1;
        return _tiles[row, col];
    }

    public bool IsHazard(int t) => t == 7 || t == 8 || t == 9;
}