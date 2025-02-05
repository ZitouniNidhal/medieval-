
using Godot;

public partial class Mage : Node3D
{
    private Vector3 _velocity = new Vector3(0, 0, 0); // Vitesse de déplacement
    private float _speed = 5.0f; // Vitesse de déplacement du Mage

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Mage is ready!");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        HandleMovement(delta);
    }

    private void HandleMovement(double delta)
    {
        _velocity = new Vector3();

        if (Input.IsActionPressed("ui_right"))
            _velocity.x += 1;
        if (Input.IsActionPressed("ui_left"))
            _velocity.x -= 1;
        if (Input.IsActionPressed("ui_up"))
            _velocity.z -= 1;
        if (Input.IsActionPressed("ui_down"))
            _velocity.z += 1;

        _velocity = _velocity.Normalized() * _speed;
        Translation += _velocity * (float)delta;
    }
}
