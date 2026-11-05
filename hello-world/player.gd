extends CharacterBody2D


@export var speed = 500.0


@onready var fireworks: CPUParticles2D = $Fireworks


func _physics_process(delta: float) -> void:
	var direction := Input.get_vector("move_left", "move_right", "move_up", "move_down")
	velocity = direction * speed
	move_and_slide()

	# Trigger fireworks burst on Spacebar press
	if Input.is_action_just_pressed("trigger_fireworks"):
		# Pick a random vibrant color for each firework burst
		fireworks.color = Color.from_hsv(randf(), 0.85, 1.0)
		fireworks.restart()
		fireworks.emitting = true
