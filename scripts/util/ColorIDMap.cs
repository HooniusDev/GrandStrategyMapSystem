using Godot;
using System;
using System.Linq;
/* 
	Replaces MapUtilities.
	Takes advantage of ColorRegion.cs Resource to encapsulate things

	IMPORTANT!!!
	It takes 19 seconds to split the large map into regions
	and another 11 seconds to crop them.
	Ok..
	Removed the BitMap Mask from ColorRegion as it did not give any benefits
	Splitting still takes 19 seconds but cropping is done in 9 milliseconds
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
public Image colorIdImage;
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
 				ulong start = Time.GetTicksMsec();
				for ( int i = 0; i < colorsRegions.Count; i++ )
				{
					colorsRegions[i].cropMask(bgImage);
				}
				GD.Print($"Cropping took: { Time.GetTicksMsec() - start}");
				updateTerritories();
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

	private void updateTerritories()
	{
		GD.Print("Updating Territories..");
		// Create or update Territory
		for ( int i = 0; i < colorsRegions.Count; i++ )
		{
			Territory territory = GetParent<GSMap>().GetTerritory(i);
			if ( !IsInstanceValid(territory) )
			{
				// Territory should be able to set these itself
				territory = new Territory();
				GetParent<GSMap>().Territories.AddChild(territory);
				territory.Owner = GetTree().EditedSceneRoot;

				//GD.Print($"{territory.Name} Created.");
				territory.SetData(
					 i,
					 colorsRegions[i].Color,
					 colorsRegions[i].GetPosition(),
					 colorsRegions[i].GetMask(),
					 colorsRegions[i].GetBg()
					 );
			}
			else
			{
				territory.SetMask( colorsRegions[i].GetMask() );
				territory.SetBg( colorsRegions[i].GetBg() );
			}
		}
	}

	private void clear()
	{
		colorsRegions = new();
		colorIdImageCopy = null;
	}

	private void preProcess()
	{
		ulong start = Time.GetTicksMsec();

		bgImage = backgroundTexture.GetImage();
		// Duplicate colorId
		colorIdImage = colorIdTexture.GetImage();
		colorIdImageCopy = colorIdImage.Duplicate() as Image;

		int height = colorIdImage.GetHeight();
		int width = colorIdImage.GetWidth();

		int prevId = -1;

		for ( int y = 0; y < height; y++ )
		{
			for ( int x = 0; x < width; x++ )
			{
				Color color = colorIdImage.GetPixel( x, y );
				// Transparent -> do nothing
				if (color.A < .01f )
				{
					continue;
				}

				int id = GetId( color );
				Color bgColor = bgImage.GetPixel(x,y);
				// Same as last one
				if ( id != -1 )
				{
					colorsRegions[id].SetPixel( x,y, bgColor );
					prevId = id;
					continue;
				}

				// id = -1, so it's unknown color and not in special colors array
				// -> create a new ColorRegion for it
				if ( id < 0 && !isSpecialColor(color) )
				{
					var region = new ColorRegion(color, colorsRegions.Count, new Vector2I(x,y), colorIdImage.GetSize());
					region.SetPixel( x,y, bgColor );
					colorsRegions.Add(region);
				}
				else
				{
					// It's special color so set it to previous Id of colorsRegions
					if (isSpecialColor(color))
					{
						GD.Print($"Found special color count: {colorsRegions.Count} prevId{prevId}.");
						colorsRegions[prevId].SetPixel( x,y, bgColor );
						colorIdImageCopy.SetPixel( x,y, colorIdImage.GetPixel( x-1, y ) );
						colorsRegions[prevId].AddSpecialLocation( new Vector2I(x,y) );
						
						continue;
					}
				}
				prevId = id;
			}
		}
		GD.Print($"Found {colorsRegions.Count} Color regions.");
		GD.Print($"Preprocess took: {Time.GetTicksMsec() - start}");
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

	public int GetId( Color color )
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

	public Color GetColor( Vector2 pos )
	{
		return colorIdTexture.GetImage().GetPixel( (int) pos.X, (int) pos.Y );
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
