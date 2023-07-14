# Vertx.Attributes
Attributes and Property Drawers/Decorators for Unity.  
All drawers support IMGUI and UIToolkit.

> **Warning**  
> Unity 2019.4+.  
> UIToolkit support is version-specific.


## Attributes

| Attribute          | Description                                                                                                                                                                                                                                                                                                                                                                                                                  |
|--------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `EditorOnlyField`  | Field is only editable in Edit mode.                                                                                                                                                                                                                                                                                                                                                                                         |
| `ReadOnlyField`    | Field is not editable.                                                                                                                                                                                                                                                                                                                                                                                                       |
| `EnumFlags`        | Displays a enum bit field with multiple values instead of displaying the default *Mixed*.<br/>*bool RedZero* - optional parameter to tint the field red when 0/None is selected.<br/>*HideObsoleteNames* - Hides enum values marked with `System.ObsoleteAttribute`.                                                                                                                                                         |
| `EnumDropdown`     | Shows an AdvancedDropdown instead of a GenericMenu.<br/>This allows for proper scrolling on Windows, and is pretty much invaluable for large enums.<br/>*bool RedZero* - optional parameter to tint the field red when 0/None is selected.                                                                                                                                                                                   |
| `KeyCode`          | Adds a picker to rebind keys by using the keyboard.                                                                                                                                                                                                                                                                                                                                                                          |
| `Layer`            | Adds layer dropdown for `int`.                                                                                                                                                                                                                                                                                                                                                                                               |
| `MinMax`           | A Range slider for `Vector2`, `Vector2Int`, Unity.Mathematics `float2` and `int2` types.<br/>Can be used with two `float` or `int` by applying `[HideInInspector]` to the second field.<br/>*float min, max* - the min and max bounds to the slider.<br/>*string label* - optional label override, generally used for int/float fields.<br/>*Aligned* -  UIToolkit-specific setting that aligns with fields in the inspector | 
| `Progress`         | Displays values in a progress bar styling.<br/>*float maxValue = 1* - defines the upper range to be remapped.                                                                                                                                                                                                                                                                                                                | 
| `CurveDisplay`     | Clamps a Curve to a new range and/or restyles its color.<br/>*int minX, minY, maxX, maxY* - the bounds of the curve.<br/>*float r, g, b* - colour values used for display.                                                                                                                                                                                                                                                   | 
| `Blend2D`          | Displays a 2D trackpad-like interface/graph for `Vector2` or Unity.Mathematics `float2` values.<br/>*string xLabel, yLabel* - Labels for the X and Y axes of the graph.<br/>*float minX, minY, maxX, maxY* - The bounds of the graph.                                                                                                                                                                                        |
| `HelpBox`          | Decorates a field with a help box.                                                                                                                                                                                                                                                                                                                                                                                           |
| `File`/`Directory` | Styles string fields with a button to pick files/directories.<br/>*bool fileIsLocalToProject* - Constrains the selection to be within the Assets directory.                                                                                                                                                                                                                                                                  |
| `Inline`           | Removes the foldout from a group of serialized fields.                                                                                                                                                                                                                                                                                                                                                                       |

> **Note**  
> These implementations perform no logic to implement nested property drawers.

## Installation
[![openupm](https://img.shields.io/npm/v/com.vertx.attributes?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.vertx.debugging/)

<table><tr><td>

#### Add the OpenUPM registry
1. Open `Edit/Project Settings/Package Manager`
1. Add a new Scoped Registry (or edit the existing OpenUPM entry):
   ```
   Name: OpenUPM
   URL:  https://package.openupm.com/
   Scope(s): com.vertx
   ```
1. **Save**

#### Add the package
1. Open the Package Manager via `Window/Package Manager`.
1. Select the <kbd>+</kbd> from the top left of the window.
1. Select **Add package by Name** or **Add package from Git URL**.
1. Enter `com.vertx.attributes`.
1. Select **Add**.

</td></tr></table>

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/Z8Z42ZYHB)
