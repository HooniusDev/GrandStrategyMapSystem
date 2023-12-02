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
private Texture2D colorIdTexture;
[Export] public Texture2D ColorIdTexture
{
	get { return colorIdTexture; }
	set 
	{
		colorIdTexture = value;
		UpdateConfigurationWarnings();
	}
}

private Texture2D backgroundTexture;
[Export] public Texture2D BackgroundTexture
{
	get { return backgroundTexture; }
	set 
	{
		backgroundTexture = value;
		UpdateConfigurationWarnings();
	}
}
[Export] private Godot.Collections.Array<Color> specialColors;

[ExportCategory("DEBUG")]
[Export] private Godot.Collections.Array<Color> colors;
[Export] private Godot.Collections.Array<Image> masks;
[Export] private Godot.Collections.Array<Image> backgrounds;
[Export] private Godot.Collections.Array<Vector2I> offsets;
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

		// TODO
		// Create warning if special colors are not present in image

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
		GD.Print("Updating Territories..");
		// Create or update Territory
		for ( int i = 0; i < colors.Count; i++ )
		{
			Territory territory = map.GetTerritory(i);
			var root = GetTree().EditedSceneRoot;
			if ( !IsInstanceValid(territory) )
			{
				territory = new Territory();
				territory.Name = "Territory" + i.ToString();
				territories.AddChild(territory);
				territory.Owner = GetTree().EditedSceneRoot;

				//PackedScene scene = GD.Load<PackedScene>("res://scenes/territory.tscn");
				//territory = scene.Instantiate<Territory>();
				territory.Create( i, masks[i], backgrounds[i], colors[i], offsets[i] );	
				//GD.Print($"{territory.Name} Created.");
			}
			else 
			{
				//GD.Print($"{territory.Name} Updating.");
				territory.UpdateData(i, masks[i], backgrounds[i], colors[i], offsets[i]);
				
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
				if (colorIdTexture.GetSize() != backgroundTexture.GetSize())
				{
					runSplitter = false;
				}
				GD.Print("Splitter running");
				preProcess();
				createMaskedImages();
				cropMaskedImages();
				updateTerritories();
				map.colors = colors;
				runSplitter = false;

			}

			if (runClear)
			{
				clear();
				//map.Clear();
				GD.Print("Clearing Arrays...");
				runClear = false;
			}
		}
	}

	// Creates masked image per colorID
	private void createMaskedImages()
	{
		GD.Print("creating Masked Images..");
		for ( int y = 0; y < colorIdImage.GetHeight(); y++ )
		{
			for ( int x = 0; x < colorIdImage.GetWidth(); x++ )
			{
				Color color = colorIdImage.GetPixel(x,y);

				if (isTransparent(color)) continue;

				for ( int i = 0; i < specialColors.Count; i++)
				{
					if ( isSpecialColor(color) )
					{
						// Replace special color by pixel on the right side
						color = colorIdImage.GetPixel( x + 1, y );
						colorIdImageCopy.SetPixel(x,y, color);
						// Mask to white
						masks[i].SetPixel(x,y, Colors.White);
						// Background 
						Color bg = bgImage.GetPixel(x,y);
						backgrounds[i].SetPixel(x,y,bg);
						continue;
					}
				} 

				int id = getId(color);
				/* 
				if (id == -1)
					GD.Print( $"{x}, {y} id: {id}" );
				 */
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

			int left = currentMask.GetWidth();
			int top = currentMask.GetHeight();
			int right = 0;
			int bottom = 0;

			for ( int y = 0; y < currentMask.GetHeight(); y++ )
			{
				for ( int x = 0; x < currentMask.GetWidth(); x++ )
				{
					Color color = currentMask.GetPixel(x,y);
					if ( color.IsEqualApprox(Colors.White) )
					{
						left = (int) MathF.Min( left, x );
						right = (int) MathF.Max( right, x );
						top = (int) MathF.Min( top, y );
						bottom = (int) MathF.Max( bottom, y );
					}
				}
			}
			GD.Print( $"territory mask {i} width: {right - left} height: {bottom - top}"  );
			// Expand the cropped image by 1 pixel respecting the image bounds
			left = (int) MathF.Max( 0, left - 1 );
			top = (int) MathF.Max( 0, top - 1 );
			right = (int) MathF.Min( currentMask.GetWidth() , right + 2 );
			bottom = (int) MathF.Min( currentMask.GetHeight() , bottom + 2 );

			int width =  right - left;
			int height = bottom - top;
			Rect2I targetRect = new Rect2I( (int) left, (int) top, (int) width, (int) height );
			GD.Print( $"territory mask {i} rect: {targetRect}" );
			// Mask
			if (targetRect.Size.Y == 0) 
			{
				GD.PrintErr("Width 0");
				continue;
			}
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

	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new Godot.Collections.Array<string>();
		if (colorIdTexture.GetSize() != backgroundTexture.GetSize())
		{
			//GD.PrintErr( "ColorId Texture and Background Texture must be same size" );
            warnings.Add("ColorId Texture and Background Texture must be same size");
		}
		return warnings.ToArray();
	}
}