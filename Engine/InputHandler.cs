using Silk.NET.SDL;

namespace TheAdventure.Engine;

public class InputHandler
{
    private readonly HashSet<Scancode> _heldKeys = new();

    public void KeyDown(int scancode) => _heldKeys.Add((Scancode)scancode);
    public void KeyUp(int scancode)   => _heldKeys.Remove((Scancode)scancode);
    public bool IsHeld(Scancode key)  => _heldKeys.Contains(key);

    public bool Up    => IsHeld(Scancode.ScancodeUp)    || IsHeld(Scancode.ScancodeW);
    public bool Down  => IsHeld(Scancode.ScancodeDown)  || IsHeld(Scancode.ScancodeS);
    public bool Left  => IsHeld(Scancode.ScancodeLeft)  || IsHeld(Scancode.ScancodeA);
    public bool Right => IsHeld(Scancode.ScancodeRight) || IsHeld(Scancode.ScancodeD);
}