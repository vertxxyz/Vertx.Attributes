# Vertx.Attributes
Attributes and Property Drawers/Decorators for Unity

Attributes has a dependency on [Utilities](https://github.com/vertxxyz/Vertx.Utilities) so ensure that is referenced into your project to use this package successfully.

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
A Range slider for Vector2, Vector2Int.  
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
Displays a 2D trackpad-like interface/graph for Vector2 values.  
*string xLabel, yLabel* - Labels for the X and Y axes of the graph.  
*float minX, minY, maxX, maxY* - The bounds of the graph.  


- **[File]**
- **[Directory]**  
Styles string fields with a button to pick files/directories.  
*bool fileIsLocalToProject* - Constrains the selection to be within the Assets directory.  