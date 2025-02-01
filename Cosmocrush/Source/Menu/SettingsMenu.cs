using Cherris;

namespace Cosmocrush;

public class SettingsMenu : Node2D
{
    private readonly Label? masterLabel;
    private readonly Label? musicLabel;
    private readonly Label? sfxLabel;
    private readonly HSlider? masterSlider;
    private readonly HSlider? musicSlider;
    private readonly HSlider? sfxSlider;
    private readonly Button? applyButton;
    private readonly Button? returnButton;
    private float previousMasterVolume = 0;
    private float previousMusicVolume = 0;
    private float previousSfxVolume = 0;

    public override void Ready()
    {
        //applyButton!.LeftClicked += OnApplyButtonLeftClicked;
        //returnButton!.LeftClicked += OnReturnButtonPressed;
        //
        //masterSlider!.Value = GameSettings.Instance.SettingsData.MasterVolume;
        //musicSlider!.Value = GameSettings.Instance.SettingsData.MusicVolume;
        //sfxSlider!.Value = GameSettings.Instance.SettingsData.SfxVolume;

        //CapturePreviousVolumes();
    }

    public override void Update()
    {
        //UpdateApplyAvailability();
        //UpdateLabels();
        //UpdateButtons();
    }

    private void OnApplyButtonLeftClicked(Button sender)
    {
        GameSettings.Instance.SettingsData.MasterVolume = masterSlider!.Value;
        GameSettings.Instance.SettingsData.MusicVolume = musicSlider!.Value;
        GameSettings.Instance.SettingsData.SfxVolume = sfxSlider!.Value;
        GameSettings.Instance.Save();

        CapturePreviousVolumes();
    }

    private void OnReturnButtonPressed(Button sender)
    {
        PackedSceneYamlNested mainMenuScene = new("Res/Scenes/Menu/MainMenu.yaml");
        var mainMenu = mainMenuScene.Instantiate<MainMenu>();
        Parent!.AddChild(mainMenu);
        Destroy();
    }

    private void CapturePreviousVolumes()
    {
        previousMasterVolume = masterSlider!.Value;
        previousMusicVolume = musicSlider!.Value;
        previousSfxVolume = sfxSlider!.Value;
    }

    private void UpdateLabels()
    {
        Vector2 windowSize = WindowManager.Size;

        masterLabel!.Position = new(
            windowSize.X / 2 - masterSlider!.Size.X / 2 - masterLabel.Size.X,
            windowSize.Y / 2 - 50
        );

        musicLabel!.Position = new(
            windowSize.X / 2 - musicSlider!.Size.X / 2 - masterLabel.Size.X,
            windowSize.Y / 2
        );

        sfxLabel!.Position = new(
            windowSize.X / 2 - sfxSlider!.Size.X / 2 - masterLabel.Size.X,
            windowSize.Y / 2 + 50
        );
    }

    private void UpdateButtons()
    {
        Vector2 windowSize = WindowManager.Size;

        float buttonSpacing = 20f;
        
        applyButton!.Position = new(
            (windowSize.X / 2) - applyButton.Size.X - buttonSpacing,
            windowSize.Y / 2 + 100
        );
        
        returnButton!.Position = new(
            (windowSize.X / 2) + buttonSpacing,
            windowSize.Y / 2 + 100
        );
    }

    private void UpdateApplyAvailability()
    {
        applyButton!.Disabled = 
            masterSlider!.Value == previousMasterVolume &&
            musicSlider!.Value == previousMusicVolume &&
            sfxSlider!.Value == previousSfxVolume;
    }
}