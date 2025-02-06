﻿using Cherris;

namespace Cosmocrush;

public partial class CombatThemePlayer : AudioPlayer
{
    private readonly Random random = new();
    private int currentThemeIndex = -1;
    private readonly string themePathTemplate = "Res/Audio/Music/CombatThemes/CombatTheme5{0}.mp3";

    public override void Ready()
    {
        PlayRandomTheme();
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
        string path = string.Format(themePathTemplate, currentThemeIndex);
        Audio = ResourceLoader.Load<Audio>(path);

        Play();
    }

    private void OnThemeFinished(AudioPlayer audioPlaer)
    {
        PlayRandomTheme();
    }
}