extends Node

@export var board: GameBoard
var snake: Array[Segment]
@onready var food_sprite = $FoodSprite
var food_cell: set = _set_food_cell

signal food_eaten

func _ready() -> void:
	food_cell = Vector2i(3,3)
	
func _on_snake_moved(p_snake: Array[Segment]) -> void:
	snake = p_snake
	var head = snake[0]
	if head.cell == food_cell:
		food_eaten.emit(food_cell)
		_spwan_food()

func _spwan_food():
	food_cell= board.random_free_cell()
	while _food_in_snake():
		food_cell = board.random_cell()

func _food_in_snake() -> bool:
	for seg in snake:
		if seg.cell == food_cell:
			return true

	return false

func _set_food_cell(p_cell):
	food_cell = p_cell
	food_sprite.position = board.grid_to_world(food_cell)
	
