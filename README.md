# Vertx.Attributes
Attributes and Property Drawers/Decorators for Unity


## Attributes
- **[EditorOnly]**  
Field is only editable in Edit-Mode.
- **[ReadOnly]**  
Field is not editable.
  

- **[EnumFlags]**  
Displays a enum bit field with multiple values instead of displaying the default *Mixed*.  
*bool RedZero* - optional parameter to tint the field red when 0/None is selected.
  

- **[KeyCode]**  
Adds a picker to rebind keys by using the keyboard.
  

- **[MinMax]**  
A Range slider for Vector2, Vector2Int. Has support for Unity.Mathematics float2 and int2 types.  
Can be used with float and int by applying [HideInInspector] to the second field.  
*float min, max* - the min and max bounds to the slider.  
*string label* - optional label to override the default. This is generally used for int/float fields.
  

- **[Progress]**  
Displays values in a progress bar styling.  
*float maxValue = 1* - defines the upper range to be remapped.  
  

- **[CurveDisplay]**  
Clamps a Curve to a new range and/or restyles its color  
*int minX, minY, maxX, maxY* - the bounds of the curve.  
*float r, g, b* - colour values used for display.  
  

- **[Blend2D]**  
Displays a 2D trackpad-like interface/graph for Vector2 values. Has support for Unity.Mathematics float2.  
*string xLabel, yLabel* - Labels for the X and Y axes of the graph.  
*float minX, minY, maxX, maxY* - The bounds of the graph.  


- **[HelpBox]**  
Decorates a field with a help box.


- **[File]**
- **[Directory]**  
Styles string fields with a button to pick files/directories.  
*bool fileIsLocalToProject* - Constrains the selection to be within the Assets directory.  

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