use godot::prelude::*;

mod food_spawner;
mod game_board;
mod segment;
mod snake;

struct SnakeExtension;

#[gdextension]
unsafe impl ExtensionLibrary for SnakeExtension {}
