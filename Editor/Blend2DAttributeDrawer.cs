using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#if UNITY_MATHEMATICS
using Unity.Mathematics;
#endif

namespace Vertx.Attributes.Editor
{
	public sealed class Blend2DBoxElement : VisualElement, IBindable
	{
		public const string BoxUssStyleName = Blend2DAttributeDrawer.UssStyleName + "__box";
		public const string BoxInteriorStyleName = BoxUssStyleName + "__interior";
		public const string BoxLabelStyleName = BoxUssStyleName + "__label";
		public const string BoxLabelXStyleName = BoxLabelStyleName + "--x";
		public const string BoxLabelYStyleName = BoxLabelStyleName + "--y";

		private readonly SerializedProperty _x;
		private readonly SerializedProperty _y;
		private readonly Vector2 _xLimit;
		private readonly Vector2 _yLimit;
		private readonly VisualElement _cursor;

		public Blend2DBoxElement(
			SerializedProperty x,
			SerializedProperty y,
			Vector2 xLimit,
			Vector2 yLimit,
			string xText,
			string yText
		)
		{
			var interior = new VisualElement { pickingMode = PickingMode.Ignore };
			interior.AddToClassList(BoxInteriorStyleName);
			interior.generateVisualContent += GenerateVisualContentInterior;
			Add(interior);
			
			var xLabel = new Label(xText) { pickingMode = PickingMode.Ignore };
			xLabel.AddToClassList(BoxLabelStyleName);
			xLabel.AddToClassList(BoxLabelXStyleName);
			Add(xLabel);

			var yLabel = new Label(yText) { pickingMode = PickingMode.Ignore };
			yLabel.AddToClassList(BoxLabelStyleName);
			yLabel.AddToClassList(BoxLabelYStyleName);
			Add(yLabel);
			
			_cursor = new VisualElement { pickingMode = PickingMode.Ignore };
			_cursor.AddToClassList(BoxInteriorStyleName);
			_cursor.generateVisualContent += GenerateVisualContentCursor;
			Add(_cursor);

			_x = x;
			_y = y;
			_xLimit = xLimit;
			_yLimit = yLimit;
			AddToClassList(BoxUssStyleName);
			this.TrackPropertyValue(x, _ => _cursor.MarkDirtyRepaint());
			this.TrackPropertyValue(y, _ => _cursor.MarkDirtyRepaint());

			RegisterCallback<PointerDownEvent, Blend2DBoxElement>((evt, args) =>
			{
				switch (evt.button)
				{
					case 0:
						args.CapturePointer(evt.pointerId);
						PositionOnClick(args, evt.localPosition);
						break;
					case 1:
						Blend2DAttributeDrawer.ShowContextMenu(
							args._x, args._y,
							args._xLimit,
							args._yLimit
						);
						break;
					default:
						return;
				}

				evt.StopPropagation();
			}, this);
			RegisterCallback<PointerMoveEvent, Blend2DBoxElement>((evt, args) =>
			{
				if (!args.HasPointerCapture(evt.pointerId))
					return;
				PositionOnClick(args, evt.localPosition);
				evt.StopPropagation();
			}, this);
			RegisterCallback<PointerUpEvent, Blend2DBoxElement>((evt, args) =>
			{
				args.ReleasePointer(evt.pointerId);
				evt.StopPropagation();
			}, this);
		}

		private static void PositionOnClick(Blend2DBoxElement element, Vector2 localPosition)
		{
			element._x.floatValue =
				Mathf.Lerp(
					element._xLimit.x,
					element._xLimit.y,
					Mathf.InverseLerp(0, element.layout.width, localPosition.x)
				);
			element._y.floatValue =
				Mathf.Lerp(
					element._yLimit.x,
					element._yLimit.y,
					1 - Mathf.InverseLerp(0, element.layout.height, localPosition.y)
				);
			if (element._x.serializedObject.ApplyModifiedProperties())
				element._cursor.MarkDirtyRepaint();
		}

		private void GenerateVisualContentInterior(MeshGenerationContext obj)
		{
			Painter2D painter2D = obj.painter2D;
			painter2D.strokeColor = Color.grey;
			float width = layout.width;
			float height = layout.height;
			float halfHeight = height * 0.5f;
			float halfWidth = width * 0.5f;
			
			painter2D.lineWidth = 1;
			painter2D.BeginPath();
			painter2D.MoveTo(new Vector2(0, halfHeight));
			painter2D.LineTo(new Vector2(width, halfHeight));
			painter2D.MoveTo(new Vector2(halfWidth, 0));
			painter2D.LineTo(new Vector2(halfWidth, height));
			painter2D.Stroke();
			
			float quarterHeight = halfHeight * 0.5f;
			float quarterWidth = halfWidth * 0.5f;
			painter2D.strokeColor = Blend2DAttributeDrawer.LowGrey;
			painter2D.BeginPath();
			painter2D.MoveTo(new Vector2(0, quarterHeight));
			painter2D.LineTo(new Vector2(width, quarterHeight));
			painter2D.MoveTo(new Vector2(0, halfHeight + quarterHeight));
			painter2D.LineTo(new Vector2(width, halfHeight + quarterHeight));
			painter2D.MoveTo(new Vector2(quarterWidth, 0));
			painter2D.LineTo(new Vector2(quarterWidth, height));
			painter2D.MoveTo(new Vector2(halfWidth + quarterWidth, 0));
			painter2D.LineTo(new Vector2(halfWidth + quarterWidth, height));
			painter2D.Stroke();
		}
		
