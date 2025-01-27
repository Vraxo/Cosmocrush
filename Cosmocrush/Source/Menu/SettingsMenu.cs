//using YamlDotNet.Serialization;

using Cherris;

namespace Cosmocrush;

public class SettingsMenu : Node2D
{
    //private PackedScene mainMenuScene = ResourceLoader.Load<PackedScene>("Res/Scenes/Menu/MainMenu.ini");

    private HSlider? masterSlider;
    private HSlider? musicSlider;
    private HSlider? sfxSlider;
    private Label? masterLabel;
    private Label? musicLabel;
    private Label? sfxLabel;
    private Button? applyButton;
    private Button? returnButton;

    public override void Ready()
    {
        masterLabel = GetNode<Label>("MasterLabel");
        musicLabel = GetNode<Label>("MusicLabel");
        sfxLabel = GetNode<Label>("SfxLabel");

        masterSlider = GetNode<HSlider>("MasterLabel/Slider");
        musicSlider = GetNode<HSlider>("MusicLabel/Slider");
        sfxSlider = GetNode<HSlider>("SfxLabel/Slider");

        //masterSlider.Value = Settings.Instance.SettingsData.MasterVolume;
        //musicSlider.Value = Settings.Instance.SettingsData.MusicVolume;
        //sfxSlider.Value = Settings.Instance.SettingsData.SfxVolume;
        //
        //applyButton.Pressed += OnApplyButtonPressed;
        //returnButton.Pressed += OnReturnButtonPressed;
    }

    private void OnApplyButtonPressed()
    {
        //Settings.Instance.SettingsData.MasterVolume = masterSlider.Value;
        //Settings.Instance.SettingsData.MusicVolume = musicSlider.Value;
        //Settings.Instance.SettingsData.SfxVolume = sfxSlider.Value;
        //Settings.Instance.Save();
    }

    private void OnReturnButtonPressed()
    {
        //var mainMenu = mainMenuScene.Instantiate<Node2D>();
        //GetParent().AddChild(mainMenu);
        //QueueFree();
    }

    public override void Update()
    {
        UpdateLabels();
        UpdateButtons();
    }

    private void UpdateLabels()
    {
        Vector2 windowSize = WindowManager.Size;

        masterLabel!.Position = new(
            windowSize.X / 2 - masterSlider!.Size.X / 2,
            windowSize.Y / 2 - 50
        );

        musicLabel!.Position = new(
            windowSize.X / 2 - musicSlider!.Size.X / 2,
            windowSize.Y / 2
        );

        sfxLabel!.Position = new(
            windowSize.X / 2 - sfxSlider!.Size.X / 2,
            windowSize.Y / 2 + 50
        );
    }

    private void UpdateButtons()
    {
        //Vector2I windowSize = DisplayServer.WindowGetSize();
        //
        //float buttonSpacing = 20f;
        //
        //applyButton.Position = new(
        //    (windowSize.X / 2) - applyButton.Size.X - buttonSpacing,
        //    windowSize.Y / 2 + 100
        //);
        //
        //returnButton.Position = new(
        //    (windowSize.X / 2) + buttonSpacing,
        //    windowSize.Y / 2 + 100
        //);
    }
}