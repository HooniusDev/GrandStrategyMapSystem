using Godot;
using System;
using System.Linq;
/* 
	Replaces MapUtilities.
	Takes advantage of ColorRegion.cs Resource to encapsulate things
 */

[Tool]
public partial class ColorIDMap : Node
{

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
private Image colorIdImage;
private Image colorIdImageCopy;
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
private Image bgImage;
[Export] private Godot.Collections.Array<Color> specialColors;

[Export] private Godot.Collections.Array<ColorRegion> colorsRegions;


[ExportCategory("Tool")]
[Export] private bool runSplitter = false;
[Export] private bool runClear = false;
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
				//createMaskedImages();
				//cropMaskedImages();
				//updateTerritories();
				//map.colors = colors;
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

	private void clear()
	{
		colorsRegions = new();
/* 		masks = new();
		backgrounds = new();
		offsets = new(); */

		colorIdImageCopy = null;
	}

	private void preProcess()
	{
		GD.Print("PreProcessing..");

		bgImage = backgroundTexture.GetImage();
		// Duplicate colorId
		colorIdImage = colorIdTexture.GetImage();
		colorIdImageCopy = colorIdImage.Duplicate() as Image;

		Color prevColor = new Color();

		for ( int y = 0; y < colorIdImage.GetHeight(); y++ )
		{
			for ( int x = 0; x < colorIdImage.GetWidth(); x++ )
			{
				Color color = colorIdImage.GetPixel( x, y );

				// New and not special color
				if ( getId( color ) < 0 && !isSpecialColor(color) )
				{
					var region = new ColorRegion(color, colorsRegions.Count, new Vector2I(x,y), colorIdImage.GetSize());
					region.SetBit( x,y );
					colorsRegions.Add(region);
				}
				else
				{
					// Add the color to region
					if (isSpecialColor(color))
					{
						int id = getId(prevColor);
						colorsRegions[id].SetBit( x,y );
					}
					else
					{
						int id = getId(color);
						colorsRegions[id].SetBit( x,y );
					}
				}

				prevColor = color;
			}
		}
		GD.Print($"Found {colorsRegions.Count} Color regions.");

	}

		// Creates masked image per colorID
	private void createMaskedImages()
	{
/* 		GD.Print("creating Masked Images..");
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
				 
				masks[id].SetPixel(x,y, Colors.White);
				Color bgPixel = bgImage.GetPixel(x,y);
				backgrounds[id].SetPixel(x,y, bgPixel);
			}
		}
*/
		
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

	private int getId( Color color )
	{
		if (color.A < 0.01)
			return -1;

		for ( int index = 0; index < colorsRegions.Count; index++)
		{
			if (colorsRegions[index].Color.IsEqualApprox(color))
				return index;
		}
		return -1;
	}

    private bool isSpecialColor( Color color )
    {
        if (specialColors.Count == 0 || color.A < .01f )
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

	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new Godot.Collections.Array<string>();

		if ( !IsInstanceValid(colorIdTexture) || !IsInstanceValid(backgroundTexture))
		{
			warnings.Add("ColorId Texture and/or Background Texture not set.");
			return warnings.ToArray();
		}

		if (colorIdTexture.GetSize() != backgroundTexture.GetSize())
		{
			//GD.PrintErr( "ColorId Texture and Background Texture must be same size" );
            warnings.Add("ColorId Texture and Background Texture must be same size");
		}
		return warnings.ToArray();
	}


}
