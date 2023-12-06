using Godot;
using System;
[GlobalClass, Tool]
public partial class Territory : Node2D
{

	[Export] public int Id { get; private set; } = -1;
	[Export] public Color ColorId { get; private set; } = Colors.Transparent;

	[Export] private TerritoryData data;

	[Export] private Sprite2D mask { get; set; }
	[Export] private Sprite2D bg { get; set; }

	[ExportCategory("Tool")]
	[Export] private bool toggleMask = false;
	[Export] private bool toggleBg = false;

/// <summary>
/// If needed creates Sprite2D nodes for mask and bg Texture2D's.
/// Raises error if 'Id' or 'ColorId' are different from
/// provided ones.
/// </summary>
	public void SetData( int id, Color colorId, Vector2I position, 
							Texture2D maskTexture, Texture2D bgTexture )
	{
		//Create the Sprite2D Nodes if First Call
		if (!IsInstanceValid(mask))
		{
			mask = new Sprite2D
			{
				Centered = false,
				Visible = false,
				Name = "Mask"
			};
			AddChild(mask);
			mask.Owner = GetTree().EditedSceneRoot; // Editor Scripts need this line
		}

		if (!IsInstanceValid(bg))
			{
			bg = new Sprite2D
			{
				Centered = false,
				Visible = false,
				Name = "Bg"
			};
			AddChild(bg);
			bg.Owner = GetTree().EditedSceneRoot;
		}

		mask.Texture = maskTexture;
		bg.Texture = bgTexture;

		if ( Id != -1 && Id != id )
		{
			 GD.PrintErr( $"{Name} SetData ID {Id} mismatches {id}!" );
		}
		if ( ColorId != Colors.Transparent && ColorId != colorId )
		{
			 GD.PrintErr( $"{Name} SetData ColorId {ColorId} mismatches {colorId}!" );
		}

		Id = id;
		ColorId = colorId;
		Position = position;

		Name = "Territory" + Id.ToString();
		SetMeta("_edit_group_", true); // Set the nodes as grouped in editor
	}
	
/// <summary>
/// Get Mask Texture2D of this Territory.
/// </summary>
/// <returns>Returns Texture2D from Mask Node</returns>
	public Texture2D GetMask()
	{
		return mask.Texture;
	}

/// <summary>
/// Checks if ColorId matches parameter color
/// </summary>
/// <param name="color"></param>
/// <returns>Returns true if colors match</returns>
	public bool isColor( Color color )
	{
		// should probably check for Alpha component 
		return ColorId.IsEqualApprox(color);
	}

	public void ToggleMaskVisible( )
	{
		mask.Visible = !mask.Visible;
	}

	public void ToggleBgVisible( )
	{
		bg.Visible = !bg.Visible;
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
