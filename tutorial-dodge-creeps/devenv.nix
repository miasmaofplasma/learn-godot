{ pkgs, ... }:

{
  packages = [
    pkgs.git
    pkgs.gdtoolkit_4 # gdformat & gdlint CLI tools for GDScript
  ];

  # Wrapper script so typing 'godot' in devenv launches host Fedora godot
  scripts.godot.exec = ''
    host_godot="/usr/bin/godot"

    # If flags request terminal output/help/version/headless, run in foreground
    for arg in "$@"; do
      case "$arg" in
        --version|--help|-h|--headless|--quit)
          exec "$host_godot" "$@"
          ;;
      esac
    done

    # GUI / Editor mode: launch detached
    nohup "$host_godot" "$@" >/dev/null 2>&1 &
    disown
    echo "🚀 Godot launched in background."
  '';

  enterShell = ''
    echo "🎮 Godot environment ready (host binary: $(/usr/bin/godot --version 2>/dev/null || echo 'not found'))"
  '';
}
