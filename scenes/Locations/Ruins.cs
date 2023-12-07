using Godot;
using System;
using System.Linq;

public partial class Ruins : Node2D
{

	[Export] Area2D area2D;
	private Label label;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		label.GuiInput += OnGuiInput;
		label.MouseEntered += OnMouseEntered;
		label.MouseExited += OnMouseExited;

		label.TooltipText = $"Location: {Name}";
		area2D.BodyEntered += OnBodyEntered;
		//area2D.InputEvent += OnInputEvent;
	}

    private void OnMouseExited()
    {
        label.AddThemeColorOverride( "font_outline_color",  new Color("8d8d8d") );
		//theme_override_colors/font_outline_color
    }


    private void OnMouseEntered()
    {
        label.AddThemeColorOverride( "font_outline_color",  Colors.White );
    }


    private void OnGuiInput(InputEvent @event)
    {
        if ( @event is InputEventMouseButton mouseButtonEvent )
		{
			//GD.Print( $"{Name} Mouse Button Event!" ); 
			if ( mouseButtonEvent.ButtonIndex is MouseButton.Left && ! mouseButtonEvent.Pressed )
			{
				BaseUnit unit = (BaseUnit) GetTree().GetNodesInGroup("selected").First();
				unit.SetMoveTarget( GlobalPosition );
				GD.Print($"{Name} set as target for {unit.Name}");
			}

		}
    }

    /*     private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
        {
            if ( @event is InputEventMouseButton mouseButtonEvent )
            {
                //GD.Print( $"{Name} Mouse Button Event!" ); 
                if ( mouseButtonEvent.ButtonIndex is MouseButton.Left && ! mouseButtonEvent.Pressed )
                {
                    BaseUnit unit = (BaseUnit) GetTree().GetNodesInGroup("selected").First();
                    unit.SetMoveTarget( GlobalPosition );
                    GD.Print($"{Name} set as target for {unit.Name}");
                    (viewport as Viewport).SetInputAsHandled();
                }

            }

        } */


    private void OnBodyEntered(Node2D body)
	{
		//GD.Print( $"{body.Name} entered {Name}!" );
		if (body is BaseUnit unit)
		{
			unit.OnArriverLocation(this);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
