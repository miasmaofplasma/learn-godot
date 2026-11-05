use godot::classes::{INode2D, Node2D};
use godot::global::randi;
use godot::prelude::*;

#[derive(GodotClass)]
#[class(base = Node2D, tool, init)]
pub struct GameBoard {
    base: Base<Node2D>,

    #[export]
    #[var(set = set_cell_size)]
    #[init(val = 60)]
    cell_size: i32,

    #[export]
    #[var(set = set_grid_width)]
    #[init(val = 20)]
    grid_width: i32,

    #[export]
    #[var(set = set_grid_height)]
    #[init(val = 20)]
    grid_height: i32,
}

#[godot_api]
impl GameBoard {
    #[func]
    fn set_cell_size(&mut self, val: i32) {
        self.cell_size = val;
        self.base_mut().queue_redraw();
    }

    #[func]
    fn set_grid_width(&mut self, val: i32) {
        self.grid_width = val;
        self.base_mut().queue_redraw();
    }

    #[func]
    fn set_grid_height(&mut self, val: i32) {
        self.grid_height = val;
        self.base_mut().queue_redraw();
    }

    pub fn cell_size(&self) -> i32 {
        self.cell_size
    }

    pub fn random_cell(&self) -> Vector2i {
        let x = (randi() % self.grid_width as i64) as i32;
        let y = (randi() % self.grid_height as i64) as i32;
        Vector2i::new(x, y)
    }

    pub fn grid_to_world(&self, grid_pos: Vector2i) -> Vector2 {
        let cs = self.cell_size as f32;
        let half = cs / 2.0;
        Vector2::new(grid_pos.x as f32 * cs + half, grid_pos.y as f32 * cs + half)
    }
}

#[godot_api]
impl INode2D for GameBoard {
    fn draw(&mut self) {
        let cs = self.cell_size as f32;
        let max_width = self.grid_width as f32 * cs;
        let max_height = self.grid_height as f32 * cs;
        let line_color = Color::from_rgba(1.0, 1.0, 1.0, 0.5);

        for x in 0..=self.grid_width {
            let x_pos = x as f32 * cs;
            self.base_mut()
                .draw_line_ex(Vector2::new(x_pos, 0.0), Vector2::new(x_pos, max_height), line_color)
                .width(1.0)
                .done();
        }

        for y in 0..=self.grid_height {
            let y_pos = y as f32 * cs;
            self.base_mut()
                .draw_line_ex(Vector2::new(0.0, y_pos), Vector2::new(max_width, y_pos), line_color)
                .width(1.0)
                .done();
        }
    }
}
