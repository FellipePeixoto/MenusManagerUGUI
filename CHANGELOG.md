# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
- Animator Display Method

### Added
- Nothing yet

### Changed
- Nothing yet

### Deprecated
- Nothing yet

### Removed
- Nothing yet

### Fixed
- Nothing yet

## [1.0.0] - 2025-09-08

### Added
- First version of the tool

### Features
- **Simplified Control over menus**: Show and hide your menus with a function call or via inspector bindable action.
- **Multiple ways to transition between menus**: You choose: Set enabled or disabled, Hide with Canvas Group (using alpha), Fade or control via Animator
- **Integrated with the Event System**: Set a default selected game object in the UI when the menu opens.
- **Bindable events in the inspector**: OnInit, OnShow and OnHide
- **Stackable menus**: Each menu that opens is remembered. Forward navigation goes deeper, back navigation retraces the exact route.
- **Keep menu on background**: Open a menu in front of another and keep the old one visible in the background.
- **Automatically Get the Menus**: Get the menus automatically in children or set by yourself.

---

## Release Notes

### Version 1.0.0
This is the initial stable release of the Menus Manager Tool package. The component provides a helper to manage menus in Unity.

**Key Highlights:**
- Custom transition methods
- Button navigation assignment requires no additional code

---

## Future Roadmap

- Include OnBeforeShow, OnShow, OnBeforeHide, OnHide
- Add labels to better explain what each inspector option does
- UNDO and Redo functionality
- Better Editor Class structure
- Add a dropdown to list the menus available from the current menu
- Option to keep the last selected object in memory to select it by default when returning to the screen
- Add missing parts in the API reference