		private void GenerateVisualContentCursor(MeshGenerationContext obj)
		{
			Painter2D painter2D = obj.painter2D;
			painter2D.strokeColor = Color.grey;
			float width = layout.width;
			float height = layout.height;
			painter2D.lineWidth = 1;

			float xNormalised = Mathf.InverseLerp(_xLimit.x, _xLimit.y, _x.floatValue);
			float yNormalised = 1 - Mathf.InverseLerp(_yLimit.x, _yLimit.y, _y.floatValue);

			painter2D.strokeColor = Blend2DAttributeDrawer.CircleColor;
			painter2D.BeginPath();
			painter2D.Arc(
				new Vector2(xNormalised * width, yNormalised * height),
				Blend2DAttributeDrawer.CircleRadius,
				default,
				Angle.Turns(1)
			);
			painter2D.Stroke();
		}

		public IBinding? binding { get; set; }
		public string? bindingPath { get; set; }
	}
	
	[CustomPropertyDrawer(typeof(Blend2DAttribute))]
	public sealed class Blend2DAttributeDrawer : PropertyDrawer
	{
		private const float BlendBoxSize = 151f;
		public const float CircleRadius = BlendBoxSize * 0.04f;

		public static Color LowGrey
		{
			get
			{
				float lowGreyVal = EditorGUIUtility.isProSkin ? 0.3f : 0.7f;
				Color lowGrey = new Color(lowGreyVal, lowGreyVal, lowGreyVal);
				return lowGrey;
			}
		}

		public static Color CircleColor => new(1, 0.5f, 0);
		
		public const string UssStyleName = "vertx-blend-2d";
		public const string RightUssStyleName = UssStyleName + "__right";
		public const string LabelUssStyleName = UssStyleName + "__label";
		public const string FieldDraggerUssStyleName = UssStyleName + "__field-dragger";
		public const string SmallLabelUssStyleName = LabelUssStyleName + "--small";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var b2D = (Blend2DAttribute)attribute;
			var xLimit = new Vector2(b2D.Min.x, b2D.Max.x);
			var yLimit = new Vector2(b2D.Min.y, b2D.Max.y);
			SerializedProperty x = property.FindPropertyRelative("x");
			SerializedProperty y = property.FindPropertyRelative("y");


			var root = new VisualElement();
			root.RegisterCallback<AttachToPanelEvent, string>(StyleSheetUtils.AddStyleSheetOnPanelEvent, SheetPaths.AttributeStyles);
			root.AddToClassList(UssStyleName);
			root.AddToClassList(BaseField<int>.ussClassName);

			var box = new Blend2DBoxElement(x, y, xLimit, yLimit, b2D.XLabel, b2D.YLabel);
			root.Add(box);

			var right = new VisualElement();
			right.AddToClassList(RightUssStyleName);
			root.Add(right);

			var label = new Label(property.displayName);
			label.AddToClassList(BaseField<int>.ussClassName);
			label.AddToClassList(LabelUssStyleName);
			right.Add(label);

			AddProperty(b2D.XLabel, x, xLimit);
			AddProperty(b2D.YLabel, y, yLimit);

			return root;

			void AddProperty(string text, SerializedProperty p, Vector2 limits)
			{
				// This setup lets us have a non-delayed dragger, and a delayed float field.
				// The float field is delayed so our clamping doesn't mess with typing (UIToolkit is frustrating).
				var fieldDragger = new FloatField(text)
				{
					bindingPath = p.propertyPath
				};
				var field = new FloatField
				{
					bindingPath = p.propertyPath,
					isDelayed = true
				};
				fieldDragger.AddToClassList(FieldDraggerUssStyleName);
				fieldDragger.Q<Label>().AddToClassList(SmallLabelUssStyleName);
				right.Add(fieldDragger);
				right.Add(field);

				fieldDragger.RegisterCallback<ChangeEvent<float>, Vector2>(Clamp, limits);
				field.RegisterCallback<ChangeEvent<float>, Vector2>(Clamp, limits);
				return;

				void Clamp(ChangeEvent<float> evt, Vector2 args)
				{
					var floatField = (FloatField)evt.target;
					if (evt.newValue <= args.x)
						floatField.SetValueWithoutNotify(args.x);
					else if (evt.newValue >= args.y)
						floatField.SetValueWithoutNotify(args.y);
				}
			}
		}


		public static void ShowContextMenu(SerializedProperty x, SerializedProperty y, Vector2 xLimit, Vector2 yLimit)
		{
			var menu = new GenericMenu();

			menu.AddItem(new GUIContent("Center"), false, () =>
			{
				x.floatValue = Mathf.Lerp(xLimit.x, xLimit.y, 0.5f);
				y.floatValue = Mathf.Lerp(yLimit.x, yLimit.y, 0.5f);
				x.serializedObject.ApplyModifiedProperties();
			});

			menu.ShowAsContext();
		}
	}
}