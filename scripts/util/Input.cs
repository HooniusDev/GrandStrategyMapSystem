using Godot;
using System;
using System.Linq;

public partial class Input : StaticBody2D
{

	[Export] GSMap map;

	[Export] int hoverId;

	[Export] Material HOVER_MATERIAL;

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
		if (@event is InputEventMouseMotion)
		{
			int newHoverId = map.getId(Position);

			if (newHoverId == hoverId || newHoverId == -1)
				return;

			Territory enterHover = map.GetTerritoryAtMouse(); //world_map.get_territory_by_id(new_hover_id)

			//GD.Print($"Enter Hover: {enterHover.Name}");

			Sprite2D hoverSprite = GetNode<Sprite2D>("TerritoryHover");
			hoverSprite.Position = enterHover.Position;
			hoverSprite.Texture = enterHover.GetMask();

			hoverId = newHoverId;
		}
		if ( @event is InputEventMouseButton mouseButtonEvent )
		{
			//GD.Print( $"{Name} Mouse Button Event!" ); 
			if ( mouseButtonEvent.ButtonIndex is MouseButton.Left && ! mouseButtonEvent.Pressed )
			{
				GD.Print($"Mouse left click at {mouseButtonEvent.GlobalPosition}");
				BaseUnit unit = (BaseUnit) GetTree().GetNodesInGroup("selected").First();
				if (IsInstanceValid(unit))
				{
					unit.SetMoveTarget( GetGlobalMousePosition() );
				}
			}

		}
    }

    public override void _Ready()
	{
		map = GetParent<GSMap>();

		RectangleShape2D shape = new RectangleShape2D();
		shape.Size = new Vector2I( (int) map.GetMapRect().Size.X, (int) map.GetMapRect().Size.Y );
		GetNode<CollisionShape2D>("CollisionShape2D").Shape = shape;
		GetNode<CollisionShape2D>("CollisionShape2D").Position = new Vector2( shape.Size.X / 2.0f, shape.Size.Y / 2.0f );
	}

	public override void _Process(double delta)
	{
	}
}
