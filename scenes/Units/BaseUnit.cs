using Godot;

public partial class BaseUnit : CharacterBody2D
{
	public const float Speed = 40.0f;

	[Export] Vector2 moveTarget;

	public override void _Ready()
	{
		moveTarget = Position;
	}

	public void SetMoveTarget( Vector2 target )
	{
		moveTarget = target;
		
	}

	public void OnArriverLocation( Ruins ruins )
	{
		GD.Print( $"I, {Name} have arrived at {ruins.Name}." );
	}

    public override void _Process(double delta)
	{
		if ( Position.DistanceTo( moveTarget ) > 5.0f )
		{
			Position = Position.MoveToward( moveTarget, (float) delta * Speed );
		}

	}
}
