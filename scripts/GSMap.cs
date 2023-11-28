using Godot;
using System;

[GlobalClass]
public partial class GSMap : Node2D
{

	[Export] private Node2D territories;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GD.Print("Helou");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Territory GetTerritory( int id )
	{
		if ( id > GetNode("Territories").GetChildCount() )
		{
			GD.PrintErr($"Index {id} on GetTerritory out of bounds..");
			return null;
		}
		return GetNode("Territories").GetChild(id) as Territory;
	}
}
