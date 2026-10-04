# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

RazerEditor is a small WPF desktop app (.NET 8, `net8.0-windows`) that edits key remappings in Razer Synapse profile XML files for the **Razer Tartarus** keypad. It reads and writes Synapse's on-disk profiles directly; it does not talk to the device or to Synapse.

## Commands

```sh
dotnet build RazerEditor.sln          # build
dotnet run --project RazerEditor.csproj  # run the app
```

There are no tests, linter config, or CI in this repo. It is not a git repository.

## Architecture

All source files are flat in the repo root, in namespace `RazerEditor`.

- **`ProfileService.cs`**: the data layer. `LoadAll()` scans `%ProgramData%\Razer\Synapse\Accounts\*\Devices\Tartarus\Profiles\*.xml` and parses each file into `TartarusProfile` → `KeymapLayer` (one per `<KeymapItem>`) → `KeyAssignment` (one per `<Key id="..."><Assignment id="TYPE">`). Malformed files are skipped silently. `Save()` reloads the original XML and patches only the `<HID>` and `<Mod>` text of existing `SINGLEKEY` assignments in place, so everything else in the Synapse file is preserved.
- **`HidKeys.cs`**: the static catalog of remap targets (`HidKey`), mapping a USB HID usage string (e.g. `"070004"` = A) to a display name, picker group, and Windows scan code. `HidKey.ModString` builds the Synapse `<Mod>` value in the format `[extended]00[scanCode]00[scanCode+0x80]` (hex). Adding a remappable key means adding an entry here with the correct scan code and extended flag.
- **`MainWindow.xaml(.cs)`**: the `PhysicalKeys` table defines the Tartarus's physical buttons (by their default HID, label, and layout section: main 5×3 grid, thumb, D-pad). The key buttons are rebuilt in code (`BuildKeyButtons`) every time the profile, layer, or a mapping changes. Button styles (`KeyBtnDefault`, `KeyBtnRemapped`, `KeyBtnUnsupported`) are defined in the window resources and looked up by name. `_suppressEvents` guards the combo-box handlers while their `ItemsSource` is being set in code.
- **`KeyPickerDialog.xaml(.cs)`**: a modal dialog listing `HidKeys.All` grouped and searchable. It returns the chosen key through `SelectedKey`.

### Key identifiers

Physical keys and remap targets both use 6-hex-digit HID usage strings (`07xxxx`). The `<Key id>` attribute in the profile XML is the physical key's HID, and the `<HID>` element is the target. Compare them case-insensitively; the parser upper-cases `PhysicalKeyId`.

### Limitations to keep in mind

- Only `SINGLEKEY` assignments are editable. Other assignment types (macros etc.) are shown as `[TYPE]` and ignored on click and on save.
- If a key has no assignment in a layer, the UI creates a new in-memory `KeyAssignment`, but `Save()` only updates `<Key>` elements that already exist in the XML. That new mapping is **not** written to disk.
- `Save()` writes straight to the live Synapse profile file. Synapse may need to be restarted or the profile reloaded before changes take effect. Writing to `%ProgramData%` may need elevated permissions.
- D-pad buttons are positioned with `Canvas.SetLeft/SetTop`, but `DpadGrid` in the XAML is a `Grid`, so those attached properties have no effect there.
