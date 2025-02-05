using Godot;
using System;

public partial class NewScript : Node
{
    private Vector2 _velocity = new Vector2(100, 0); // Vitesse de déplacement

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Node is ready!");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Déplacer le nœud chaque frame
        Position += _velocity * (float)delta;
        var newPosition = Position + _velocity * (float)delta;
        SetPosition(newPosition);
    }
}
