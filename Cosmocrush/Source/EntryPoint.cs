using Cherris;

namespace Cosmocrush;

public class EntryPoint
{
    [STAThread]
    public static void Main(string[] args)
    {
        App.Instance.Run();
    }
}