using Cherris;

namespace Cosmocrush;

public class SettingsMenu : Node2D
{
    private readonly VBoxContainer? sliderContainer;

    private readonly Label? masterLabel;
    
    private readonly HSlider? masterSlider;
    private readonly HSlider? musicSlider;
    
    private readonly HSlider? sfxSlider;
    private readonly Button? applyButton;
    private readonly Button? returnButton;

    private float previousMasterVolume = 0;
    private float previousMusicVolume = 0;
    private float previousSfxVolume = 0;

    // Main

    public override void Ready()
    {
        applyButton!.LeftClicked += OnApplyButtonLeftClicked;
        returnButton!.LeftClicked += OnReturnButtonPressed;

        masterSlider!.Value = GameSettings.Instance.SettingsData.MasterVolume;
        musicSlider!.Value = GameSettings.Instance.SettingsData.MusicVolume;
        sfxSlider!.Value = GameSettings.Instance.SettingsData.SfxVolume;

        CapturePreviousVolumes();
    }

    public override void Update()
    {
        UpdateApplyAvailability();
        UpdateButtons();

        sliderContainer!.Position = VisualServer.WindowSize / 2;

        sliderContainer!.Position = new(
            VisualServer.WindowSize.X / 2 - (masterSlider!.Size.X + masterLabel!.Size.X) / 2,
            VisualServer.WindowSize.Y * 0.25F);
    }

    // Events

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
        PackedScene mainMenuScene = new("Res/Scenes/Menu/MainMenu.yaml");
        var mainMenu = mainMenuScene.Instantiate<MainMenu>();
        Parent!.AddChild(mainMenu);
        Free();
    }

    // Update

    private void CapturePreviousVolumes()
    {
        previousMasterVolume = masterSlider!.Value;
        previousMusicVolume = musicSlider!.Value;
        previousSfxVolume = sfxSlider!.Value;
    }

    private void UpdateButtons()
    {
        Vector2 windowSize = VisualServer.WindowSize;

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