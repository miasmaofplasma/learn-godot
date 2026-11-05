extends Node2D

const HEAD: int = 0

@export var board: GameBoard

@export var start_position: Vector2i = Vector2i(10,10)
var segment_array: Array[Segment] = []
@onready var segments = $Segments
var direction = Vector2i.LEFT

var add_segment: bool

signal moved(snake_array: Array[Segment])
signal died()

const DIRECTIONS := {
   "move_left": Vector2i.LEFT,
   "move_right": Vector2i.RIGHT,
   "move_up": Vector2i.UP,
   "move_down": Vector2i.DOWN,
}

const BEND_TRANSFORMS := {
	# Left & Top
	[Vector2i.LEFT, Vector2i.UP]: [0.0, false],
	[Vector2i.UP, Vector2i.LEFT]: [0.0, false],

	# Top & Right
	[Vector2i.UP, Vector2i.RIGHT]: [PI / 2, false],
	[Vector2i.RIGHT, Vector2i.UP]: [PI / 2, false],

	# Right & Bottom
	[Vector2i.RIGHT, Vector2i.DOWN]: [PI, false],
	[Vector2i.DOWN, Vector2i.RIGHT]: [PI, false],

	# Bottom & Left
	[Vector2i.DOWN, Vector2i.LEFT]: [3 * PI / 2, false],
	[Vector2i.LEFT, Vector2i.DOWN]: [3 * PI / 2, false],
}

#segment scenes
var head_texture: Texture2D= load("res://assets/head.png")
var body_texture: Texture2D= load("res://assets/body.png")
var tail_texture: Texture2D= load("res://assets/tail.png")
var bend_texture: Texture2D= load("res://assets/bend.png")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var head = Segment.new(start_position, head_texture, board)
	_add_segment(0, head)
	var body = Segment.new(start_position + Vector2i(1,0), body_texture, board)
	_add_segment(1, body)
	var tail = Segment.new(start_position + Vector2i(2,0), tail_texture, board)
	_add_segment(2, tail)

func _unhandled_input(event: InputEvent) -> void:
	for action: String in DIRECTIONS:
		if event.is_action_pressed(action):
			var new_direction: Vector2i = DIRECTIONS[action]
			if new_direction != -direction:   # reversal guard: compare against the OPPOSITE
				direction = new_direction
				return

func _on_tick_timer_timeout() -> void:
	var old_head_cell: Vector2i = segment_array[HEAD].cell
	var new_head_cell = old_head_cell + direction

	if board.out_of_world(new_head_cell):
		died.emit()
		return

	if add_segment:
		add_segment = false
		# Don't shift the body! Just insert a new segment where the head was:
		_add_segment(1, Segment.new(old_head_cell, body_texture, board))
	else:
		# Normal tick: shift the body forward
		for i in range(segment_array.size() - 1, 0, -1):
			segment_array[i].cell = segment_array[i - 1].cell
			if segment_array[i].cell == new_head_cell:
				died.emit()

	var head = segment_array[HEAD]
	
	head.cell = new_head_cell 
	head.direction = direction

	_refresh_segment_visuals()
	_moved()

	
func _add_segment(index: int, segment: Segment) -> void: 
	segment_array.insert(index, segment)
	segments.add_child(segment)

func _refresh_segment_visuals():
	for i in range(1, segment_array.size()):
		var seg = segment_array[i]
		var ahead = segment_array[i-1]

		if i == segment_array.size() -1:
			seg.direction = ahead.cell - seg.cell
			continue

		# Body piece (straight or bend)
		var behind := segment_array[i + 1]
		var dir_ahead: Vector2i = ahead.cell - seg.cell
		var dir_behind: Vector2i = behind.cell - seg.cell

		if dir_ahead != -dir_behind:
			# Corner!
			seg.texture = bend_texture
			var t: Array = BEND_TRANSFORMS.get([dir_ahead, dir_behind], [0.0, false])
			seg.rotation = t[0]
			seg.flip_h = t[1]
		else:
			# Straight segment
			seg.texture = body_texture
			seg.direction = dir_ahead

func _moved(): 
	moved.emit(segment_array)


func _on_food_manager_food_eaten(_cell: Vector2i) -> void:
	add_segment = true
