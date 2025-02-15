using Godot;
using System;

public class CameraController : Camera
{
    // Parameters for camera behavior
    private Vector3 offset = new Vector3(0, 2, -5); // Initial camera offset from the player
    private float rotationSpeed = 0.05f; // Speed of camera rotation
    private float zoomSpeed = 0.1f; // Speed of camera zoom
    private float minZoom = 2.0f; // Minimum zoom distance
    private float maxZoom = 10.0f; // Maximum zoom distance

    public override void _Ready()
    {
        // Ensure the camera is set as the current active camera
        Current = true;
    }

    public override void _Process(float delta)
    {
        // Get input for camera rotation
        float rotateHorizontal = Input.GetActionStrength("mouse_motion_x") * -rotationSpeed;
        float rotateVertical = Input.GetActionStrength("mouse_motion_y") * -rotationSpeed;

        // Rotate the camera around the player horizontally
        Transform parentTransform = GetParent().Transform;
        parentTransform.Basis = parentTransform.Basis.Rotated(Basis.VectorUp, rotateHorizontal);
        GetParent().Transform = parentTransform;

        // Rotate the camera vertically (limit the pitch to avoid flipping)
        Basis cameraBasis = Basis.Rotated(Basis.VectorRight, rotateVertical);
        cameraBasis = LimitPitch(cameraBasis);

        // Apply the vertical rotation to the camera
        Transform = new Transform(cameraBasis, GlobalTransform.origin);

        // Handle zooming
        float zoomInput = Input.GetActionStrength("ui_zoom_in") - Input.GetActionStrength("ui_zoom_out");
        if (zoomInput != 0)
        {
            offset.Length = Mathf.Clamp(offset.Length - zoomInput * zoomSpeed, minZoom, maxZoom);
        }

        // Update the camera position relative to the player
        GlobalTransform.origin = GetParent().GlobalTransform.origin + parentTransform.Basis.Xform(offset);
    }

    private Basis LimitPitch(Basis basis)
    {
        // Limit the pitch angle to prevent the camera from flipping upside down
        float pitchAngle = Mathf.Atan2(basis.Z.y, Vector3.Right.Dot(basis.Z));
        pitchAngle = Mathf.Clamp(pitchAngle, -Mathf.Pi / 2 + 0.1f, Mathf.Pi / 2 - 0.1f);
        return Basis.CreateFromEuler(new Vector3(-pitchAngle, basis.GetEuler().y, 0));
    }
}