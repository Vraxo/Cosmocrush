using Cherris;

namespace Cosmocrush;

public class EntryPoint
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppManager.Instance.Run();
    }
}