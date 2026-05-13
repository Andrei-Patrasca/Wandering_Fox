using TheAdventure.Engine;

namespace TheAdventure.Models;

public unsafe class PlayerObject
{
    private readonly SpriteSheet _idle;
    private readonly SpriteSheet _walk;
    private readonly SpriteSheet _death;

    private enum State { Idle, Walk, Dead }
    private State _state = State.Idle;

    public int X { get; private set; }
    public int Y { get; private set; }
    public int Size { get; } = 64;
    public bool IsDead => _state == State.Dead;

    private int _spawnX;
    private int _spawnY;

    public PlayerObject(Renderer renderer, int spawnX = 64, int spawnY = 64)
    {
        _spawnX = spawnX;
        _spawnY = spawnY;
        X = spawnX;
        Y = spawnY;
        _idle  = SpriteSheetFactory.CreateIdle(renderer);
        _walk  = SpriteSheetFactory.CreateWalk(renderer);
        _death = SpriteSheetFactory.CreateDeath(renderer);
    }

    public void SetSpawn(int x, int y)
    {
        _spawnX = x;
        _spawnY = y;
    }

    public void Respawn()
    {
        X = _spawnX;
        Y = _spawnY;
        _state = State.Idle;
        _idle.Reset();
        _walk.Reset();
        _death.Reset();
    }

    public void Update(double deltaMs, InputHandler input)
    {
        if (_state == State.Dead)
        {
            _death.Update(deltaMs);
            // When death animation finishes, respawn automatically
            if (_death.IsFinished)
                Respawn();
            return;
        }

        int dx = 0, dy = 0;
        int speed = 3;

        if (input.Up)    dy -= speed;
        if (input.Down)  dy += speed;
        if (input.Left)  dx -= speed;
        if (input.Right) dx += speed;

        bool moving = dx != 0 || dy != 0;

        if (dx < 0) { _idle.Direction = (FacingDirection)2; _walk.Direction = (FacingDirection)2; }
        if (dx > 0) { _idle.Direction = (FacingDirection)3; _walk.Direction = (FacingDirection)3; }
        if (dy < 0) { _idle.Direction = (FacingDirection)1; _walk.Direction = (FacingDirection)1; }
        if (dy > 0) { _idle.Direction = (FacingDirection)0; _walk.Direction = (FacingDirection)0; }

        if (moving)
        {
            X += dx;
            Y += dy;
            _state = State.Walk;
            _walk.Update(deltaMs);
        }
        else
        {
            _state = State.Idle;
            _idle.Update(deltaMs);
        }
    }

    public void Kill()
    {
        if (_state == State.Dead) return;
        _state = State.Dead;
        _death.Direction = _idle.Direction;
        _death.Reset();
    }

    public void Draw()
{
    var sheet = _state switch
    {
        State.Idle => _idle,
        State.Walk => _walk,
        State.Dead => _death,
        _          => _idle
    };
    sheet.Draw(X, Y, Size);
}
}