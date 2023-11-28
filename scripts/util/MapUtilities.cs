using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class MapUtilities : Node
{
[ExportCategory("Node References")]
[Export] public GSMap map;
[Export] private Node2D territories;

[ExportCategory("Inputs")]
[Export] private Texture2D colorIdTexture;
[Export] private Texture2D backgroundTexture;
[Export] private Godot.Collections.Array<Color> specialColors;

[ExportCategory("DEBUG")]
[Export] private Godot.Collections.Array<Color> colors;
[Export] private Godot.Collections.Array<Image> masks;
[Export] private Godot.Collections.Array<Image> backgrounds;
[Export] private Godot.Collections.Array<Vector2> offsets;
private Image colorIdImage;
[Export] private Image colorIdImageCopy;

[ExportCategory("Tool")]
[Export] private bool runSplitter = false;
[Export] private bool runClear = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	private void preProcess()
	{
		GD.Print("PreProcessing..");

		// Duplicate colorId
		colorIdImage = colorIdTexture.GetImage();
		colorIdImageCopy = colorIdImage.Duplicate() as Image;

		for ( int y = 0; y < colorIdImage.GetHeight(); y++ )
		{
			for ( int x = 0; x < colorIdImage.GetWidth(); x++ )
			{
				Color color = colorIdImage.GetPixel( x, y );

				if (color.A < .01 || isSpecialColor(color))
				{
					continue;
				}

				if ( getId( color ) < 0 )
				{
					colors.Add(color);
				}
			}
		}
		GD.Print($"Found {colors.Count} Color regions.");

		// Initialize debug arrays to the size
		masks.Resize(colors.Count);
		backgrounds.Resize(colors.Count);
		offsets.Resize(colors.Count);
	}

	private void updateTerritories()
	{
		// Create or update Territory
		for ( int i = 0; i < colors.Count; i++ )
		{
			Territory territory = GetTerritory(i);
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

	private void clear()
	{
		colors = new();
		masks = new();
		backgrounds = new();
		offsets = new();

		colorIdImageCopy = null;
	}

	private bool isSpecialColor(Color color)
	{
		if (specialColors.Count == 0)
			return false;
		foreach (Color c in specialColors)
			if (color.IsEqualApprox( c ))
				return true;
		return false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{
			if (runSplitter && colorIdTexture != null && backgroundTexture != null)
			{
				GD.Print("Splitter runing");
				preProcess();
				updateTerritories();
				runSplitter = false;

				

			}

			if (runClear)
			{
				clear();
				GD.Print("clearArrays run");
				runClear = false;
			}
		}
	}

	private Territory GetTerritory( int id )
	{
		if ( id > territories.GetChildCount() )
		{
			var scene = GD.Load<PackedScene>("res://scenes/territory.tscn");
			Territory territory = scene.Instantiate<Territory>();
			territory.Name = "Territory" + id.ToString();
			territory.Owner = GetTree().EditedSceneRoot;
			territories.AddChild(territory);
			return territory;
		}
		return territories.GetChild(id) as Territory;
	}
}
