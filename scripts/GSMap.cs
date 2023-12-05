using Godot;
using System;

[GlobalClass, Tool]
public partial class GSMap : Node2D
{

	[Export] public Node2D Territories;
	[Export] public Godot.Collections.Array<Color> colors;

	[Export] private bool clear_territories = false;
	[Export] private bool toggleBackgrounds = false;
	[Export] private bool toggleMasks = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GD.Print("Helou");
	}
	
	public void Clear()
	{
		foreach ( Node n in Territories.GetChildren() )
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
			if (toggleBackgrounds == true)
			{
				ToggleTerritoryBgVisibility();
				toggleBackgrounds = false;
			}
				if (toggleMasks == true)
			{
				ToggleTerritoryMaskVisibility();
				toggleMasks = false;
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
		return Territories.GetChildOrNull<Territory>(id);
	}

	public Rect2 GetMapRect()
	{
		return GetNode<Sprite2D>("MapColorID").GetRect();
	}

	private void ToggleTerritoryBgVisibility()
	{
		foreach ( Territory t in Territories.GetChildren() )
		{
			t.ToggleBgVisible();
		}
	}

	private void ToggleTerritoryMaskVisibility()
	{
		foreach ( Territory t in Territories.GetChildren() )
		{
			t.ToggleMaskVisible();
		}
	}
}
