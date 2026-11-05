class_name Segment
extends Sprite2D

var cell: Vector2i: set = _set_cell
var direction: Vector2i = Vector2i.LEFT: set = _set_direction
var board: GameBoard

const ROTATION := {
   Vector2i.LEFT: 0.0, Vector2i.RIGHT: 0.0,
   Vector2i.UP: PI / 2, Vector2i.DOWN: 3 * PI / 2,
}
const FLIP := {
   Vector2i.LEFT: false, Vector2i.RIGHT: true,
   Vector2i.UP: false, Vector2i.DOWN: false,
}

func _init(p_cell: Vector2i, p_texture: Texture2D, p_board: GameBoard) -> void:
	texture = p_texture
	board = p_board
	_fit_to_cell()
	cell = p_cell

func _fit_to_cell() -> void:
	if texture == null:
		return
	var native: Vector2 = texture.get_size()
	scale = Vector2.ONE * (float(board.cell_size) / native.x)

func move_segment(other: Segment) -> void: 
	cell = other.cell
	direction= other.direction

func _set_cell(p_cell: Vector2i) -> void: 
	cell = p_cell
	position = board.grid_to_world(cell)

func _set_direction(p_direction) -> void: 
	direction = p_direction
	rotation = ROTATION[p_direction]
	flip_h = FLIP[p_direction]
