using Godot;
using System;
using System.Collections;
using System.Net;

/* 
    Resource class to manage color region of ColorIDMap
    Encapsulates things like  
    Mask
    Bg 
    Offsets
    that were used in MapUtilities.cs

    Dont yet know how this relates to TerritoryData Resource as they 
    are pretty similar.
    Maybe this will be more of a editor side that can manage TerritoryData
    objects images etc.. 

 */


[Tool]
public partial class ColorRegion : Resource
{

    [Export] private Rect2I rect;
    private Color color;
    [Export]public Color Color
    {
        get { return color; }
        set { color = value; }
    }
    private int id;
    [Export]public int Id
    {
        get { return id; }
        set { id = value; }
    }

    [Export] private Godot.Collections.Array<Vector2I> specialLocations;

    [Export] private Bitmap bitmap;
    //[Export] private Bitmap croppedBitmap;
    [Export] private Image bg;
    [Export] private Image mask;

    public void SetBit( int x, int y, bool add = true )
    {
        bitmap.SetBit( x,y, add );
        rect = rect.Expand( new Vector2I(x,y) );
    }

    public void AddSpecialLocation( Vector2I pos )
    {
        specialLocations.Add( pos );
    }

    public Vector2I GetPosition()
    {
        return rect.Position;
    } 

    public ImageTexture GetMask()
    {
        return ImageTexture.CreateFromImage( mask );
    }

    public void cropMask( Image source )
    {
        Bitmap croppedBitmap = new();
        Vector2I end = rect.End;
        rect.End = end + Vector2I.One;
        croppedBitmap.Create( rect.Size );

        bg = new Image();
        bg = Image.Create( rect.Size.X, rect.Size.Y, false, Image.Format.Rgba8 );
        mask = new Image();
        mask = Image.Create( rect.Size.X, rect.Size.Y, false, Image.Format.Rgba8 );

        for ( int y = 0; y < bitmap.GetSize().Y; y++ )
		{
			for ( int x = 0; x < bitmap.GetSize().X; x++ )
            {
                var sourceBit = bitmap.GetBit(x,y);
                
                if (rect.HasPoint( new Vector2I( x,y )))
                {
                    croppedBitmap.SetBitv( new Vector2I(x,y) -  rect.Position, sourceBit );
                    if ( sourceBit )
                    {
                        var sourcePixel = source.GetPixel(x,y);
                        bg.SetPixelv( new Vector2I(x,y) -  rect.Position , sourcePixel);
                        mask.SetPixelv( new Vector2I(x,y) -  rect.Position , Colors.White);
                    }
                    else
                    {
                        mask.SetPixelv( new Vector2I(x,y) -  rect.Position , Colors.Transparent);
                    }
                }

            }
        }
        bitmap = croppedBitmap;
    }

    public ColorRegion()
    {
        GD.Print("ColorRegion parametereless constructor");
        rect = new();
        Color = Colors.Transparent;
        Id = -1;
    }

    public ColorRegion( Color color, int id, Vector2I firstPixel, Vector2I sourceSize )
    {
        GD.Print("ColorRegion constructor");
        this.color = color;
        this.id = id;
        bitmap = new();
        bitmap.Create( sourceSize );
        rect = new Rect2I( firstPixel, Vector2I.Zero );
        specialLocations = new();
    }


}
