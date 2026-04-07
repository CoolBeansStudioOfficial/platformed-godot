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
    bool usedJump = true;
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
            usedJump = false;

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
            bool didJump = false;

            if (!IsOnFloor())
            {
                wallCastLeft.ForceShapecastUpdate();
                wallCastRight.ForceShapecastUpdate();

                //determine whether player should be able to jump from each side
                bool canJumpFromLeft = false;
                bool canJumpFromRight = false;

                if (wallCastLeft.IsColliding())
                {
                    //don't let the player wall jump if the only thing the cast hit was stuff that should kill the player
                    for (int i = 0; i < wallCastLeft.GetCollisionCount(); i++)
                    {
                        //if collision is with scene, skip invalid object check
                        if (wallCastLeft.GetCollider(i) is not TileMapLayer)
                        {
                            canJumpFromLeft = true;
                            break;
                        }

                        var tile = LevelManager.Instance.GetTileFromCollision(wallCastLeft.GetColliderRid(i));
                        if (tile is not null)
                        {
                            if (!(bool)tile.GetCustomData("Kill On Touch"))
                            {
                                canJumpFromLeft = true;
                                break;
                            }
                        }
                    }
                }

                if (wallCastRight.IsColliding())
                {
                    //don't let the player wall jump if the only thing the cast hit was stuff that should kill the player
                    for (int i = 0; i < wallCastRight.GetCollisionCount(); i++)
                    {
                        //if collision is with scene, skip invalid object check
                        if (wallCastRight.GetCollider(i) is not TileMapLayer)
                        {
                            canJumpFromRight = true;
                            break;
                        }

                        var tile = LevelManager.Instance.GetTileFromCollision(wallCastRight.GetColliderRid(i));
                        if (tile is not null)
                        {
                            if (!(bool)tile.GetCustomData("Kill On Touch"))
                            {
                                canJumpFromRight = true;
                                break;
                            }
                        }
                    }
                }


                //if player is close enough to wall jump off of either wall, pick the closest wall
                if (canJumpFromLeft && canJumpFromRight)
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
                    didJump = true;
                    AudioManager.Instance.PlayStream(jumpSound);
                }
                //otherwise, jump off of whichever wall is elligible
                else if (canJumpFromLeft)
                {
                    velocity.Y = jumpVelocity;
                    velocity.X = walljumpVelocity;
                    moveLock = walljumpMoveLock;

                    jumpBufferTimeRemaining = 0f;
                    didJump = true;
                    AudioManager.Instance.PlayStream(jumpSound);
                }
                else if (canJumpFromRight)
                {
                    velocity.Y = jumpVelocity;
                    velocity.X = -walljumpVelocity;
                    moveLock = walljumpMoveLock;

                    jumpBufferTimeRemaining = 0f;
                    didJump = true;
                    AudioManager.Instance.PlayStream(jumpSound);
                }
            }

            if (coyoteTimeRemaining > 0f && !didJump && !usedJump)
            {
                velocity.Y = jumpVelocity;

                jumpBufferTimeRemaining = 0f;
                didJump = true;
                usedJump = true;
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
                var move = velocity.X + (direction.X * acceleration * (float)delta);

                //accelerate in the direction the player pressed if they haven't reached the speed cap yet or it is a slowdown
                if (Mathf.Abs(move) <= maxSpeed || Mathf.Abs(move) < Math.Abs(velocity.X))
                {
                    velocity.X = move;
                }
                
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

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            HandleCollision(GetSlideCollision(i));
        }

        


    }

    void HandleCollision(KinematicCollision2D collision)
    {
        if (collision is not null && collision.GetCollider() is TileMapLayer)
        {
            TileData tile = LevelManager.Instance.GetTileFromCollision(collision.GetColliderRid());

            if (tile is not null)
            {
                if ((bool)tile.GetCustomData("Kill On Touch"))
                {
                    LevelManager.Instance.KillPlayer();
                }

                if ((int)tile.GetCustomData("Bounce Pad Height") > 0)
                {
                    int rotation = (int)tile.GetCustomData("Rotation");
                    int bouncePadHeight = (int)tile.GetCustomData("Bounce Pad Height");

                    if (rotation == (int)TileRotation.Up) Velocity = new(Velocity.X, -bouncePadHeight);
                    else if (rotation == (int)TileRotation.Left)
                    {
                        moveLock = 0.25f;
                        bouncedSidewaysBySpring = true;
                        Velocity = new(-bouncePadHeight, Velocity.Y);
                    }
                    else if (rotation == (int)TileRotation.Down) Velocity = new(Velocity.X, bouncePadHeight);
                    else
                    {
                        moveLock = 0.25f;
                        bouncedSidewaysBySpring = true;
                        Velocity = new(bouncePadHeight, Velocity.Y);
                    }
                }
            }
        }
    }
}
