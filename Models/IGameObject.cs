namespace TheAdventure.Models;

public interface IGameObject
{
    int X { get; }
    int Y { get; }
    void Update(double deltaMs);
    void Draw();
}