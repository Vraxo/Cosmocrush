using Raylib_cs;

namespace Cherris;

public class AudioPlayer : Node
{
    public string Bus { get; set; } = "Master";

    private Audio? _audio;
    public Audio? Audio
    {
        get => _audio;

        set
        {
            _audio = value;
            Volume = _volume;
            Volume = AudioManagerCore.Instance.GetBusVolume(Bus);
            Pitch = _pitch;
            Pan = _pan;
        }
    }

    public bool AutoPlay { get; set; } = false;
    public bool Loop { get; set; } = false;
    public bool Playing = false;
    public float TimePlayed => Audio is not null ? Raylib.GetMusicTimePlayed(Audio!) : 0;

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

    public delegate void AudioPlayerEventHandler(AudioPlayer sender);
    public event AudioPlayerEventHandler? Paused;
    public event AudioPlayerEventHandler? Resumed;
    public event AudioPlayerEventHandler? Finished;

    public AudioPlayer()
    {
        AudioManagerCore.Instance.VolumeChanged += OnAudioManagerBusVolumeChanged;
    }

    public override void Ready()
    {
        if (Audio is null)
        {
            return;
        }

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

    public void Play(float timestamp = 0.1f)
    {
        if (Audio is null)
        {
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
            return;
        }

        Raylib.ResumeMusicStream(Audio);
        Resumed?.Invoke(this);
    }

    public void Pause()
    {
        if (Audio is null)
        {
            return;
        }

        Raylib.PauseMusicStream(Audio);
        Paused?.Invoke(this);
    }

    public void Stop()
    {
        if (Audio is null)
        {
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
            return;
        }

        timestamp = Math.Clamp(timestamp, 0.1f, Audio.Length);

        Raylib.SeekMusicStream(Audio, timestamp);
    }

    private void OnAudioManagerBusVolumeChanged(string bus, float volume)
    {
        if (Bus == bus)
        {
            Volume = volume;
            Console.WriteLine("Set volume to " + volume);
        }
    }
}