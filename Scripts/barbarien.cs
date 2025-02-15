using Godot;
using System;

namespace MedievalGame.Scripts
{
public class BarbarianMovement : KinematicBody
{
    // Paramètres de mouvement
    private Vector3 velocity = Vector3.Zero; // Vitesse actuelle du barbare
    private float speed = 8.0f; // Vitesse de déplacement
    private float jumpSpeed = 15.0f; // Force de saut
    private float gravity = -9.81f; // Gravité

    // Paramètres d'attaque
    private float attackRange = 2.0f; // Portée de l'attaque
    private float attackDamage = 10.0f; // Dégâts infligés par l'attaque
    private bool isAttacking = false; // Indique si le barbare est en train d'attaquer

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

        // Attaque
        if (Input.IsActionJustPressed("ui_attack") && !isAttacking)
        {
            Attack();
        }

        // Appliquer le mouvement
        MoveAndSlide();
    }

    private void Attack()
    {
        // Définir que le barbare est en train d'attaquer
        isAttacking = true;

        // Jouer une animation d'attaque (si disponible)
        if (GetNodeOrNull<AnimationPlayer>("AnimationPlayer") is AnimationPlayer animPlayer)
        {
            animPlayer.Play("Attack"); // Assurez-vous que vous avez une animation nommée "Attack"
        }

        // Attendre la fin de l'animation avant de permettre une nouvelle attaque
        GetTree().CreateTimer(1.0f).Timeout += () =>
        {
            isAttacking = false; // Réinitialiser l'état d'attaque après 1 seconde
        };

        // Vérifier s'il y a des ennemis dans la portée de l'attaque
        DetectEnemiesInRange();
    }

    private void DetectEnemiesInRange()
    {
        // Créer un rayon ou une sphère pour détecter les ennemis
        Vector3 attackPosition = GlobalTransform.origin + Transform.basis.Z * attackRange;
        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;

        // Configuration de la requête de collision
        TypedArray<PhysicsShapeQueryResult> results = spaceState.IntersectRay(
            GlobalTransform.origin,
            attackPosition,
            new Array<string>() { "Enemy" } // Filtrer par groupe "Enemy"
        );

        // Parcourir les résultats et appliquer les dégâts
        foreach (var result in results)
        {
            if (result.Collider is Node enemyNode && enemyNode.HasMethod("TakeDamage"))
            {
                enemyNode.Call("TakeDamage", attackDamage);
            }
        }
    }
}
}