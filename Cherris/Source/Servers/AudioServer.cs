namespace Cherris;

public static class AudioServer
{
    public static event AudioServerCore.BusVolumeChangedEventHandler VolumeChanged
    {
        add => AudioServerCore.Instance.VolumeChanged += value;
        remove => AudioServerCore.Instance.VolumeChanged -= value;
    }

    public static void PlaySound(Sound sound, string bus = "Master")
    {
        AudioServerCore.Instance.PlaySound(sound, bus);
    }

    public static void SetBusVolume(string bus, float volume)
    {
        AudioServerCore.Instance.SetBusVolume(bus, volume);
    }

    public static float GetBusVolume(string bus)
    {
        return AudioServerCore.Instance.GetBusVolume(bus);
    }

    public static void AddBus(string name, float volume = 1f)
    {
        AudioServerCore.Instance.AddBus(name, volume);
    }

    public static bool BusExists(string name)
    {
        return AudioServerCore.Instance.BusExists(name);
    }
}