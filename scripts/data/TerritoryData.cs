using Godot;
using System;
[Tool]
public partial class TerritoryData : Resource
{

    [Export] public int ID { get; private set; }
    [Export] public Image Mask { get; private set; }
    [Export] public Image Bg { get; private set; }
    [Export] public Color Color{ get; private set; }

    public TerritoryData()
    {
        ID = -1;
    }
    public TerritoryData( int id, Image mask, Image bg, Color color)
    {
        ID = id;
        Mask = mask;
        Bg = bg;
        Color = color;
        GD.Print( $"TerritoryData {ID} Created" );
    }

    	public void Update( int id, Image mask, Image bg, Color color )
	{
		if (ID != id )
		{
			GD.PrintErr( $"ID mismatch on {ID}.UpdateData()" );
		}
		if ( !Color.IsEqualApprox(color) )
		{
			GD.PrintErr( $"Color mismatch on {ID}.UpdateData()" );
		}
        Mask = mask;
        Bg = bg;
	}
}
