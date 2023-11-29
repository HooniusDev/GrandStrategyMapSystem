using Godot;
using System;

[GlobalClass, Tool]
public partial class GSMap : Node2D
{

	[Export] private Node2D territories;

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

	public Territory GetTerritory( int id )
	{
		return territories.GetChildOrNull<Territory>(id);
	}
}
