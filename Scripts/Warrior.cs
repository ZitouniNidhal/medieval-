using Godot;
using System;

public partial class Warrior : Node3D
{
    private Vector3 _velocity = new Vector3(0, 0, 0); // Vitesse de déplacement
    private float _speed = 5.0f; // Vitesse de déplacement du Warrior
    private Camera3D _camera;
    private Vector3 _cameraOffset = new Vector3(0, 3, -5); // Position relative de la caméra

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Warrior is ready!");

        // Trouve la caméra dans la scène (assure-toi que son nom est "Camera3D")
        _camera = GetNode<Camera3D>("/root/Main/Camera3D"); // Adapte le chemin selon ta scène
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        HandleMovement(delta);
        UpdateCameraPosition();
    }

    private void HandleMovement(double delta)
    {
        _velocity = Vector3.Zero;

        if (Input.IsActionPressed("ui_right"))
            _velocity.X += 1;
        if (Input.IsActionPressed("ui_left"))
            _velocity.X -= 1;
        if (Input.IsActionPressed("ui_up"))
            _velocity.Z -= 1;
        if (Input.IsActionPressed("ui_down"))
            _velocity.Z += 1;

        if (_velocity.Length() > 0)
        {
            _velocity = _velocity.Normalized() * _speed;
            LookAt(GlobalTransform.Origin + new Vector3(_velocity.X, 0, _velocity.z), Vector3.Up);
        }

        GlobalTransform = new Transform3D(GlobalTransform.Basis, GlobalTransform.Origin + _velocity * (float)delta);
    }

    private void UpdateCameraPosition()
    {
        if (_camera != null)
        {
            // Positionne la caméra derrière et au-dessus du Warrior
            _camera.GlobalTransform = new Transform3D(_camera.GlobalTransform.Basis, GlobalTransform.Origin + _cameraOffset);
            // Oriente la caméra vers le Warrior
            _camera.LookAt(GlobalTransform.Origin, Vector3.Up);
        }
    }
}