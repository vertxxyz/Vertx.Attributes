# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.7.0]
### Added
- `[Layer]`, for ints representing a single layer.

## [1.6.0]
### Changed
- Renamed `[ReadOnly]` to `[ReadOnlyField]` to avoid naming conflicts with the Collections namespace.
- Renamed `[EditorOnly]` to `[EditorOnlyField]` because of the above change.

## [1.5.2] - 2023-04-11
- Fixed [Inline] UIToolkit drawer drawing fields in reverse order.

## [1.5.1] - 2023-02-23
- Fixed certain UIToolkit fields missing styles when created alone.

## [1.5.0] - 2022-09-01
- Added UIToolkit support to all property drawers. Support may not be present in all Unity versions.
- Added a Height property to CurveDisplay, allowing for larger display of the curve field.
- Added [EnumDropdown], this shows an AdvancedDropdown instead of a GenericMenu, allowing for scrolling in long enums.
- Added [Inline], removes the foldout from a group of serialized fields.

## [1.4.1] - 2022-08-28
- Added Relabel drawer.
- Fixed issue with UIToolkit EditorOnly drawer.

## [1.4.0] - 2022-07-04
- Added HideObsoleteNames to EnumFlagsAttribute (defaults to true).
- Added InspectorNameAttribute to EnumFlagsAttribute.
- Fixed general issues with Blend2DAttribute.
- Added UIToolkit version of ReadOnly and EditorOnly attributes.

## [1.3.5] - 2021-07-26
- MinMax attribute supports Unity.Mathematics int2 and float2.
- EnumFlags attribute supports enums with an underlying short type.

## [1.3.4] - 2021-07-25
- EnumFlags attribute supports enums with an underlying byte type.

## [1.3.3] - 2021-07-23
- Improvements to EnumFlags attribute.
   - Improved display of flags with multiple values.
   - Improved display of large enums.

## [1.3.2] - 2021-07-23
- Fixed issues with EnumFlags attribute.
   - Supports non-sequential flags.
   - Better supports flags with multiple values.
- Fixed issue where Blend2D drawer shows incorrect interface.

## [1.3.1] - 2021-03-25
- Removed dependency on Vertx.Utilities.

## [1.3.0] - 2020-10-11
- Fixed indent issues associated with MinMax. Requires Vertx.Utilities v2.1.2.

## [1.2.4] - 2020-10-10
- Added HelpBoxAttribute for decorating a field with a help box.

## [1.2.3] - 2020-09-30
- Added property styling for Blend2D and MinMax to enable better behaviour with prefab overrides.

## [1.2.2] - 2020-09-18
- Fixes for File and Directory attributes throwing exceptions in 2020 and having overflowing buttons.
- MinMaxAttribute fixes:
	- Max field now properly uses int values.
	- Max field is now a delayed field to stop zeroing out min whilst editing.
	- Label parameter is now optional. This is helpful for Vector2 fields that can derive the label from the field.
- ProgressAttribute's max value parameter is optional.

## [1.2.1] - 2020-09-12
- Added Vector support to MinMaxAttribute.

## [1.2.0] - 2020-09-12
- Removed EnumToValueDrawer. This was intended to be a part of Vertx.Utilities.

## [1.0.0]
- Initial release.