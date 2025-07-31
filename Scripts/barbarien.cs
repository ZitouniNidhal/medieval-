using Godot;
using System;

namespace MedievalGame.Scripts
{
    public partial class BarbarianMovement : CharacterBody3D
    {
        // Paramètres de mouvement
        private Vector3 velocity = Vector3.Zero;
        [Export] private float speed = 8.0f;
        public float Speed
        {
            get => speed;
            set => speed = value;
        }
        [Export] public float JumpSpeed = 15.0f;
        [Export] public float Gravity = -9.81f;

        // Paramètres d'attaque
        [Export] public float AttackRange = 2.0f;
        [Export] public float AttackDamage = 10.0f;
        private bool isAttacking = false;

        // Référence à l'AnimationPlayer
        private AnimationPlayer animPlayer;

        public override void _Ready()
        {
            animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            if (animPlayer == null)
            {
                GD.PrintErr("Aucun AnimationPlayer trouvé dans la scène.");
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            HandleMovement((float)delta); // Convertir explicitement en float
            ApplyGravity((float)delta);
            HandleJump();
            HandleAttack();
            MoveAndSlide();
        }

        private void HandleMovement(float delta)
        {
            Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            Vector3 direction = new Vector3(inputDir.X, 0, inputDir.Y).Normalized();

            if (direction != Vector3.Zero)
            {
                velocity.X = direction.X * Speed;
                velocity.Z = direction.Z * Speed;
            }
            else
            {
                velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);
                velocity.Z = Mathf.MoveToward(velocity.Z, 0, Speed);
            }
        }

        private void ApplyGravity(float delta)
        {
            if (!IsOnFloor())
            {
                velocity.Y += Gravity * delta;
            }
        }

        private void HandleJump()
        {
            if (IsOnFloor() && Input.IsActionJustPressed("ui_accept"))
            {
                velocity.Y = JumpSpeed;
            }
        }

        private void HandleAttack()
        {
            if (Input.IsActionJustPressed("ui_attack") && !isAttacking)
            {
                Attack();
            }
        }

        private void Attack()
        {
            isAttacking = true;
            animPlayer?.Play("Attack");

            GetTree().CreateTimer(1.0f).Timeout += () =>
            {
                isAttacking = false;
            };

            DetectEnemiesInRange();
        }

        private void DetectEnemiesInRange()
        {
            PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
            Vector3 start = GlobalPosition;
            Vector3 end = start + -GlobalTransform.Basis.Z * AttackRange;

            var query = PhysicsRayQueryParameters3D.Create(start, end);
            query.CollisionMask = 1; // Layer par défaut
            query.HitFromInside = true;

            var result = spaceState.IntersectRay(query);

            if (result.Count > 0 && result["collider"].As<Node>() is Node enemyNode && enemyNode.IsInGroup("Enemy"))
            {
                enemyNode.Call("TakeDamage", AttackDamage);
            }
        }
    }
}