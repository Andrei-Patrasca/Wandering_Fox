namespace TheAdventure.Engine;

public class ScoreBoard
{
    private readonly string _filePath;

    public int HighScore { get; private set; } = 0;

    public ScoreBoard()
    {
        _filePath = Path.Combine(AppContext.BaseDirectory, "highscore.txt");
        Load();
    }

    private void Load()
    {
        try
        {
            if (File.Exists(_filePath))
                HighScore = int.Parse(File.ReadAllText(_filePath).Trim());
        }
        catch
        {
            HighScore = 0;
        }
    }

    public void Save(int score)
    {
        if (score > HighScore)
        {
            HighScore = score;
            File.WriteAllText(_filePath, HighScore.ToString());
        }
    }
}