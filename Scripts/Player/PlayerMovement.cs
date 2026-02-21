using Godot;
using System;

public partial class PlayerMovement : CharacterBody2D
{
    [Export] PlayerAnimation animation;

    [Export] ShapeCast2D wallCastLeft;
    [Export] ShapeCast2D wallCastRight;
    [Export] float acceleration;
    [Export] float deceleration;
    [Export] float airMultiplier;
    [Export] float maxSpeed;
    [Export] float jumpVelocity;
    [Export] float coyoteTime;
    [Export] float jumpBufferTime;
    [Export] float walljumpVelocity;
    [Export] public float walljumpMoveLock;

    [Export] AudioStream jumpSound;


    float coyoteTimeRemaining = 0f;
    float jumpBufferTimeRemaining = 0f;
    public float moveLock = 0f;
	public bool bouncedSidewaysBySpring = false;

	public override void _Process(double delta)
	{
		//return if game is paused
		if (UIManager.Instance.pauseMenu.Visible) return; 

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

			if (moveLock <= 0f) bouncedSidewaysBySpring = false;
		}

        // Handle Jump.
        if (Input.IsActionJustPressed("jump"))
        {
            jumpBufferTimeRemaining = jumpBufferTime;
        }
        else jumpBufferTimeRemaining -= (float)delta;

		if (jumpBufferTimeRemaining > 0f)
		{
            if (!IsOnFloor())
            {
                wallCastLeft.ForceShapecastUpdate();
                wallCastRight.ForceShapecastUpdate();

                //if player is close enough to wall jump off of either wall, pick the closest wall
                if (wallCastLeft.IsColliding() && wallCastRight.IsColliding())
                {
                    float leftWallDistance = Mathf.Abs(Position.X - wallCastLeft.GetCollisionPoint(0).X);
                    float rightWallDistance = Mathf.Abs(Position.X - wallCastRight.GetCollisionPoint(0).X);

                    if (leftWallDistance < rightWallDistance)
                    {
                        velocity.Y = jumpVelocity;
                        velocity.X = walljumpVelocity;
                        moveLock = walljumpMoveLock;
                    }
                    else
                    {
                        velocity.Y = jumpVelocity;
                        velocity.X = -walljumpVelocity;
                        moveLock = walljumpMoveLock;
                    }

                    jumpBufferTimeRemaining = 0f;
                    AudioManager.Instance.PlayStream(jumpSound);
                }
                //otherwise, jump off of whichever wall is elligible
                else if (wallCastLeft.IsColliding())
                {
                    velocity.Y = jumpVelocity;
                    velocity.X = walljumpVelocity;
                    moveLock = walljumpMoveLock;

                    jumpBufferTimeRemaining = 0f;
                    AudioManager.Instance.PlayStream(jumpSound);
                }
                else if (wallCastRight.IsColliding())
                {
                    velocity.Y = jumpVelocity;
                    velocity.X = -walljumpVelocity;
                    moveLock = walljumpMoveLock;

                    jumpBufferTimeRemaining = 0f;
                    AudioManager.Instance.PlayStream(jumpSound);
                }
            }
            else if (coyoteTimeRemaining > 0f)
            {
                velocity.Y = jumpVelocity;

                jumpBufferTimeRemaining = 0f;
                AudioManager.Instance.PlayStream(jumpSound);
            }
        }


		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("move_left", "move_right", "ui_up", "ui_down");

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

            //speed cap
			if (!bouncedSidewaysBySpring)
			{
                velocity.X = Mathf.Clamp(velocity.X, -maxSpeed, maxSpeed);
            }
            
        }
		else
		{
            if (moveLock > 0f) moveLock -= (float)delta;
        }

		Velocity = velocity;
		MoveAndSlide();

		animation.UpdateAnimation(velocity, IsOnFloor(), delta);
	}
}
