using Cherris;

namespace Cosmocrush;

public partial class CombatThemePlayer : AudioPlayer
{
    private readonly Random random = new Random();
    private int currentThemeIndex = -1;
    private readonly string themePathTemplate = "Res/MoveAudio/Music/CombatThemes/CombatTheme{0}.mp3";

    public override void Ready()
    {
        //PlayRandomTheme();
        Finished += OnThemeFinished;
    }

    private void PlayRandomTheme()
    {
        int nextThemeIndex;
        do
        {
            nextThemeIndex = random.Next(1, 7);
        }
        while (nextThemeIndex == currentThemeIndex);

        currentThemeIndex = nextThemeIndex;
        Audio = new(string.Format(themePathTemplate, currentThemeIndex));
        Play();
    }

    private void OnThemeFinished(AudioPlayer audioPlaer)
    {
        PlayRandomTheme();
    }
}