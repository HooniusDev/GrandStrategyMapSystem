using Godot;
using System;

[GlobalClass, Tool]
public partial class GSMap : Node2D
{

	[Export] public Node2D Territories;
	[Export] private ColorIDMap colorIdSystem;

	[Export] public Godot.Collections.Array<Color> colors;

	[ExportCategory("Tool")]
	[Export] private bool clearTerritories = false;
	[Export] private bool toggleBackgrounds = false;
	[Export] private bool toggleMasks = false;

	
	public void ClearTerritories()
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
			if (clearTerritories )
			{
				GD.Print("clear_territories");

				ClearTerritories();
				clearTerritories = false;
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
		return colorIdSystem.GetId(color);
	}

	public int getId( Vector2 pos )
	{
	Vector2 local = GetLocalMousePosition();
	Rect2 rect = GetNode<Sprite2D>("MapColorID").GetRect();
	if ( rect.HasPoint( local ) )
	{
		//GD.Print($"position {local} ");
		Color color = colorIdSystem.GetColor( local );
		return getId( color );
	}

	return -1;
	}

	public Territory GetTerritoryAtMouse( )
	{
		Vector2 local = GetLocalMousePosition();
		Color color = colorIdSystem.GetColor((Vector2I)local);
		int id = colorIdSystem.GetId( color );
		return GetTerritory(id);
	}

	public Territory GetTerritory( int id )
	{
		return Territories.GetChildOrNull<Territory>(id);
	}

	public Territory GetTerritory( Color color )
	{
		foreach (Territory t in Territories.GetChildren())
		{
			if (t.isColor(color))
				return t;
		}
		return null;
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
