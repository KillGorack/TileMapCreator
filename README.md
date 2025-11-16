# Tilemap Creator

A simple WPF tool for generating **tilesets** from a base texture and border image.  
It supports both **9‑slice borders** (40 recipes → 10×4 tileset) and **4‑block borders** (16 recipes → 4×4 tileset).  

## Features
- Load a **border image** and a **base tile**
- Enter border size
- Automatically detects:
  - **9‑slice mode** → 40 recipes, 10×4 tileset
  - **4‑block mode** → 16 recipes, 4×4 tileset
- Exports a single **PNG spritesheet** ready for game engines like Godot or Unity

## Getting Started

1. **Clone the repo**
   ```bash
   git clone https://github.com/yourusername/tilemap-builder.git
   cd tilemap-builder
2. **Build and run the project** using Visual Studio or your preferred IDE.
3. **Load your images**:
   1. Click "Load Border Image" to select your border texture.
   2. Click "Load Base Tile" to select your base tile texture.
4. **Set the border size** in pixels.
5. **Generate the tileset** by clicking the "Generate Tileset" button.


## Usage Notes

> If the border size is exactly half the image width and height → 4×4 tileset (TileSetRecipeB).  
>
> If the border size is smaller than half → 10×4 tileset (TileSetRecipe).
>
> Works best with square tiles (e.g., 64×64, 128×128 pixels).
>
> The border image should have transparent areas where the base tile will show through.


### User interface

<img width="367" height="257" alt="image" src="https://github.com/user-attachments/assets/b6ba666f-bb32-455c-a878-3d077a455d1c" />  

### Example 16 block map

<img width="128" height="128" alt="TileSet_20251116064445" src="https://github.com/user-attachments/assets/84bbe603-f5b8-4e39-940f-f955f6c5d03c" />  

### Example 40 block map

<img width="320" height="128" alt="Stone_Block" src="https://github.com/user-attachments/assets/36265302-56bf-48b2-990e-d1da9d202162" />

### Example in Godot TileSet Editor

<img width="588" height="218" alt="image" src="https://github.com/user-attachments/assets/561054ff-86cf-4bd6-b76e-4d4b2651bc60" />



[KillGorack.com](https://www.killgorack.com/PX4/index.php?ap=hme&cn=hme)  
MIT License — free to use, modify, and share.

