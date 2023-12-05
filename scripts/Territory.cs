using Godot;
using System;
[GlobalClass, Tool]
public partial class Territory : Node2D
{

	[Export] private TerritoryData data;
	[Export] private ColorRegion colorRegion;
	[Export] private Sprite2D mask;
	[Export] private Sprite2D bg;

	[Export] private bool toggleMask = false;
	[Export] private bool toggleBg = false;


	// Called when the node enters the scene tree for the first time.
	public void Create( int id, Image mask, Image bg, Color color, Vector2I offset )
	{
		GD.Print($"{Name} Created");
		data = new( id, mask, bg, color );
		this.mask = new Sprite2D();
		//this.mask = GetNode<Sprite2D>("Mask");
		this.mask.Texture = ImageTexture.CreateFromImage( mask );
		this.mask.Name = "Mask";
		this.mask.Centered = false;
		AddChild(this.mask);
		this.mask.Owner = GetTree().EditedSceneRoot;
		this.bg = new Sprite2D();
		//this.bg = GetNode<Sprite2D>("Bg");
		this.bg.Texture = ImageTexture.CreateFromImage( bg );
		this.bg.Name = "Bg";
		this.bg.Centered = false;
		AddChild(this.bg);
		this.bg.Owner = GetTree().EditedSceneRoot;
		Position = offset;
		SetMeta("_edit_group_", true);
	}

	public void Create( ColorRegion colorRegion )
	{
		GD.Print("color region territory created");
		this.colorRegion = colorRegion;
		Position = colorRegion.GetPosition();
		this.mask = new Sprite2D();
		//this.mask = GetNode<Sprite2D>("Mask");
		this.mask.Texture = colorRegion.GetMask();
		this.mask.Name = "Mask";
		this.mask.Centered = false;
		AddChild(this.mask);
		this.mask.Owner = GetTree().EditedSceneRoot;
	}

	public ImageTexture GetMask()
	{
		return colorRegion.GetMask();
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
		//GD.Print($"{Name} Updating.");
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
