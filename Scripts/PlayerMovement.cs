using Godot;
using System;

public partial class PlayerMovement : CharacterBody2D
{
    [Export] RayCast2D wallCastLeft;
    [Export] RayCast2D wallCastRight;
    [Export] float speed;
    [Export] float jumpVelocity;
    [Export] float coyoteTime;
    [Export] float walljumpVelocity;
    [Export] float walljumpMoveLock;

    

	float coyoteTimeRemaining = 0f;
	float moveLock = 0f;

	public override void _Process(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
			if (coyoteTimeRemaining > 0f) coyoteTimeRemaining -= (float)delta;
		}
		else
		{
			coyoteTimeRemaining = coyoteTime;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept"))
		{
			if (!IsOnFloor())
			{
				wallCastLeft.ForceRaycastUpdate();
				wallCastRight.ForceRaycastUpdate();

                if (wallCastLeft.GetCollider() is not null)
				{
					GD.Print("walljumped off a left wall");
                    velocity.Y = jumpVelocity;
					velocity.X = walljumpVelocity;
					moveLock = walljumpMoveLock;
                }

                if (wallCastRight.GetCollider() is not null)
                {
                    GD.Print("walljumped off a right wall");
                    velocity.Y = jumpVelocity;
                    velocity.X = -walljumpVelocity;
                    moveLock = walljumpMoveLock;
                }
            }

			if (coyoteTimeRemaining > 0f)
			{
                velocity.Y = jumpVelocity;
            }
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		if (moveLock <= 0f)
		{
            if (direction != Vector2.Zero)
            {
                velocity.X = direction.X * speed;
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
            }
        }
		else
		{
            if (moveLock > 0f) moveLock -= (float)delta;
        }



			Velocity = velocity;
		MoveAndSlide();
	}
}
