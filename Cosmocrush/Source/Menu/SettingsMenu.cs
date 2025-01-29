//using YamlDotNet.Serialization;

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

    public override void Ready()
    {
        applyButton!.LeftClicked += OnApplyButtonLeftClicked;
        returnButton!.LeftClicked += OnReturnButtonPressed;
    }

    public override void Update()
    {
        UpdateLabels();
        UpdateButtons();
    }

    private void OnApplyButtonLeftClicked(Button sender)
    {
        //Settings.Instance.SettingsData.MasterVolume = masterSlider.Value;
        //Settings.Instance.SettingsData.MusicVolume = musicSlider.Value;
        //Settings.Instance.SettingsData.SfxVolume = sfxSlider.Value;
        //Settings.Instance.Save();
    }

    private void OnReturnButtonPressed(Button sender)
    {
        PackedSceneYamlNested mainMenuScene = new("Res/Scenes/Menu/MainMenu.yaml");
        var mainMenu = mainMenuScene.Instantiate<MainMenu>();
        Parent!.AddChild(mainMenu);
        Destroy();
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
}