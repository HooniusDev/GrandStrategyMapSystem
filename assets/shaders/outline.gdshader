shader_type canvas_item;
render_mode blend_mix;

uniform vec4  outline_color : source_color;
uniform vec4  fill_color : source_color;

void fragment()
{
	// TODO
	// Change this to darken only visible border pixels not transparent ones
    vec4 tcol = texture(TEXTURE, UV);
	
	if (tcol.a < 0.1)
	{
		discard;
	}
    else
    {
	    vec4 up = texture(TEXTURE, UV + vec2(0.0, -1.0) * TEXTURE_PIXEL_SIZE);
	    vec4 down = texture(TEXTURE, UV + vec2(0.0, 1.0) * TEXTURE_PIXEL_SIZE);
	    vec4 left = texture(TEXTURE, UV + vec2(-1.0, 0.0) * TEXTURE_PIXEL_SIZE);
	    vec4 right = texture(TEXTURE, UV + vec2(1.0, 0.0) * TEXTURE_PIXEL_SIZE);
            
		if (up.a == 1.0 && down.a == 1.0 && left.a == 1.0 && right.a == 1.0) {
        	COLOR = fill_color;
    	} else {
        COLOR = outline_color;
    	}
	}
        
}