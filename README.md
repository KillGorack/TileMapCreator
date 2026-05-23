# Tilemap Creator

A cross-platform tool for generating tilesets from a base texture and border image.
Built with C# and Avalonia, runs on Windows and Linux.

Supports both 9-slice borders (44 recipes → 11×4 tileset) and 4-block borders (16 recipes → 4×4 tileset).

## Features

- Load a border image and a base tile
- Set border size in pixels
- Automatically detects:
  - 9-slice mode → 44 recipes, 11×4 tileset
  - 4-block mode → 16 recipes, 4×4 tileset
- Exports a single PNG spritesheet ready for Godot, Unity, or any tile-based engine

## Downloads

Pre-built binaries available at [KillGorack.com](https://www.killgorack.com/PX4/index.php?ap=hme&cn=hme)

- Windows x64
- Linux x64

## Building from source

```bash
git clone https://github.com/KillGorack/TileMapCreator.git
cd TileMapCreator
dotnet run
```

## Usage

1. Click Add Border Map and select your border texture
2. Click Add Tile Map and select your base tile
3. Set the border size in pixels
4. Click Generate Map

## Usage Notes

- If border size is exactly half the image width and height → 4×4 tileset
- If border size is smaller than half → 11×4 tileset
- Works best with square tiles (e.g. 64×64, 128×128)
- The border image should have transparent areas where the base tile shows through

## License

MIT — free to use, modify, and share.
