# 🧱 Tilemap Builder 🧱

A simple WPF tool for generating **tilesets** from a base texture and border image.  
It supports both **9‑slice borders** (40 recipes → 10×4 tileset) and **4‑block borders** (16 recipes → 4×4 tileset).

---

## Features
- Load a **border image** and a **base tile**
- Enter border size
- Automatically detects:
  - **9‑slice mode** → 40 recipes, 10×4 tileset
  - **4‑block mode** → 16 recipes, 4×4 tileset
- Exports a single **PNG spritesheet** ready for game engines like Godot or Unity

---

## Getting Started

1. **Clone the repo**
   ```bash
   git clone https://github.com/yourusername/tilemap-builder.git
   cd tilemap-builder

2. **Build and run the project** using Visual Studio or your preferred IDE.

3. **Load your images**:
   - Click "Load Border Image" to select your border texture.
   - Click "Load Base Tile" to select your base tile texture.

4. **Set the border size** in pixels.

5. **Generate the tileset** by clicking the "Generate Tileset" button.


## Usage Notes

- If the border size is exactly half the image width and height → 4×4 tileset (TileSetRecipeB).
- If the border size is smaller than half → 10×4 tileset (TileSetRecipe).

<img width="735" height="515" alt="image" src="https://github.com/user-attachments/assets/b6ba666f-bb32-455c-a878-3d077a455d1c" />
<img width="256" height="256" alt="TileSet_20251116064445" src="https://github.com/user-attachments/assets/84bbe603-f5b8-4e39-940f-f955f6c5d03c" />
<img width="640" height="256" alt="TileSet_20251116065429" src="https://github.com/user-attachments/assets/dd205630-b71f-4b3c-a837-3b23ca5eac85" />



MIT License — free to use, modify, and share.

