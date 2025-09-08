# API Reference - Unity Menus Manager Tool

## Core Classes

### MenusManager
Main component for managing menu navigation and transitions.

### Menu
Manageable component that represents an individual menu in the system.

### Menus Manager Methods
- `SwitchTo(Menu menu)`: Switches to a target menu.
- `Back()`: Triggers a back action, returning to the last active menu.

### Menus Manager Inspector Properties Only
- `bool Use First Sibling as Default`: Use the first menu in children as the default menu?
- `bool Do not repeat menus in stack`: Prevent stacking repeated menus when using `SwitchTo`?
- `bool Automatically get the menus in children`: Get the Menu components in children automatically?

### Menu Inspector Properties Only
- `bool Keep On Background`: Keep the menu visible in the background.
- `MenuDisplayMethod Display Method`: The display method the menu will follow when calling Show or Hide.

## Enums

### MenuDisplayMethod
- `State`: Set the Game Object Enabled or Disabled
- `CanvasGroup`: Controls the interaction and Alpha of the Canvas Group attached
- `Fade`: Fade in or out the Menu