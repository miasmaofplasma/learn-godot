use godot::classes::{INode, Node, Sprite2D};
use godot::prelude::*;

use crate::game_board::GameBoard;
use crate::segment::Segment;

#[derive(GodotClass)]
#[class(base = Node, init)]
pub struct FoodSpawner {
    base: Base<Node>,

    #[export]
    board: Option<Gd<GameBoard>>,

    snake: Vec<Gd<Segment>>,
    food_sprite: Option<Gd<Sprite2D>>,
    food_cell: Vector2i,
}

impl FoodSpawner {
    fn board(&self) -> Gd<GameBoard> {
        self.board.clone().expect("FoodSpawner.board not set")
    }

    fn set_food_cell(&mut self, cell: Vector2i) {
        self.food_cell = cell;
        let pos = self.board().bind().grid_to_world(cell);
        self.food_sprite
            .as_mut()
            .expect("FoodSpawner not ready")
            .set_position(pos);
    }

    fn spawn_food(&mut self) {
        let mut cell = self.board().bind().random_cell();
        while self.food_in_snake(cell) {
            cell = self.board().bind().random_cell();
        }
        self.set_food_cell(cell);
    }

    fn food_in_snake(&self, cell: Vector2i) -> bool {
        self.snake.iter().any(|seg| seg.bind().cell == cell)
    }
}

#[godot_api]
impl INode for FoodSpawner {
    fn ready(&mut self) {
        self.food_sprite = Some(self.base().get_node_as::<Sprite2D>("FoodSprite"));
        self.set_food_cell(Vector2i::new(3, 3));
    }
}

#[godot_api]
impl FoodSpawner {
    #[signal]
    fn food_eaten(cell: Vector2i);

    #[func(rename = _on_snake_moved)]
    fn on_snake_moved(&mut self, p_snake: Array<Gd<Segment>>) {
        self.snake = p_snake.iter_shared().collect();
        let head_cell = self.snake[0].bind().cell;
        if head_cell == self.food_cell {
            let eaten = self.food_cell;
            self.signals().food_eaten().emit(eaten);
            self.spawn_food();
        }
    }
}
