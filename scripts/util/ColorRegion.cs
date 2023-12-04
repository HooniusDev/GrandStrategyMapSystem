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

    [Export] private Bitmap bitmap;

    [Export] private Image bg;

    public void SetBit( int x, int y, bool add = true )
    {
        bitmap.SetBit( x,y, add );
        rect = rect.Expand( new Vector2I(x,y) );
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
    }

    // Set Pixel should update the rect


}
