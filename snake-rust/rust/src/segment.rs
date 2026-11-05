use std::f32::consts::{FRAC_PI_2, PI};

use godot::classes::{Sprite2D, Texture2D};
use godot::prelude::*;

use crate::game_board::GameBoard;

#[derive(GodotClass)]
#[class(base = Sprite2D, init)]
pub struct Segment {
    base: Base<Sprite2D>,
    pub cell: Vector2i,
    #[init(val = Vector2i::LEFT)]
    pub direction: Vector2i,
    board: Option<Gd<GameBoard>>,
}

impl Segment {
    pub fn create(cell: Vector2i, texture: Gd<Texture2D>, board: Gd<GameBoard>) -> Gd<Self> {
        let mut seg = Self::new_alloc();
        {
            let mut s = seg.bind_mut();
            let native = texture.get_size();
            let cell_size = board.bind().cell_size();
            s.base_mut().set_texture(&texture);
            s.base_mut()
                .set_scale(Vector2::splat(cell_size as f32 / native.x));
            s.board = Some(board);
            s.set_cell(cell);
        }
        seg
    }

    pub fn set_cell(&mut self, cell: Vector2i) {
        self.cell = cell;
        let pos = self
            .board
            .as_ref()
            .expect("Segment has no board")
            .bind()
            .grid_to_world(cell);
        self.base_mut().set_position(pos);
    }

    pub fn set_direction(&mut self, direction: Vector2i) {
        self.direction = direction;
        let (rotation, flip_h) = if direction == Vector2i::LEFT {
            (0.0, false)
        } else if direction == Vector2i::RIGHT {
            (0.0, true)
        } else if direction == Vector2i::UP {
            (FRAC_PI_2, false)
        } else if direction == Vector2i::DOWN {
            (PI + PI / 2.0, false)
        } else {
            panic!("non-cardinal segment direction: {direction:?}");
        };
        self.base_mut().set_rotation(rotation);
        self.base_mut().set_flip_h(flip_h);
    }
}
