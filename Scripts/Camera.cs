using Godot; // Importation du module Godot pour utiliser les classes spécifiques du moteur de jeu Godot.
using System;

namespace MedievalGame.Scripts
{
    public class CameraController : Camera3D
    {
        // Paramètres pour le comportement de la caméra
        private Vector3 offset = new Vector3(0, 2, -5); // Décalage initial de la caméra par rapport au joueur
        private float rotationSpeed = 0.05f; // Vitesse de rotation de la caméra
        private float zoomSpeed = 0.1f; // Vitesse de zoom de la caméra
        private float minZoom = 2.0f; // Distance minimale de zoom
        private float maxZoom = 10.0f; // Distance maximale de zoom

        public override void _Ready()
        {
            // S'assure que la caméra est définie comme caméra active
            Current = true;
        }

        public override void _Process(float delta)
        {
            // Récupère l'entrée utilisateur pour la rotation de la caméra
            float rotateHorizontal = Input.GetActionStrength("mouse_motion_x") * -rotationSpeed; // Rotation horizontale selon l'entrée de la souris
            float rotateVertical = Input.GetActionStrength("mouse_motion_y") * -rotationSpeed; // Rotation verticale selon l'entrée de la souris

            // Rotation horizontale de la caméra autour du joueur
            Transform parentTransform = GetParent().Transform; // Obtient la transformation de l'objet parent (supposé être le joueur)
            parentTransform.Basis = parentTransform.Basis.Rotated(Basis.VectorUp, rotateHorizontal); // Applique la rotation horizontale
            GetParent().Transform = parentTransform; // Met à jour la transformation du parent

            // Rotation verticale de la caméra en limitant l'inclinaison pour éviter un retournement
            Basis cameraBasis = Basis.Rotated(Basis.VectorRight, rotateVertical); // Applique la rotation verticale à la caméra
            cameraBasis = LimitPitch(cameraBasis); // Limite l'angle d'inclinaison vertical de la caméra

            // Applique la rotation verticale calculée à la caméra
            Transform = new Transform3D(cameraBasis, GlobalTransform.origin); // Met à jour la transformation globale de la caméra

            // Gestion du zoom de la caméra
            float zoomInput = Input.GetActionStrength("ui_zoom_in") - Input.GetActionStrength("ui_zoom_out"); // Calcul de l'entrée de zoom (entrée utilisateur)
            if (zoomInput != 0)
            {
                offset.Length = Mathf.Clamp(offset.Length() - zoomInput * zoomSpeed, minZoom, maxZoom); // Ajuste la distance de l'offset selon l'entrée utilisateur
            }

            // Mise à jour de la position de la caméra par rapport au joueur
            GlobalTransform.origin = GetParent().GlobalTransform.origin + parentTransform.Basis.Xform(offset); // Applique l'offset calculé au parent
        }

        private Basis LimitPitch(Basis basis)
        {
            // Limite l'angle d'inclinaison de la caméra pour éviter qu'elle ne se retourne
            float pitchAngle = Mathf.Atan2(basis.Z.y, Vector3.Right.Dot(basis.Z)); // Calcule l'angle d'inclinaison actuel
            pitchAngle = Mathf.Clamp(pitchAngle, -Mathf.Pi / 2 + 0.1f, Mathf.Pi / 2 - 0.1f); // Limite l'angle entre -90° et 90° (avec une petite marge)
            return Basis.CreateFromEuler(new Vector3(-pitchAngle, basis.GetEuler().y, 0)); // Renvoie une nouvelle base limitée en inclinaison
        }
    }
}
