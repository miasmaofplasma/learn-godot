@tool
extends Node2D
class_name GameBoard

const INVALID_POS = Vector2(-1, -1)

@export var cell_size: int = 60 : set = set_cell_size
@export var grid_width: int = 20 : set = set_grid_width
@export var grid_height: int = 20: set = set_grid_height

signal game_won()

var snake_cells: Array[Vector2i]

func _draw():
	var max_width = grid_width * cell_size
	var max_height = grid_height * cell_size

	var line_color = Color(1, 1, 1, .5)
	var line_thickness = 1.0

	for x in range(grid_width +1):
		var x_pos = x * cell_size
		var start_point = Vector2(x_pos, 0)
		var end_point = Vector2(x_pos, max_height)
		draw_line(start_point, end_point, line_color, line_thickness)

	for y in range(grid_width +1):
		var y_pos = y * cell_size
		var start_point = Vector2(0, y_pos)
		var end_point = Vector2(max_width, y_pos)
		draw_line(start_point, end_point, line_color, line_thickness)
	
func random_free_cell() -> Vector2i:
	var free_cells = _free_cells()
	var rand_i = randi() % free_cells.size()
	return free_cells[rand_i]

func grid_to_world(grid_pos: Vector2i) -> Vector2:
	if out_of_world(grid_pos):
		return INVALID_POS
	return Vector2(grid_pos * cell_size) + Vector2(cell_size / 2.0, cell_size / 2.0)

func set_cell_size(val):
	cell_size = val
	queue_redraw()

func set_grid_width(val):
	grid_width = val
	queue_redraw()

func set_grid_height(val):
	grid_height = val
	queue_redraw()

func out_of_world(cell: Vector2i) -> bool:
	return cell.x < 0 || cell.x >= grid_width || cell.y < 0 || cell.y >= grid_height

func _free_cells() -> Array[Vector2i]:
	var empty_cells: Array[Vector2i] = []
	
	# Loop through every possible x and y coordinate of your grid
	for x in range(grid_width):
		for y in range(grid_height):
			var cell = Vector2i(x, y)
			
			# Check your snake array directly!
			# If the snake array does NOT contain this cell, it's empty.
			if not snake_cells.has(cell):
				empty_cells.append(cell)
				
	return empty_cells

func _on_snake_moved(snake: Array[Segment]) -> void:
	snake_cells.assign(snake.map(func(seg: Segment): return seg.cell))
	if snake_cells.size() == grid_height * grid_width:
		game_won.emit()

