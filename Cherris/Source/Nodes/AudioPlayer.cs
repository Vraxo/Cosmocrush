using Raylib_cs;

namespace Cherris;

public class AudioPlayer : Node
{
    public bool AutoPlay { get; set; } = false;
    public bool Loop { get; set; } = false;
    public bool Playing { get; private set; } = false;

    public string Bus
    {
        get;
        set
        {
            if (Bus == value)
            {
                return;
            }

            if (!AudioServer.Instance.BusExists(value))
            {
                Log.Error($"[AudioPlayer] [{Name}] Bus: '{value}' does not exist.");
                return;
            }

            field = value;
            BusChanged?.Invoke(this, Bus);
        }
    } = "Master";

    public AudioStream? Audio
    {
        get;

        set
        {
            field = value;
            Volume = Volume;
            Volume = AudioServer.Instance.GetBusVolume(Bus);
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
                Log.Error($"[AudioPlayer] [{Name}] TimePlayed: AudioStream is null.");
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

    public delegate void Event(AudioPlayer sender);
    public delegate void BusEvent(AudioPlayer sender, string bus);
    public event Event? Paused;
    public event Event? Resumed;
    public event Event? Finished;
    public event BusEvent? BusChanged;

    // Main

    public AudioPlayer()
    {
        AudioServer.Instance.VolumeChanged += OnBusVolumeChanged;
    }

    public override void Ready()
    {
        if (AutoPlay)
        {
            Play();
        }
    }

    public override void Process()
    {
        if (Audio is null)
        {
            return;
        }

        Volume = AudioServer.Instance.GetBusVolume(Bus);

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
            Log.Error($"[{Name}] Play: AudioStream is null.");
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
            Log.Error($"[AudioPlayer] [{Name}] Resume: AudioStream is null.");
            return;
        }

        Raylib.ResumeMusicStream(Audio);
        Resumed?.Invoke(this);
    }

    public void Pause()
    {
        if (Audio is null)
        {
            Log.Error($"[AudioPlayer] [{Name}] Pause: AudioStream is null.");
            return;
        }

        Raylib.PauseMusicStream(Audio);
        Paused?.Invoke(this);
    }

    public void Stop()
    {
        if (Audio is null)
        {
            Log.Error($"[AudioPlayer] [{Name}] Stop: AudioStream is null.");
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
            Log.Error($"[AudioPlayer] [{Name}] Seek: AudioStream is null.");
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