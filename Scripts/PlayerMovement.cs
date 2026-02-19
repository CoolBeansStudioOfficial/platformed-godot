using Godot;
using System;

public partial class PlayerMovement : CharacterBody2D
{
    [Export] ShapeCast2D wallCastLeft;
    [Export] ShapeCast2D wallCastRight;
    [Export] float acceleration;
    [Export] float deceleration;
    [Export] float airMultiplier;
    [Export] float maxSpeed;
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
				wallCastLeft.ForceShapecastUpdate();
				wallCastRight.ForceShapecastUpdate();

                if (wallCastLeft.IsColliding())
				{
                    velocity.Y = jumpVelocity;
					velocity.X = walljumpVelocity;
					moveLock = walljumpMoveLock;
                }

                if (wallCastRight.IsColliding())
                {
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
				//accelerate in the direction the player pressed
                velocity.X += direction.X * acceleration * (float)delta;
            }
            else
            {
				//decelerate
				if (IsOnFloor())
				{
                    velocity.X = Mathf.MoveToward(velocity.X, 0f, deceleration * (float)delta);
                }
				else
				{
                    velocity.X = Mathf.MoveToward(velocity.X, 0f, deceleration * (float)delta * airMultiplier);
                }
					
            }
        }
		else
		{
            if (moveLock > 0f) moveLock -= (float)delta;
        }

		//speed cap
		velocity.X = Mathf.Clamp(velocity.X, -maxSpeed, maxSpeed);

		Velocity = velocity;
		MoveAndSlide();
	}
}
