namespace TheAdventure.Engine;

public class GameState
{
    public int Score     { get; private set; } = 0;
    public int Lives     { get; private set; } = 3;
    public int Level     { get; private set; } = 1;
    public bool GameOver { get; private set; } = false;
    public bool Won      { get; private set; } = false;

    public void AddScore(int points) => Score += points;

    public void LoseLife()
    {
        Lives--;
        if (Lives <= 0)
        {
            Lives    = 0;
            GameOver = true;
        }
    }

    public void NextLevel() => Level++;
    public void SetWon()    => Won = true;

    public void Reset()
    {
        Score    = 0;
        Lives    = 3;
        Level    = 1;
        GameOver = false;
        Won      = false;
    }
}