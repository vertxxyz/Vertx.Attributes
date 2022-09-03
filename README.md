# Vertx.Attributes
Attributes and Property Drawers/Decorators for Unity


## Attributes

| Attribute              | Description                                                                                                                                                                                                                                                                                                                                                                                                                  |
|------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **EditorOnly**         | Field is only editable in Edit mode.                                                                                                                                                                                                                                                                                                                                                                                         |
| **ReadOnly**           | Field is not editable.                                                                                                                                                                                                                                                                                                                                                                                                       |
| **EnumFlags**          | Displays a enum bit field with multiple values instead of displaying the default *Mixed*.<br/>*bool RedZero* - optional parameter to tint the field red when 0/None is selected.<br/>*HideObsoleteNames* - Hides enum values marked with `System.ObsoleteAttribute`.                                                                                                                                                         |
| **EnumDropdown**       | Shows an AdvancedDropdown instead of a GenericMenu.<br/>This allows for proper scrolling on Windows, and is pretty much invaluable for large enums.<br/>*bool RedZero* - optional parameter to tint the field red when 0/None is selected.                                                                                                                                                                                   |
| **KeyCode**            | Adds a picker to rebind keys by using the keyboard.                                                                                                                                                                                                                                                                                                                                                                          |
| **MinMax**             | A Range slider for `Vector2`, `Vector2Int`, Unity.Mathematics `float2` and `int2` types.<br/>Can be used with two `float` or `int` by applying `[HideInInspector]` to the second field.<br/>*float min, max* - the min and max bounds to the slider.<br/>*string label* - optional label override, generally used for int/float fields.<br/>*Aligned* -  UIToolkit-specific setting that aligns with fields in the inspector | 
| **Progress**           | Displays values in a progress bar styling.<br/>*float maxValue = 1* - defines the upper range to be remapped.                                                                                                                                                                                                                                                                                                                | 
| **CurveDisplay**       | Clamps a Curve to a new range and/or restyles its color.<br/>*int minX, minY, maxX, maxY* - the bounds of the curve.<br/>*float r, g, b* - colour values used for display.                                                                                                                                                                                                                                                   | 
| **Blend2D**            | Displays a 2D trackpad-like interface/graph for `Vector2` or Unity.Mathematics `float2` values.<br/>*string xLabel, yLabel* - Labels for the X and Y axes of the graph.<br/>*float minX, minY, maxX, maxY* - The bounds of the graph.                                                                                                                                                                                        |
| **HelpBox**            | Decorates a field with a help box.                                                                                                                                                                                                                                                                                                                                                                                           |
| **File**/**Directory** | Styles string fields with a button to pick files/directories.<br/>*bool fileIsLocalToProject* - Constrains the selection to be within the Assets directory.                                                                                                                                                                                                                                                                  |

> **Note**  
> These implementations perform no logic to implement nested property drawers.

---
If you find this resource helpful:

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/Z8Z42ZYHB)

## Installation

<details>
<summary>Add from OpenUPM <em>| via scoped registry, recommended</em></summary>

This package is available on OpenUPM: https://openupm.com/packages/com.vertx.attributes

To add it the package to your project:

- open `Edit/Project Settings/Package Manager`
- add a new Scoped Registry:
  ```
  Name: OpenUPM
  URL:  https://package.openupm.com/
  Scope(s): com.vertx
  ```
- click <kbd>Save</kbd>
- open Package Manager
- click <kbd>+</kbd>
- select <kbd>Add from Git URL</kbd>
- paste `com.vertx.attributes`
- click <kbd>Add</kbd>
</details>

<details>
<summary>Add from GitHub | <em>not recommended, no updates through UPM</em></summary>

You can also add it directly from GitHub on Unity 2019.4+. Note that you won't be able to receive updates through Package Manager this way, you'll have to update manually.

- open Package Manager
- click <kbd>+</kbd>
- select <kbd>Add from Git URL</kbd>
- paste `https://github.com/vertxxyz/Vertx.Attributes.git`
- click <kbd>Add</kbd>  
  **or**
- Edit your `manifest.json` file to contain `"com.vertx.editors": "https://github.com/vertxxyz/Vertx.Attributes.git"`,

To update the package with new changes, remove the lock from the `packages-lock.json` file.
</details>