using Nodica;

namespace Cosmocrush;

public class EntryPoint
{
    [STAThread]
    public static void Main(string[] args)
    {
        App.Instance.Start();
    }
}