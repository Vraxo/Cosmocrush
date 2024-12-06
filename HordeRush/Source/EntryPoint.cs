using Nodica;

namespace HordeRush;

public class EntryPoint
{
    [STAThread]
    public static void Main(string[] args)
    {
        App.Instance.Start();
    }
}