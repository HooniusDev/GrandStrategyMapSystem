using Godot;
using System;
[GlobalClass, Tool]
public partial class Territory : Node2D
{

	[Export] private TerritoryData data;
	[Export] private Sprite2D mask;
	[Export] private Sprite2D bg;

	[Export] private bool toggleMask = false;
	[Export] private bool toggleBg = false;


	// Called when the node enters the scene tree for the first time.
	public void Create( int id, Image mask, Image bg, Color color, Vector2I offset )
	{
		GD.Print($"{Name} entered the play");
		data = new( id, mask, bg, color );
		this.mask = GetNode<Sprite2D>("Mask");
		this.mask.Texture = ImageTexture.CreateFromImage( mask );
		this.bg = GetNode<Sprite2D>("Bg");
		this.bg.Texture = ImageTexture.CreateFromImage( bg );
		Position = offset;
	}



	public void ToggleMaskVisible( )
	{
		mask.Visible = !mask.Visible;
	}

	public void ToggleBgVisible( )
	{
		bg.Visible = !bg.Visible;
	}

	public void UpdateData( int id, Image mask, Image bg, Color color, Vector2I offset )
	{
		data.Update(id, mask, bg, color);
		Position = offset;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{
			if (toggleMask)
			{
				ToggleMaskVisible();
				toggleMask = false;
			}
			if (toggleBg)
			{
				ToggleBgVisible();
				toggleBg = false;
			}
		}
	}
}
