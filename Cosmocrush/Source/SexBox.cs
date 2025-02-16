using Cherris;

namespace Cosmocrush;

public class SexBox : VerticalContainer
{
    public override void Process()
    {
        base.Process();

        var parent = (Parent as PopUp)!;

        Size = parent.Size - new Vector2(0, parent.TitleBarHeight);
    }
}