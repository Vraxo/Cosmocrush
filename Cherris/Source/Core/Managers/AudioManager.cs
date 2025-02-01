namespace Cherris;

public static class AudioManager
{
    public static event AudioManagerCore.BusVolumeChangedEventHandler VolumeChanged
    {
        add => AudioManagerCore.Instance.VolumeChanged += value;
        remove => AudioManagerCore.Instance.VolumeChanged -= value;
    }

    public static void PlaySound(Audio audio, string bus = "Master")
    {
        AudioManagerCore.Instance.PlaySound(audio, bus);
    }

    public static void SetBusVolume(string bus, float volume)
    {
        AudioManagerCore.Instance.SetBusVolume(bus, volume);
    }

    public static float GetBusVolume(string bus)
    {
        return AudioManagerCore.Instance.GetBusVolume(bus);
    }

    public static void AddBus(string name, float volume = 1f)
    {
        AudioManagerCore.Instance.AddBus(name, volume);
    }
}