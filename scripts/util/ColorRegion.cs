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

    [Export] private Image bg;
    [Export] private Image mask;

    public void SetPixel( int x, int y, Color color )
    {
        mask.SetPixel(x,y, Colors.White);
        bg.SetPixel(x,y, color);
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

    public Texture2D GetMask()
    {
        return ImageTexture.CreateFromImage( mask );
    }

    public Texture2D GetBg()
    {
        return ImageTexture.CreateFromImage( bg );
    }

    public void cropMask( Image source )
    {

        // Rect is created in one too little width and height
        rect.End += Vector2I.One;

        // Mask
        Image maskCropped = Image.Create( rect.Size.X, rect.Size.Y, false, Image.Format.Rgba8 );
        maskCropped.BlitRect( mask, rect, Vector2I.Zero );
        mask = maskCropped;

        // Bg
        Image bgCropped = Image.Create( rect.Size.X, rect.Size.Y, false, Image.Format.Rgba8 );
        bgCropped.BlitRect( bg, rect, Vector2I.Zero );
        bg = bgCropped;



    }

    public ColorRegion()
    {
        rect = new();
        Color = Colors.Transparent;
        Id = -1;
    }

    public ColorRegion( Color color, int id, Vector2I firstPixel, Vector2I sourceSize )
    {
        this.color = color;
        this.id = id;
        rect = new Rect2I( firstPixel, Vector2I.Zero );
        mask = Image.Create( sourceSize.X, sourceSize.Y, false, Image.Format.Rgba8 );
        bg = Image.Create( sourceSize.X, sourceSize.Y, false, Image.Format.Rgba8 );
        specialLocations = new();
    }


}
