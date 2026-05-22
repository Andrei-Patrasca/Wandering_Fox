// AI-generated
using Silk.NET.SDL;
using Silk.NET.Maths;

namespace TheAdventure.Engine;

using SdlRenderer = Silk.NET.SDL.Renderer;

public unsafe class HUD
{
    private readonly Sdl _sdl;
    private SdlRenderer* _renderer;
    private readonly TheAdventure.Engine.Renderer _gameRenderer;
    private readonly Texture* _heartTexture;

    private const int HeartSize  = 36;
    private const int HeartPad   = 8;
    private const int TopPad     = 8;
    private const int ScoreBoxW  = 120;
    private const int ScoreBoxH  = 36;
    private const int DigitW     = 10;
    private const int DigitH     = 16;

    // 5x3 pixel font for digits 0-9
    private static readonly bool[,,] Digits = new bool[10, 5, 3]
    {
        { {true,true,true},{true,false,true},{true,false,true},{true,false,true},{true,true,true} },   // 0
        { {false,true,false},{false,true,false},{false,true,false},{false,true,false},{false,true,false} }, // 1
        { {true,true,true},{false,false,true},{true,true,true},{true,false,false},{true,true,true} },   // 2
        { {true,true,true},{false,false,true},{true,true,true},{false,false,true},{true,true,true} },   // 3
        { {true,false,true},{true,false,true},{true,true,true},{false,false,true},{false,false,true} }, // 4
        { {true,true,true},{true,false,false},{true,true,true},{false,false,true},{true,true,true} },   // 5
        { {true,true,true},{true,false,false},{true,true,true},{true,false,true},{true,true,true} },    // 6
        { {true,true,true},{false,false,true},{false,false,true},{false,false,true},{false,false,true} },// 7
        { {true,true,true},{true,false,true},{true,true,true},{true,false,true},{true,true,true} },     // 8
        { {true,true,true},{true,false,true},{true,true,true},{false,false,true},{true,true,true} },    // 9
    };

    public HUD(Sdl sdl, SdlRenderer* renderer, TheAdventure.Engine.Renderer gameRenderer)
    {
        _sdl          = sdl;
        _renderer     = renderer;
        _gameRenderer = gameRenderer;
        _heartTexture = gameRenderer.LoadTexture("Assets/UI/heart.bmp");
    }

    public void Draw(int lives, int score, int screenWidth)
    {
        DrawLives(lives);
        DrawScore(score, screenWidth);
    }

    private void DrawLives(int lives)
    {
        for (int i = 0; i < 3; i++)
        {
            int x = HeartPad + i * (HeartSize + 4);
            int y = TopPad;

            if (i < lives)
            {
                // Full heart
                _gameRenderer.DrawTexture(_heartTexture, x, y, HeartSize, HeartSize);
            }
            else
            {
                // Empty heart — draw dark rectangle
                var rect = new Rectangle<int>(x, y, HeartSize, HeartSize);
                _sdl.SetRenderDrawColor(_renderer, 60, 0, 0, 200);
                _sdl.RenderFillRect(_renderer, ref rect);
                _sdl.SetRenderDrawColor(_renderer, 120, 0, 0, 255);
                _sdl.RenderDrawRect(_renderer, ref rect);
            }
        }
    }

    private void DrawScore(int score, int screenWidth)
        {
            int boxX = screenWidth - ScoreBoxW - HeartPad;
            int boxY = TopPad;

            var bgRect = new Rectangle<int>(boxX, boxY, ScoreBoxW, ScoreBoxH);
            _sdl.SetRenderDrawColor(_renderer, 0, 0, 0, 180);
            _sdl.RenderFillRect(_renderer, ref bgRect);
            _sdl.SetRenderDrawColor(_renderer, 255, 215, 0, 255);
            _sdl.RenderDrawRect(_renderer, ref bgRect);

            // Draw each digit of the score
            string text  = score.ToString("D5");
            int    startX = boxX + 10;
            int    startY = boxY + 6;

            for (int i = 0; i < text.Length; i++)
            {
                int digit = text[i] - '0';
                DrawDigit(digit, startX + i * (DigitW + 3), startY);
            }
        }

    private void DrawDigit(int digit, int px, int py)
    {
        int scale = 3;
        for (int row = 0; row < 5; row++)
        for (int col = 0; col < 3; col++)
        {
            if (!Digits[digit, row, col]) continue;
            var r = new Rectangle<int>(
                px + col * scale,
                py + row * scale,
                scale, scale);
            _sdl.SetRenderDrawColor(_renderer, 255, 215, 0, 255);
            _sdl.RenderFillRect(_renderer, ref r);
        }
    }
}
// end AI-generated