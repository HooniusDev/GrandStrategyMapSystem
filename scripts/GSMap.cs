using Godot;
using System;

[GlobalClass, Tool]
public partial class GSMap : Node2D
{

	[Export] private Node2D territories;
	[Export] public Godot.Collections.Array<Color> colors;

	[Export] private bool clear_territories = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GD.Print("Helou");
	}
	
	public void Clear()
	{
		foreach ( Node n in territories.GetChildren() )
		{
			GD.Print($"Territory {n.Name} cleared");
			n.QueueFree();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{
			if (clear_territories )
			{
				GD.Print("clear_territories");

				Clear();
				clear_territories = false;
			}
		}
	}

	private int getId( Color color )
	{
		if (color.A < 0.01)
			return -1;

		for ( int index = 0; index < colors.Count; index++)
		{
			if (colors[index].IsEqualApprox(color))
				return index;
		}
		return -1;
	}

	public int getId( Vector2 pos )
	{

	Vector2 local = GetLocalMousePosition();
	Rect2 rect = GetNode<Sprite2D>("MapColorID").GetRect();
	if ( rect.HasPoint( pos ) )
	{
		Color color = GetNode<Sprite2D>("MapColorID").Texture.GetImage().GetPixelv((Vector2I)local );
		return getId( color );
	}

	return -1;
	}

	public Territory GetTerritory( int id )
	{
		return territories.GetChildOrNull<Territory>(id);
	}

	public Rect2 GetMapRect()
	{
		return GetNode<Sprite2D>("MapColorID").GetRect();
	}
}
