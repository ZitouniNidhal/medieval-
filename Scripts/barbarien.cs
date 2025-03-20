using Godot;
using System;

namespace MedievalGame.Scripts
{
    public class BarbarianMovement : KinematicBody
    {
        // Paramètres de mouvement
        private Vector3 velocity = Vector3.Zero; // Vitesse actuelle du barbare
        [Export] public float Speed = 8.0f; // Vitesse de déplacement (exportable dans l'éditeur)
        [Export] public float JumpSpeed = 15.0f; // Force de saut (exportable dans l'éditeur)
        [Export] public float Gravity = -9.81f; // Gravité (exportable dans l'éditeur)

        // Paramètres d'attaque
        [Export] public float AttackRange = 2.0f; // Portée de l'attaque (exportable dans l'éditeur)
        [Export] public float AttackDamage = 10.0f; // Dégâts infligés par l'attaque (exportable dans l'éditeur)
        private bool isAttacking = false; // Indique si le barbare est en train d'attaquer

        // Référence à l'AnimationPlayer
        private AnimationPlayer animPlayer;

        public override void _Ready()
        {
            // Initialiser les références aux nœuds enfants
            animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            if (animPlayer == null)
            {
                GD.PrintErr("Aucun AnimationPlayer trouvé dans la scène.");
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            HandleMovement(delta);
            ApplyGravity(delta);
            HandleJump();
            MoveAndSlide(velocity);
        }

        private void HandleMovement(float delta)
        {
            // Input pour le mouvement horizontal
            Vector3 inputDirection = new Vector3();

            if (Input.IsActionPressed("ui_right")) inputDirection.x += 1.0f;
            if (Input.IsActionPressed("ui_left")) inputDirection.x -= 1.0f;
            if (Input.IsActionPressed("ui_down")) inputDirection.z += 1.0f;
            if (Input.IsActionPressed("ui_up")) inputDirection.z -= 1.0f;

            // Normaliser la direction pour éviter une vitesse plus rapide en diagonale
            if (inputDirection.LengthSquared() > 0)
            {
                inputDirection = inputDirection.Normalized();
            }

            // Appliquer la vitesse horizontale
            velocity.x = inputDirection.x * Speed;
            velocity.z = inputDirection.z * Speed;
        }

        private void ApplyGravity(float delta)
        {
            // Appliquer la gravité
            if (!IsOnFloor())
            {
                velocity.y += Gravity * delta;
            }
        }

        private void HandleJump()
        {
            // Saut
            if (IsOnFloor() && Input.IsActionJustPressed("ui_accept"))
            {
                velocity.y = JumpSpeed;
            }
        }

        private void HandleAttack()
        {
            // Attaque
            if (Input.IsActionJustPressed("ui_attack") && !isAttacking)
            {
                Attack();
            }
        }

        private void Attack()
        {
            // Définir que le barbare est en train d'attaquer
            isAttacking = true;

            // Jouer une animation d'attaque (si disponible)
            if (animPlayer != null)
            {
                animPlayer.Play("Attack"); // Assurez-vous que vous avez une animation nommée "Attack"
            }

            // Attendre la fin de l'animation avant de permettre une nouvelle attaque
            GetTree().CreateTimer(1.0f).Timeout += () =>
            {
                isAttacking = false; // Réinitialiser l'état d'attaque après 1 seconde
            };

            // Détecter les ennemis dans la portée de l'attaque
            DetectEnemiesInRange();
        }

        private void DetectEnemiesInRange()
        {
            // Créer un rayon ou une sphère pour détecter les ennemis
            Vector3 attackPosition = GlobalTransform.origin + Transform.basis.Z * AttackRange;
            PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;

            // Configuration de la requête de collision
            TypedArray<PhysicsShapeQueryResult> results = spaceState.IntersectRay(
                GlobalTransform.origin,
                attackPosition,
                new string[] { "Enemy" } // Filtrer par groupe "Enemy"
            );

            // Parcourir les résultats et appliquer les dégâts
            foreach (var result in results)
            {
                if (result.Collider is Node enemyNode && enemyNode.HasMethod("TakeDamage"))
                {
                    enemyNode.Call("TakeDamage", AttackDamage);
                }
            }
        }
    }
}