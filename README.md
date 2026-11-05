# learn-godot

A monorepo for Godot game development learning projects, experiments, tutorials, and demos.

## Projects

| Directory | Description | Engine / Language |
| :--- | :--- | :--- |
| `hello-world/` | Initial introductory Godot project | Godot 4 / GDScript |
| `tutorial-dodge-creeps/` | "Dodge the Creeps" official 2D beginner tutorial | Godot 4 / GDScript |
| `snake/` | 2D Snake game implementation | Godot 4 / C# (.NET) |
| `snake-rust/` | Snake game using GDExtension | Godot 4 / Rust (`godot-rust`) |
| `dotnet/` | Exploration and experiments with Godot .NET / C# | Godot 4 / C# (.NET) |
| `tootie-farm/` | Farming sim project with unit testing via GdUnit4 | Godot 4 / C# (.NET) |
| `demos/third-person-shooter-demo/` | 3D third-person shooter exploration / demo | Godot 4 / 3D |

## Setup & Development

Each subproject contains its own Godot configuration (`project.godot`).

1. Open **Godot 4** project manager.
2. Click **Import** and select the `project.godot` file inside any subproject directory.
3. For C#/.NET projects (`snake`, `dotnet`, `tootie-farm`), ensure the [.NET 8+ SDK](https://dotnet.microsoft.com/) and Godot .NET build are installed.
4. For Rust projects (`snake-rust`), build the GDExtension library in `snake-rust/rust/` with `cargo build`.
