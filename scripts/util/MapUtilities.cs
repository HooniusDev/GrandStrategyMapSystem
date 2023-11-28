using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

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
private Image bgImage;
[Export] private Image colorIdImageCopy;

[ExportCategory("Tool")]
[Export] private bool runSplitter = false;
[Export] private bool runClear = false;



	private void preProcess()
	{
		GD.Print("PreProcessing..");

		bgImage = backgroundTexture.GetImage();
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

		for ( int i = 0; i < colors.Count; i++  )
		{
			masks[i] = Image.Create( colorIdImage.GetSize().X, colorIdImage.GetSize().Y, false, Image.Format.Rgba8 );
			backgrounds[i] = Image.Create( colorIdImage.GetSize().X, colorIdImage.GetSize().Y, false, Image.Format.Rgba8 );
		}

	}

	private void updateTerritories()
	{
		GD.Print("Update Territories start.");
		// Create or update Territory
		for ( int i = 0; i < colors.Count; i++ )
		{
		
			Territory territory = map.GetTerritory(i);
			var root = GetTree().EditedSceneRoot;
			if ( !IsInstanceValid(territory) )
			{
				PackedScene scene = GD.Load<PackedScene>("res://scenes/territory.tscn");
				territory = scene.Instantiate<Territory>();
				territory.Name = "Territory" + i.ToString();
				territories.AddChild(territory);
				territory.Owner = GetTree().EditedSceneRoot;
			}
			else 
			{
				GD.Print($"{territory.Name} Found.");
			}
			territory.Id = i;
			territory.Position = offsets[i];
			territory.Mask.Texture = ImageTexture.CreateFromImage( masks[i] );
			territory.Background.Texture = ImageTexture.CreateFromImage( backgrounds[i] );
			
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

	private bool isTransparent(Color color)
	{
		return color.A < 0.01;
	}

	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{
			if (runSplitter && colorIdTexture != null && backgroundTexture != null)
			{
				GD.Print("Splitter running");
				preProcess();
				createMaskedImages();
				cropMaskedImages();
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

	// Creates masked image per colorID
	private void createMaskedImages()
	{
		GD.Print("createMaskedImages");
		for ( int y = 0; y < colorIdImage.GetHeight(); y++ )
		{
			for ( int x = 0; x < colorIdImage.GetWidth(); x++ )
			{
				Color color = colorIdImage.GetPixel(x,y);
				int id = getId(color);

				if (isTransparent(color)) continue;

				for ( int i = 0; i < specialColors.Count; i++)
				{
					if ( isSpecialColor(color) )
					{
						// Replace special color by pixel on the right side
						Color rPixel = colorIdImage.GetPixel( x + 1,y );
						colorIdImageCopy.SetPixel(x,y, rPixel);
						// Mask to white
						masks[i].SetPixel(x,y, Colors.White);
						// Background 
						Color bg = bgImage.GetPixel(x,y);
						backgrounds[i].SetPixel(x,y,bg);
						continue;
					}
				} 
				masks[id].SetPixel(x,y, Colors.White);
				Color bgPixel = bgImage.GetPixel(x,y);
				backgrounds[id].SetPixel(x,y, bgPixel);
			}
		}
	/*
	# Do a second pass to add transparent pixels border pixels to bg's
	for y in source.get_height():
		for x in source.get_width():
			var color = source.get_pixel( x, y )
			if color.a > 0.1:
				continue
			if x > 0: # left
				var id = get_id_by_color( source.get_pixel( x - 1, y ) )
				if id != -1: # There is a color
					backgrounds[id].set_pixel(x,y, bg_image.get_pixel( x, y))
			if x < source.get_width() - 1: #right
				var id = get_id_by_color( source.get_pixel( x + 1, y ) )
				if id != -1: # There is a color
					backgrounds[id].set_pixel(x,y, bg_image.get_pixel( x, y))
			if y > 0: # top
				var id = get_id_by_color( source.get_pixel( x, y - 1 ) )
				if id != -1: # There is a color
					backgrounds[id].set_pixel(x,y, bg_image.get_pixel( x, y))
			if y < source.get_height() - 1: # down
				var id = get_id_by_color( source.get_pixel( x, y + 1 ) )
				if id != -1: # There is a color
					backgrounds[id].set_pixel(x,y, bg_image.get_pixel( x, y))
			if x > 0 and y < source.get_height() - 1: # topleft
				var id = get_id_by_color( source.get_pixel( x - 1, y + 1 ) )
				if id != -1: # There is a color
					backgrounds[id].set_pixel(x,y, bg_image.get_pixel( x, y))
			if x > 0 and y > 0: # downleft
				var id = get_id_by_color( source.get_pixel( x - 1, y - 1 ) )
				if id != -1: # There is a color
					backgrounds[id].set_pixel(x,y, bg_image.get_pixel( x, y))
			if x < source.get_width() - 1 and y > 0: # downright
				var id = get_id_by_color( source.get_pixel( x + 1, y - 1 ) )
				if id != -1: # There is a color
					backgrounds[id].set_pixel(x,y, bg_image.get_pixel( x, y))
			if x < source.get_width() - 1 and y < source.get_height() - 1: # upright
				var id = get_id_by_color( source.get_pixel( x + 1, y + 1 ) )
				if id != -1: # There is a color
					backgrounds[id].set_pixel(x,y, bg_image.get_pixel( x, y))
	*/	
	
	}

	private void cropMaskedImages()
	{
		//## TODO: 
		//## Handle colored background image cropping
		//## Adjustable radius
		//## Multiple Layers? Someday maybe

		for ( int i = 0; i < colors.Count; i++  )
		{
			Image currentMask = masks[i];

			float left = currentMask.GetWidth();
			float top = currentMask.GetHeight();
			float right = 0;
			float bottom = 0;

			for ( int y = 0; y < currentMask.GetHeight(); y++ )
			{
				for ( int x = 0; x < currentMask.GetWidth(); x++ )
				{
					Color color = currentMask.GetPixel(x,y);
					if ( color.IsEqualApprox(Colors.White) )
					{
						left = MathF.Min( left, x );
						right = MathF.Max( right, x );
						top = MathF.Min( top, y );
						bottom = MathF.Max( bottom, y );
					}
				}
			}
			// Expand the cropped image by 1 pixel respecting the image bounds
			left = MathF.Max( 0, left - 1 );
			top = MathF.Max( 0, top - 1 );
			right = MathF.Min( currentMask.GetWidth() - 1, right + 2 );
			bottom = MathF.Min( currentMask.GetHeight() - 1, bottom + 2 );
			// NOT CREATED RIGHT FOR REGIONS AT BOTTOM
			float width =  right - left;
			float height = bottom - top;
			Rect2I targetRect = new Rect2I( (int) left, (int) top, (int) width, (int) height );

			// Mask
			Image maskCropped = Image.Create( (int) width, (int) height, false, Image.Format.Rgba8 );
			maskCropped.BlitRect( currentMask, targetRect, Vector2I.Zero );
			masks[i] = maskCropped;

			// Bg
			Image bgCropped = Image.Create( (int) width, (int) height, false, Image.Format.Rgba8 );
			bgCropped.BlitRect( backgrounds[i], targetRect, Vector2I.Zero );
			backgrounds[i] = bgCropped;

			offsets[i] = targetRect.Position;
		}
	}


}
