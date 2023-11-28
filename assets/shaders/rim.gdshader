shader_type canvas_item;

uniform vec4 line_color : source_color = vec4(1.0);
uniform float line_thickness : hint_range(0, 10) = 1.0;

const vec2 OFFSETS[8] = {
    vec2(-1, -1), vec2(-1, 0), vec2(-1, 1),
    vec2(0, -1), vec2(0, 1),
    vec2(1, -1), vec2(1, 0), vec2(1, 1)
};

void fragment() {
    vec2 size = TEXTURE_PIXEL_SIZE * line_thickness;
    float outline_alpha = 0.0;
    vec4 color = vec4(0);

    for (int i = 0; i < OFFSETS.length(); i++) {
        vec2 coordinate_offset = UV + size * OFFSETS[i];
        if (coordinate_offset.x > 1.0 || coordinate_offset.x < 0.0 || coordinate_offset.y > 1.0 || coordinate_offset.y < 0.0) {
        } else {
            outline_alpha += texture(TEXTURE, coordinate_offset).a;
        }
    }

    outline_alpha = min(outline_alpha, 1.0);

    if (UV.x > 1.0 || UV.x < 0.0 || UV.y > 1.0 || UV.y < 0.0) {
        color = vec4(0);
    } else {
        color = texture(TEXTURE, UV);
		if (color.a < 0.1) discard;
    }

    COLOR = mix(color, line_color, outline_alpha - color.a);
}

