# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
- Nothing in progress

### Added
- Option to skip a frame on open/close menus operations

### Changed
- Nothing

### Deprecated
- Nothing

### Removed
- Nothing

### Fixed
- Nothing

## [1.4.0] - 2025-09-26

### Added
- Option to skip a frame on open/close menus operations

## [1.3.4] - 2025-09-20

### Fixed
- Missing dependent compilation

## [1.3.3] - 2025-09-19

### Fixed
- Show as overlay wasn't disabling interaction

## [1.3.2] - 2025-09-17

### Fixed
- ExecuteAnimationOnInit was doing nothing. Rewind to the conditional normalized time of Animator

## [1.3.1] - 2025-09-17

### Fixed
- Null reference on add Button Nav or Back Buton

## [1.3.0] - 2025-09-17

### Added
- Add a dropdown to list the menus available from the current menu
- Merge Canvas Group Method with Fade method
- Include OnBeforeShow, OnShow, OnBeforeHide, OnHide

### Changed
- Great Refactor
- SwitchTo renamed to Open
- Replaced the Keep On Background option OpenOverlay. Function passed to the Menu Manager
- Add of internals to keep the flow clean
- Add scaled time for CanvasGroup and Animator modes only
- Grouped Fades

### Deprecated
- UIFlow container

### Removed
- XML: Inspector UI made by code only

### Fixed
- Animator display mode interfering with other display methods

## [1.2.1] - 2025-09-10

### Fixed
- Missing platform dependent compilation

## [1.2.0] - 2025-09-10

### Added
- Bool to setup behaviour of animator method on Init

### Changed
- Init function signature of Menu classes
- Removed a option from Display Method Enum

### Fixed
- All menus playing Hidden animation on Init

## [1.1.0] - 2025-09-09

### Added

- Animator Display Method

### Changed

- Inspector UI of Menu

### Removed
- Some meta files

## [1.0.0] - 2025-09-08

### Added
- First version of the tool