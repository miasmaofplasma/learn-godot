use std::f32::consts::PI;

use godot::classes::{INode2D, InputEvent, Node, Node2D, Texture2D};
use godot::prelude::*;
use godot::tools::load;

use crate::game_board::GameBoard;
use crate::segment::Segment;

const HEAD: usize = 0;

const DIRECTIONS: [(&str, Vector2i); 4] = [
    ("move_left", Vector2i::LEFT),
    ("move_right", Vector2i::RIGHT),
    ("move_up", Vector2i::UP),
    ("move_down", Vector2i::DOWN),
];

/// (ahead, behind) -> (rotation, flip_h). Missing keys default to (0, false).
fn bend_transform(ahead: Vector2i, behind: Vector2i) -> (f32, bool) {
    match (ahead, behind) {
        (Vector2i::LEFT, Vector2i::UP) | (Vector2i::UP, Vector2i::LEFT) => (0.0, false),
        (Vector2i::UP, Vector2i::RIGHT) | (Vector2i::RIGHT, Vector2i::UP) => (PI / 2.0, false),
        (Vector2i::RIGHT, Vector2i::DOWN) | (Vector2i::DOWN, Vector2i::RIGHT) => (PI, false),
        (Vector2i::DOWN, Vector2i::LEFT) | (Vector2i::LEFT, Vector2i::DOWN) => {
            (3.0 * PI / 2.0, false)
        }
        _ => (0.0, false),
    }
}

#[derive(GodotClass)]
#[class(base = Node2D, init)]
pub struct Snake {
    base: Base<Node2D>,

    #[export]
    board: Option<Gd<GameBoard>>,

    #[export]
    #[init(val = Vector2i::new(10, 10))]
    start_position: Vector2i,

    segment_array: Vec<Gd<Segment>>,
    segments: Option<Gd<Node>>,

    #[init(val = Vector2i::LEFT)]
    direction: Vector2i,

    add_segment: bool,

    head_texture: Option<Gd<Texture2D>>,
    body_texture: Option<Gd<Texture2D>>,
    tail_texture: Option<Gd<Texture2D>>,
    bend_texture: Option<Gd<Texture2D>>,
}

impl Snake {
    fn add_segment_at(&mut self, index: usize, segment: Gd<Segment>) {
        self.segment_array.insert(index, segment.clone());
        self.segments
            .as_mut()
            .expect("Snake not ready")
            .add_child(&segment.upcast::<Node>());
    }

    fn board(&self) -> Gd<GameBoard> {
        self.board.clone().expect("Snake.board not set")
    }

    fn refresh_segment_visuals(&mut self) {
        let n = self.segment_array.len();
        for i in 1..n {
            let seg_cell = self.segment_array[i].bind().cell;
            let ahead_cell = self.segment_array[i - 1].bind().cell;

            if i == n - 1 {
                let dir = ahead_cell - seg_cell;
                self.segment_array[i].bind_mut().set_direction(dir);
                continue;
            }

            let behind_cell = self.segment_array[i + 1].bind().cell;
            let dir_ahead = ahead_cell - seg_cell;
            let dir_behind = behind_cell - seg_cell;

            let mut seg = self.segment_array[i].bind_mut();
            if dir_ahead != -dir_behind {
                // Corner
                seg.base_mut().set_texture(self.bend_texture.as_ref());
                let (rotation, flip_h) = bend_transform(dir_ahead, dir_behind);
                seg.base_mut().set_rotation(rotation);
                seg.base_mut().set_flip_h(flip_h);
            } else {
                // Straight
                seg.base_mut().set_texture(self.body_texture.as_ref());
                seg.set_direction(dir_ahead);
            }
        }
    }
}

#[godot_api]
impl INode2D for Snake {
    fn ready(&mut self) {
        self.segments = Some(self.base().get_node_as::<Node>("Segments"));
        self.head_texture = Some(load::<Texture2D>("res://assets/head.png"));
        self.body_texture = Some(load::<Texture2D>("res://assets/body.png"));
        self.tail_texture = Some(load::<Texture2D>("res://assets/tail.png"));
        self.bend_texture = Some(load::<Texture2D>("res://assets/bend.png"));

        let board = self.board();
        let start = self.start_position;
        let head = Segment::create(start, self.head_texture.clone().unwrap(), board.clone());
        self.add_segment_at(0, head);
        let body = Segment::create(
            start + Vector2i::new(1, 0),
            self.body_texture.clone().unwrap(),
            board.clone(),
        );
        self.add_segment_at(1, body);
        let tail = Segment::create(
            start + Vector2i::new(2, 0),
            self.tail_texture.clone().unwrap(),
            board,
        );
        self.add_segment_at(2, tail);
    }

    fn unhandled_input(&mut self, event: Gd<InputEvent>) {
        for (action, new_direction) in DIRECTIONS {
            if event.is_action_pressed(action) {
                // Reversal guard: compare against the OPPOSITE of the current direction.
                if new_direction != -self.direction {
                    self.direction = new_direction;
                    return;
                }
            }
        }
    }
}

#[godot_api]
impl Snake {
    // Synchronous emit. food_eaten is connected deferred in main.tscn, so Snake is not re-borrowed mid-tick.
    #[signal]
    fn moved(cell: Array<Gd<Segment>>);

    #[func(rename = _on_tick_timer_timeout)]
    fn on_tick_timer_timeout(&mut self) {
        let old_head_cell = self.segment_array[HEAD].bind().cell;

        if self.add_segment {
            self.add_segment = false;
            // Don't shift the body! Just insert a new segment where the head was.
            let seg = Segment::create(
                old_head_cell,
                self.body_texture.clone().unwrap(),
                self.board(),
            );
            self.add_segment_at(1, seg);
        } else {
            for i in (1..self.segment_array.len()).rev() {
                let prev = self.segment_array[i - 1].bind().cell;
                self.segment_array[i].bind_mut().set_cell(prev);
            }
        }

        let direction = self.direction;
        {
            let mut head = self.segment_array[HEAD].bind_mut();
            let new_cell = head.cell + direction;
            head.set_cell(new_cell);
            head.set_direction(direction);
        }

        self.refresh_segment_visuals();

        let snapshot: Array<Gd<Segment>> = self.segment_array.iter().cloned().collect();
        self.signals().moved().emit(&snapshot);
    }

    #[func(rename = _on_food_manager_food_eaten)]
    fn on_food_manager_food_eaten(&mut self, _cell: Vector2i) {
        self.add_segment = true;
    }
}
