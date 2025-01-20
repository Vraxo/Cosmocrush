using Cherris;

namespace Cosmocrush;

public class SexBox : VerticalContainer
{
    public override void Update()
    {
        base.Update();

        var parent = (Parent as PopUp)!;

        Size = parent.Size - new Vector2(0, parent.TitleBarHeight);
    }
}