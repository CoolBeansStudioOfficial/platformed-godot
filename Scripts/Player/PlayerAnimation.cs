using Godot;
using System;

public partial class PlayerAnimation : Sprite2D
{
    [Export] float stepTime;

    float timeToNextStep = 0f;
    bool altStep = false;
    bool facingLeft = true;
	public void UpdateAnimation(Vector2 velocity, bool grounded, double delta)
    {
        if (Mathf.Abs(velocity.X) > 0f)
        {
            if (velocity.X < 0f) facingLeft = true;
            else facingLeft = false;
        }
            

        if (grounded)
        {
            if (Mathf.Abs(velocity.X) > 0f)
            {
                timeToNextStep -= (float)delta;
                if (timeToNextStep < 0f)
                {
                    //switch steps
                    altStep = !altStep;

                    timeToNextStep = stepTime;
                }

                if (facingLeft)
                {
                    //left main step
                    if (!altStep)
                    {
                        RegionRect = new()
                        {
                            Position = new Vector2(48, 0),
                            Size = new(16, 16)
                        };
                    }
                    //left alt step
                    else
                    {
                        RegionRect = new()
                        {
                            Position = new Vector2(80, 0),
                            Size = new(16, 16)
                        };
                    }

                }
                else
                {
                    //right main step
                    if (!altStep)
                    {
                        RegionRect = new()
                        {
                            Position = new Vector2(32, 0),
                            Size = new(16, 16)
                        };
                    }
                    //right alt step
                    else
                    {
                        RegionRect = new()
                        {
                            Position = new Vector2(64, 0),
                            Size = new(16, 16)
                        };
                    }
                }
            }
            else
            {
                //left idle animation
                if (facingLeft)
                {
                    RegionRect = new()
                    {
                        Position = new Vector2(16, 0),
                        Size = new(16, 16)
                    };
                }
                //right idle animation
                else
                {
                    RegionRect = new()
                    {
                        Position = new Vector2(0, 0),
                        Size = new(16, 16)
                    };
                }
            }
            
        }
        else
        {
            //jump animation
            if (velocity.Y <= 0f)
            {
                //left jump animation
                if (facingLeft)
                {
                    RegionRect = new()
                    {
                        Position = new Vector2(112, 0),
                        Size = new(16, 16)
                    };
                }
                //right jump animation
                else
                {
                    RegionRect = new()
                    {
                        Position = new Vector2(96, 0),
                        Size = new(16, 16)
                    };
                }
            }
            //fall animation
            else
            {
                //left fall animation
                if (facingLeft)
                {
                    RegionRect = new()
                    {
                        Position = new Vector2(144, 0),
                        Size = new(16, 16)
                    };
                }
                //right fall animation
                else
                {
                    RegionRect = new()
                    {
                        Position = new Vector2(128, 0),
                        Size = new(16, 16)
                    };
                }
            }
        }
	}
}
