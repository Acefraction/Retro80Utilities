# Palette Template Specification

This folder contains editable palette templates for use with Retro80Colorizer.

Each palette is defined as a 32×32 grid PNG image.  
**Each cell in the grid is 16×16 pixels**, and the color at the **center of each cell** (pixel at `x * 16 + 8`, `y * 16 + 8`) is used as the actual palette color by the program.

## 💡 Structure

- The **horizontal axis (X)** represents color variations within a color group (e.g., different shades of orange).
- The **vertical axis (Y)** represents logical **function groups**, such as:
  - Character skin tones
  - Background tones
  - Highlight / Shadow
  - UI elements

Up to **32 function groups** and **32 color variants** per group can be defined.

## 🖌️ How to Edit

- You can open and edit the PNG file in any image editor that supports pixel-level editing.
- Only the **center color** of each cell is used — the surrounding area is for human readability.
- Grid lines are optional and ignored by the system.

## 🔄 Use in the Program

- At runtime, the program reads only the center pixels of each grid cell and constructs a `Color[,]` array.
- This allows for easy visual editing while keeping the program logic clean.

## 📝 Naming Convention

Use descriptive names for palette files:

For each palette PNG file, an optional .labels.json file with the same name can be added to define the meaning of each row (Y axis).
