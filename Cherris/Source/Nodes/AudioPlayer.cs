using Raylib_cs;

namespace Cherris;

public class AudioPlayer : Node
{
    public bool AutoPlay { get; set; } = false;
    public bool Loop { get; set; } = false;
    public bool Playing = false;

    public string Bus
    {
        get;
        set
        {
            if (Bus == value)
            {
                return;
            }

            if (!AudioServerCore.Instance.BusExists(value))
            {
                Log.Error($"[AudioPlayer] [{Name}] Bus: '{value}' does not exist.");
                return;
            }

            field = value;
            BusChanged?.Invoke(this, Bus);
        }
    } = "Master";

    public Audio? Audio
    {
        get;

        set
        {
            field = value;
            Volume = Volume;
            Volume = AudioServer.GetBusVolume(Bus);
            Pitch = Pitch;
            Pan = Pan;
        }
    }

    public float TimePlayed
    {
        get
        {
            if (Audio is null)
            {
                Log.Error($"[AudioPlayer] [{Name}] TimePlayed: Audio is null.");
                return 0;
            }

            return Raylib.GetMusicTimePlayed(Audio!);
        }
    }

    public float Volume
    {
        get;

        set
        {
            field = value;

            if (Audio is null)
            {
                return;
            }

            Raylib.SetMusicVolume(Audio, field);
        }
    } = 1;

    public float Pitch
    {
        get;

        set
        {
            field = value;

            if (Audio is null)
            {
                return;
            }

            Raylib.SetMusicPitch(Audio, field);
        }
    } = 1;

    public float Pan
    {
        get;

        set
        {
            if (Audio is null)
            {
                return;
            }

            field = value;
            Raylib.SetMusicPan(Audio, field);
        }
    } = 1;

    // Events

    public delegate void EventHandler(AudioPlayer sender);
    public delegate void BusEventHandler(AudioPlayer sender, string bus);
    public event EventHandler? Paused;
    public event EventHandler? Resumed;
    public event EventHandler? Finished;
    public event BusEventHandler? BusChanged;

    // Main

    public AudioPlayer()
    {
        AudioServerCore.Instance.VolumeChanged += OnBusVolumeChanged;
    }

    public override void Ready()
    {
        if (AutoPlay)
        {
            Play();
        }
    }

    public override void Update()
    {
        if (Audio is null)
        {
            return;
        }

        Volume = AudioServer.GetBusVolume(Bus);

        Raylib.UpdateMusicStream(Audio);

        if (TimePlayed >= Audio.Length - 0.1 && Playing)
        {
            if (Loop)
            {
                Play();
                Finished?.Invoke(this);
            }
            else
            {
                Stop();
            }
        }
    }

    // Public

    public void Play(float timestamp = 0.1f)
    {
        if (Audio is null)
        {
            Log.Error($"[{Name}] Play: Audio is null.");
            return;
        }

        Playing = true;

        timestamp = Math.Clamp(timestamp, 0.1f, Audio.Length);

        if (timestamp >= Audio.Length - 0.1f)
        {
            Stop();
        }
        else
        {
            Raylib.SeekMusicStream(Audio, timestamp);
            Raylib.PlayMusicStream(Audio);
        }
    }

    public void Resume()
    {
        if (Audio is null)
        {
            Log.Error($"[AudioPlayer] [{Name}] Resume: Audio is null.");
            return;
        }

        Raylib.ResumeMusicStream(Audio);
        Resumed?.Invoke(this);
    }

    public void Pause()
    {
        if (Audio is null)
        {
            Log.Error($"[AudioPlayer] [{Name}] Pause: Audio is null.");
            return;
        }

        Raylib.PauseMusicStream(Audio);
        Paused?.Invoke(this);
    }

    public void Stop()
    {
        if (Audio is null)
        {
            Log.Error($"[AudioPlayer] [{Name}] Stop: Audio is null.");
            return;
        }

        Playing = false;

        Raylib.StopMusicStream(Audio);
        Finished?.Invoke(this);
    }

    public void Seek(float timestamp)
    {
        if (Audio is null)
        {
            Log.Error($"[AudioPlayer] [{Name}] Seek: Audio is null.");
            return;
        }

        timestamp = Math.Clamp(timestamp, 0.1f, Audio.Length);

        Raylib.SeekMusicStream(Audio, timestamp);
    }

    // Private

    private void OnBusVolumeChanged(string bus, float volume)
    {
        if (Bus == bus)
        {
            Volume = volume;
        }
    }
}