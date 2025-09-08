# Unity Menus Manager Tool

A Unity extension that provides a comprehensive helper to manage menus in Unity with simplified control and advanced navigation features.

## Features

- **Simplified Control over menus**: Show and hide your menus with a function call or via inspector bindable action.
- **Multiple ways to transition between menus**: You choose: Set enabled or disabled, Hide with Canvas Group (using alpha), Fade or control via Animator
- **Integrated with the Event System**: Set a default selected game object in the UI when the menu opens.
- **Bindable events in the inspector**: OnInit, OnShow and OnHide
- **Stackable menus**: Each menu that opens is remembered. Forward navigation goes deeper, back navigation retraces the exact route.
- **Keep menu on background**: Open a menu in front of another and keep the old one visible in the background.
- **Automatically Get the Menus**: Get the menus automatically in children or set by yourself.
- **Custom transition methods** for enhanced visual effects.
- **Button navigation assignment requires no additional code**.

## Installation

Just add this repository as a package in your project

## Usage

1. Add the Menus Manager component to a Game Object inside a Canvas or even to the Canvas itself. Available at: **Add Component >> DevPeixoto >> UI >> Menus Manager**. It will be your manager.\
![Inserting the component](/Docs~~/Unity_XkXHDxoM3e.gif)
2. Add Menus. Menus are Rect Transforms or Game Objects that are children of the Menus Manager with the Menu component. **Add Component >> DevPeixoto >> UI >> Menu**.\
![Setup_1](/Docs~/Unity_EZaPuYYZUY.gif)
3. Add the created menus to the Menu Manager or just check the option "Automatically get the menus in children".\
![Setup_2](/Docs~/Unity_eIxrkda6HL.gif)
4. Configure your menu navigation and events through the inspector. The buttons inside the Menu hierarchy are loaded automatically in the Menu inspector.\
![Configuration](/Docs~/Unity_e5wdNXhuYR.gif)
5. It's ready to fly!\
![Demo](/Docs~/Unity_7AXcSwpz5C.gif)

## Requirements

- Tested on Unity 6000.0.38.

## Version History

### [1.0.0] - 2025-09-08
- Initial stable release
- All core menu management features implemented
- Complete inspector integration
- Stackable menu system
- Multiple transition options

## Contributing

Contributions are welcome! Open an issue or submit a pull request.

## License

MIT License