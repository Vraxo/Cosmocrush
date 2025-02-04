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

        //GetNode<ColliderRectangle>("ActualCollider").CollisionLayers = [10];
    }

    public override void Update()
    {
        base.Update();

        LookAtMouse();
        HandleMovement();

        //GlobalPosition = GetNode<ColliderRectangle>("ActualCollider").GlobalPosition;
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

    private void LookAtMouse()
    {
        sprite.FlipH = Input.WorldMousePosition.X <= GlobalPosition.X;
    }

    private void HandleMovement()
    {
        Vector2 direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
        Position += direction * TimeServer.Delta * speed;
    }

    private void PlayDamageSound()
    {
        AudioPlayer audioPlayer = new()
        {
            Audio = ResourceLoader.Load<Audio>(damageAudioPath),
            Bus = "SFX"
        };

        AddChild(audioPlayer);
        audioPlayer.Finished += (audioPlayer) => audioPlayer.Free();
        audioPlayer.Play();
    }

    private void Die()
    {
        Free();
    }
}