using Godot;
using System;
[GlobalClass, Tool]
public partial class Territory : Node2D
{
	[Export] private int id;
	[Export] public Texture2D Mask;
	[Export] public Texture2D Background;

	public Territory()
	{
		
	}

	public Territory( int id )
	{
		this.id = id;
	}
	

	// Called when the node enters the scene tree for the first time.
	public override void _EnterTree()
	{
		Owner = GetTree().EditedSceneRoot;
		GD.Print($"Territory {Name} entered the play");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
