namespace Cherris;

public class Timer : Node
{
    public float TimePassed { get; private set; } = 0;
    public bool AutoStart { get; set; } = false;
    public bool OneShot { get; set; } = true;
    public float WaitTime { get; set; } = 1.0f;

    public float TimeLeft => float.Max(0, WaitTime - TimePassed);

    private bool fired = false;

    // Events

    public delegate void Event(Timer sender);
    public event Event? Timeout;

    // Main

    public override void Ready()
    {
        base.Ready();

        if (AutoStart)
        {
            Fire();
        }
    }

    public override void Process()
    {
        base.Process();

        if (fired)
        {
            TimePassed += Time.Delta;

            if (TimePassed >= WaitTime)
            {
                fired = false;
                TimePassed = 0;

                Timeout?.Invoke(this);

                if (!OneShot)
                {
                    Fire();
                }
            }
        }
    }

    // Public

    public void Fire()
    {
        fired = true;
        TimePassed = 0;
    }

    public void Stop()
    {
        fired = false;
    }

    public void Reset()
    {
        TimePassed = 0;
        fired = false;
    }
}