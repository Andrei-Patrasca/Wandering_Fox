using Silk.NET.SDL;
using Silk.NET.Maths;
using System.Runtime.InteropServices;

namespace TheAdventure.Engine;

using SdlRenderer = Silk.NET.SDL.Renderer;

public unsafe class Renderer : IDisposable
{
    // Call SDL2 native functions directly — Silk.NET wraps the DLL but hides some functions
    [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
    private static extern RWops* SDL_RWFromFile(byte* file, byte* mode);

    [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
    private static extern Surface* SDL_LoadBMP_RW(RWops* src, int freesrc);

    private readonly Sdl _sdl;
    private SdlRenderer* _renderer;
    private readonly Dictionary<string, nint> _textureCache = new();

    public Renderer(Sdl sdl, SdlRenderer* renderer)
    {
        _sdl      = sdl;
        _renderer = renderer;
    }

    public Texture* LoadTexture(string relativePath)
    {
        if (_textureCache.TryGetValue(relativePath, out var cached))
            return (Texture*)cached;

        var fullPath = Path.Combine(AppContext.BaseDirectory, relativePath);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Asset not found: {fullPath}");

        var pathBytes = System.Text.Encoding.UTF8.GetBytes(fullPath + "\0");
        var modeBytes = System.Text.Encoding.UTF8.GetBytes("rb\0");

        Surface* surface;
        fixed (byte* pathPtr = pathBytes, modePtr = modeBytes)
        {
            var rw = SDL_RWFromFile(pathPtr, modePtr);
            if (rw == null)
                throw new Exception($"SDL_RWFromFile failed: {fullPath}");
            surface = SDL_LoadBMP_RW(rw, 1);
        }

        if (surface == null)
            throw new Exception($"Failed to load BMP: {relativePath}");

        // Magenta = transparent color key
        _sdl.SetColorKey(surface, 1, _sdl.MapRGB(surface->Format, 255, 0, 255));

        var texture = _sdl.CreateTextureFromSurface(_renderer, surface);
        _sdl.FreeSurface(surface);

        if (texture == null)
            throw new Exception($"Failed to create texture: {relativePath}");

        _textureCache[relativePath] = (nint)texture;
        return texture;
    }

    public void DrawTexture(Texture* texture,
                            int srcX, int srcY, int srcW, int srcH,
                            int dstX, int dstY, int dstW, int dstH)
    {
        var src = new Rectangle<int>(srcX, srcY, srcW, srcH);
        var dst = new Rectangle<int>(dstX, dstY, dstW, dstH);
        _sdl.RenderCopy(_renderer, texture, ref src, ref dst);
    }

    public void DrawTexture(Texture* texture,
                            int dstX, int dstY, int dstW, int dstH)
    {
        var dst = new Rectangle<int>(dstX, dstY, dstW, dstH);
        _sdl.RenderCopy(_renderer, texture, (Rectangle<int>*)null, ref dst);
    }

    public void Clear(byte r = 30, byte g = 30, byte b = 30)
    {
        _sdl.SetRenderDrawColor(_renderer, r, g, b, 255);
        _sdl.RenderClear(_renderer);
    }

    public void Present() => _sdl.RenderPresent(_renderer);

    public void Dispose()
    {
        foreach (var ptr in _textureCache.Values)
            _sdl.DestroyTexture((Texture*)ptr);
        _textureCache.Clear();
    }
}