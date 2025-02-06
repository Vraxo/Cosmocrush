using Raylib_cs;

namespace Cherris;

public class AudioPlayer : Node
{
    private string _bus = "Master";
    public string Bus 
    {
        get => _bus;
        set
        {
            if (Bus == value)
            {
                return;
            }

            if (!AudioServer.BusExists(Name))
            {
                Log.Error($"[AudioPlayer] [{Name}] Bus: '{value}' does not exist.");
                return;
            }

            _bus = value;
            BusChanged?.Invoke(this, Bus);
        }
    }

    private Audio? _audio;
    public Audio? Audio
    {
        get => _audio;

        set
        {
            _audio = value;
            Volume = _volume;
            Volume = AudioServer.GetBusVolume(Bus);
            Pitch = _pitch;
            Pan = _pan;
        }
    }

    public bool AutoPlay { get; set; } = false;
    public bool Loop { get; set; } = false;
    public bool Playing = false;

    public float TimePlayed
    {
        get
        {
            if (Audio is not null)
            {
                return Raylib.GetMusicTimePlayed(Audio!);
            }
            else
            {
                Log.Error($"[AudioPlayer] [{Name}] TimePlayed: Audio is null.");
                return 0;
            }
        }
    }

    private float _volume = 1;
    public float Volume
    {
        get => _volume;

        set
        {
            _volume = value;

            if (Audio is null)
            {
                return;
            }

            Raylib.SetMusicVolume(Audio, _volume);
        }
    }

    private float _pitch = 1;
    public float Pitch
    {
        get => _pitch;

        set
        {
            _pitch = value;

            if (Audio is null)
            {
                return;
            }

            Raylib.SetMusicPitch(Audio, _pitch);
        }
    }

    private float _pan = 1f;
    public float Pan
    {
        get => _pan;

        set
        {
            if (Audio is null)
            {
                return;
            }

            _pan = value;
            Raylib.SetMusicPan(Audio, _pan);
        }
    }

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
            Log.Error($"[AudioPlayer] [{Name}] Play: Audio is null.");
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