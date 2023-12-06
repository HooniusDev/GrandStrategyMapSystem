using Godot;
using System;

public partial class Input : StaticBody2D
{

	[Export] GSMap map;

	[Export] int hoverId;

	[Export] Material HOVER_MATERIAL;

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
		if (@event is InputEventMouseMotion)
		{
			//base._InputEvent(viewport, @event, shapeIdx);
			int newHoverId = map.getId(Position);
			GD.Print($"new hover id: {newHoverId}");
			if (newHoverId == hoverId)
				return;
			//	#GameEvents.hover_territory_changed.emit( new_hover_id )
			//print( new_hover_id )
			Territory enterHover = map.GetTerritoryAtMouse(); //world_map.get_territory_by_id(new_hover_id)
			GD.Print($"Enter Hover: {enterHover.Name}");
			Sprite2D hoverSprite = GetNode<Sprite2D>("TerritoryHover");
			hoverSprite.Position = enterHover.Position;

			hoverSprite.Texture = enterHover.GetMask();
/* 			GD.Print(enterHover.Name);
			if ( IsInstanceValid( enterHover ))
			{
				enterHover.GetNode<Sprite2D>("Mask").Visible = true;
				enterHover.GetNode<Sprite2D>("Mask").Material = HOVER_MATERIAL;
				enterHover.GetNode<Sprite2D>("Mask").ZIndex = 10;
			}
			Territory exitHover = map.GetTerritory(hoverId); //world_map.get_territory_by_id(new_hover_id)
			if ( IsInstanceValid( exitHover ))
			{
				exitHover.GetNode<Sprite2D>("Mask").Visible = false;
				exitHover.GetNode<Sprite2D>("Mask").Material = null;
				exitHover.GetNode<Sprite2D>("Mask").ZIndex = 1;
			} */
			hoverId = newHoverId;
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
