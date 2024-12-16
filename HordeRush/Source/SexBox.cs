using Nodica;

namespace HordeRush;

public class SexBox : VerticalContainer
{
    public override void Update()
    {
        base.Update();

        var parent = (Parent as PopUp)!;

        //Size = parent.Size - new Vector2(0, parent.TitleBarHeight);
        //Position = parent.Size / 2 - Origin;
    }
}