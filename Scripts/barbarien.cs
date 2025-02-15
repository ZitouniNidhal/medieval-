using Godot;
using System;

public class BarbarianMovement : KinematicBody
{
    // Paramètres de mouvement
    private Vector3 velocity = Vector3.Zero; // Vitesse actuelle du barbare
    private float speed = 8.0f; // Vitesse de déplacement
    private float jumpSpeed = 15.0f; // Force de saut
    private float gravity = -9.81f; // Gravité

    public override void _Ready()
    {
        // Initialisation (si nécessaire)
    }

    public override void _PhysicsProcess(float delta)
    {
        // Input pour le mouvement horizontal
        Vector3 inputDirection = new Vector3();

        if (Input.IsActionPressed("ui_right"))
        {
            inputDirection.x += 1.0f;
        }
        if (Input.IsActionPressed("ui_left"))
        {
            inputDirection.x -= 1.0f;
        }
        if (Input.IsActionPressed("ui_down"))
        {
            inputDirection.z += 1.0f;
        }
        if (Input.IsActionPressed("ui_up"))
        {
            inputDirection.z -= 1.0f;
        }

        // Normaliser la direction pour éviter une vitesse plus rapide en diagonale
        if (inputDirection.Length() > 0)
        {
            inputDirection = inputDirection.Normalized();
        }

        // Appliquer la gravité
        velocity.y += gravity * delta;

        // Mouvement horizontal
        velocity.x = inputDirection.x * speed;
        velocity.z = inputDirection.z * speed;

        // Saut
        if (IsOnFloor() && Input.IsActionJustPressed("ui_accept"))
        {
            velocity.y = jumpSpeed;
        }

        // Appliquer le mouvement
        MoveAndSlide();
    }
}