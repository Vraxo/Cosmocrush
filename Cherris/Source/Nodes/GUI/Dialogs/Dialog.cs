namespace Cherris;

public partial class Dialog : Node2D
{
    public override void Start()
    {
        Offset = GetNode<ThemedRectangle>("Background").Size / 2;
        ClickServer.Instance.MinLayer = ClickableLayer.DialogButtons;
        GetNode<Button>("CloseButton").LeftClicked += OnCloseButtonLeftClicked;
    }

    public override void Update()
    {
        UpdatePosition();
    }

    protected void Close()
    {
        ClickServer.Instance.MinLayer = 0;
        Free();
    }

    private void OnCloseButtonLeftClicked(Button sender)
    {
        Close();
    }

    private void UpdatePosition()
    {
        Position = VisualServer.WindowSize / 2;
    }
}