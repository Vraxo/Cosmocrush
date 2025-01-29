//using YamlDotNet.Serialization;

using Cherris;

namespace Cosmocrush;

public class SettingsMenu : Node2D
{
    public Label? masterLabel { get; set; }
    private Label? musicLabel;
    private Label? sfxLabel;
    private HSlider? masterSlider;
    private HSlider? musicSlider;
    private HSlider? sfxSlider;
    private Button? applyButton;
    private Button? returnButton;

    public override void Ready()
    {
        masterSlider = GetNode<HSlider>("MasterLabel/Slider");
        musicSlider = GetNode<HSlider>("MusicLabel/Slider");
        sfxSlider = GetNode<HSlider>("SfxLabel/Slider");

        applyButton = GetNode<Button>("ApplyButton");
        returnButton = GetNode<Button>("ReturnButton");

        applyButton.LeftClicked += OnApplyButtonLeftClicked;
        returnButton.LeftClicked += OnReturnButtonPressed;
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