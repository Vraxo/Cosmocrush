using Cherris;

namespace Cosmocrush;

public class Player : ColliderRectangle
{
    public int Health { get; private set; } = 100;

    private Sprite sprite = new();
    private readonly float speed = 200f;
    private readonly string damageAudioPath = "Res/Audio/SFX/Damage.mp3";

    public override void Ready()
    {
        base.Ready();
        sprite = GetNode<Sprite>("Sprite");
    }

    public override void Update()
    {
        base.Update();
        sprite.FlipH = Input.MousePosition.X <= GlobalPosition.X;
        Vector2 direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
        Position += direction * Time.Delta * speed;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        PlayDamageSound();

        if (Health <= 0)
        {
            Die();
        }
    }

    private void PlayDamageSound()
    {
        AudioPlayer audioPlayer = new()
        {
            Audio = ResourceLoader.Load<Audio>(damageAudioPath)
        };

        AddChild(audioPlayer);
        audioPlayer.Finished += OnAudioPlayerFinished;
        audioPlayer.Play();
    }

    private void OnAudioPlayerFinished(AudioPlayer sender)
    {
        sender.Destroy();
    }

    private void Die()
    {
        Destroy();
    }
}