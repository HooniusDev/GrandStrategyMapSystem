using Godot;
using System;
[Tool]
public partial class TerritoryData : Resource
{

    [Export] public int ID { get; private set; }
    [Export] public Image Mask { get; private set; }
    [Export] public Image Bg { get; private set; }
    [Export] public Vector2I Offset { get; private set; }
    [Export] private Color Color;
    public TerritoryData()
    {
        ID = -1;
        Offset = Vector2I.Zero;
    }
    public TerritoryData( int id, Image mask, Image bg, Color color, Vector2I offset )
    {
        ID = id;
        Mask = mask;
        Bg = bg;
        Color = color;
        Offset = offset; 
        GD.Print( $"TerritoryData {ID} Created" );
    }

}
