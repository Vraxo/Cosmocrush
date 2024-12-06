using Raylib_cs;

namespace Nodica;

public class AudioPlayer : Node
{
    public Audio? Audio { get; set; }
    public bool AutoPlay { get; set; } = false;
    public bool Loop { get; set; } = false;
    public bool Playing => Raylib.IsMusicStreamPlaying(Audio!);
    public float TimePlayed => Raylib.GetMusicTimePlayed(Audio!);

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

    private float _pan = 0.5f;
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

    public void Load(string path)
    {
        Audio = new(path);
        Volume = Volume;
        Pitch = Pitch;
        Pan = Pan;
    }

    public void Play(float timestamp = 0.1f)
    {
        if (Audio is null)
        {
            return;
        }

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
}