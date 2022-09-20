using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
#if UNITY_MATHEMATICS
using Unity.Mathematics;
#endif

namespace Vertx.Attributes.Editor
{
#if UNITY_2021_1_OR_NEWER
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

		public IBinding binding { get; set; }
		public string bindingPath { get; set; }
	}
#endif
	
	[CustomPropertyDrawer(typeof(Blend2DAttribute))]
	public sealed class Blend2DAttributeDrawer : PropertyDrawer
	{
		private const float blendBoxSize = 151f;
		public const float CircleRadius = blendBoxSize * 0.04f;

		public static Color LowGrey
		{
			get
			{
				float lowGreyVal = EditorGUIUtility.isProSkin ? 0.3f : 0.7f;
				Color lowGrey = new Color(lowGreyVal, lowGreyVal, lowGreyVal);
				return lowGrey;
			}
		}

		public static Color CircleColor => new Color(1, 0.5f, 0);

#if UNITY_2021_1_OR_NEWER
		public const string UssStyleName = "vertx-blend-2d";
		public const string RightUssStyleName = UssStyleName + "__right";
		public const string LabelUssStyleName = UssStyleName + "__label";
		public const string FieldDraggerUssStyleName = UssStyleName + "__field-dragger";
		public const string SmallLabelUssStyleName = LabelUssStyleName + "--small";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			Blend2DAttribute b2D = (Blend2DAttribute)attribute;
			var xLimit = new Vector2(b2D.Min.x, b2D.Max.x);
			var yLimit = new Vector2(b2D.Min.y, b2D.Max.y);
			SerializedProperty x = property.FindPropertyRelative("x");
			SerializedProperty y = property.FindPropertyRelative("y");


			var root = new VisualElement();
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

				void Clamp(ChangeEvent<float> evt, Vector2 args)
				{
					var floatField = (FloatField)evt.target;
					if (evt.newValue <= args.x)
						floatField.SetValueWithoutNotify(args.x);
					else if (evt.newValue >= args.y)
						floatField.SetValueWithoutNotify(args.y);
				}
			}

			return root;
		}
#endif

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Blend2DAttribute b2D = (Blend2DAttribute)attribute;
			using (new EditorGUI.PropertyScope(position, GUIContent.none, property))
				Do2DBlend(position, property, b2D);
		}

		private int hotControl = -1;
		private readonly int hash = "Blend2D".GetHashCode();

		private void Do2DBlend(Rect r, SerializedProperty property, Blend2DAttribute b2D)
		{
			Profiler.BeginSample(nameof(Do2DBlend));
			GUI.BeginGroup(r);
			try
			{
				Rect blendRect = new Rect(Vector2.zero, Vector2.one * blendBoxSize);

				Event e = Event.current;
				if (e.isMouse && e.button == 0 && e.rawType == EventType.MouseDown)
				{
					if (blendRect.Contains(e.mousePosition))
					{
						hotControl = GUIUtility.GetControlID(hash, FocusType.Passive, blendRect);
						GUIUtility.hotControl = hotControl;
						e.Use();
					}
				}

				if (e.isMouse && e.rawType == EventType.MouseUp)
				{
					switch (e.button)
					{
						case 0:
							hotControl = -1;
							GUIUtility.hotControl = 0;
							e.Use();
							break;
						case 1:
							if (!blendRect.Contains(e.mousePosition))
								break;
							SerializedProperty x = property.FindPropertyRelative("x");
							SerializedProperty y = property.FindPropertyRelative("y");
							ShowContextMenu(
								x, y,
								new Vector2(b2D.Min.x, b2D.Max.x),
								new Vector2(b2D.Min.y, b2D.Max.y)
							);
							e.Use();
							break;
					}
				}

				if (GUIUtility.hotControl == hotControl)
				{
					if (e.isMouse)
					{
						Vector2 value = Lerp(
							b2D.Min,
							b2D.Max,
							new Vector2(e.mousePosition.x / blendBoxSize, (blendBoxSize - e.mousePosition.y) / blendBoxSize));
#if UNITY_MATHEMATICS
						if (IsFloat2())
						{
							SerializedProperty x = property.FindPropertyRelative("x");
							SerializedProperty y = property.FindPropertyRelative("y");
							x.floatValue = value.x;
							y.floatValue = value.y;
						}
						else
#endif
						{
							property.vector2Value = value;
						}

						e.Use();
					}
				}

				GUI.Box(blendRect, "", EditorStyles.helpBox);

				if (e.type == EventType.Repaint)
				{
					GL.Begin(Application.platform == RuntimePlatform.WindowsEditor ? GL.QUADS : GL.LINES);
					ApplyWireMaterialDelegate.Invoke(CompareFunction.Always);
					
					const float quarter = 0.25f * blendBoxSize,
						half = quarter * 2,
						threeQuarters = half + quarter;
					
					Color lowGrey = LowGrey;
					Color grey = Color.grey;
					DrawLineFast(new Vector2(blendRect.x + quarter, blendRect.y), new Vector2(blendRect.x + quarter, blendRect.y + blendBoxSize), lowGrey);
					DrawLineFast(new Vector2(blendRect.x + quarter, blendRect.y), new Vector2(blendRect.x + quarter, blendRect.y + blendBoxSize), lowGrey);
					DrawLineFast(new Vector2(blendRect.x + threeQuarters, blendRect.y), new Vector2(blendRect.x + threeQuarters, blendRect.y + blendBoxSize), lowGrey);
					DrawLineFast(new Vector2(blendRect.x, blendRect.y + quarter), new Vector2(blendRect.x + blendBoxSize, blendRect.y + quarter), lowGrey);
					DrawLineFast(new Vector2(blendRect.x, blendRect.y + threeQuarters), new Vector2(blendRect.x + blendBoxSize, blendRect.y + threeQuarters), lowGrey);
					DrawLineFast(new Vector2(blendRect.x + half, blendRect.y), new Vector2(blendRect.x + half, blendRect.y + blendBoxSize), grey);
					DrawLineFast(new Vector2(blendRect.x, blendRect.y + half), new Vector2(blendRect.x + blendBoxSize, blendRect.y + half), grey);
					GL.End();

					GUI.Label(new Rect(blendRect.x + quarter, blendRect.y + half, blendBoxSize, 15), b2D.XLabel, EditorStyles.centeredGreyMiniLabel);
					Matrix4x4 matrixP = GUI.matrix;
					GUIUtility.RotateAroundPivot(-90, new Vector2(blendRect.x, blendRect.y) + new Vector2(half, half));
					GUI.Label(new Rect(quarter, half - 15, blendBoxSize, 15), b2D.YLabel, EditorStyles.centeredGreyMiniLabel);
					GUI.matrix = matrixP;

					GL.Begin(Application.platform == RuntimePlatform.WindowsEditor ? GL.QUADS : GL.LINES);
					ApplyWireMaterialDelegate.Invoke(CompareFunction.Always);
					Vector2 circlePos = InverseLerp(b2D.Min, b2D.Max, property.vector2Value);
					circlePos.y = 1 - circlePos.y;
					circlePos *= blendBoxSize;
					DrawCircleFast(blendRect.position + circlePos, CircleRadius, 2, CircleColor);
					GL.End();
				}

#if UNITY_MATHEMATICS
				bool IsFloat2() => property.propertyType == SerializedPropertyType.Generic && property.type == nameof(float2);

				Vector2 v;
				if (IsFloat2())
				{
					SerializedProperty x = property.FindPropertyRelative("x");
					SerializedProperty y = property.FindPropertyRelative("y");
					v = new Vector2(x.floatValue, y.floatValue);
					// ReSharper disable once ConvertToUsingDeclaration
					using (EditorGUI.ChangeCheckScope cC = new EditorGUI.ChangeCheckScope())
					{
						Rect labelRect = new Rect(blendBoxSize + 5, blendBoxSize / 2f - 50, Screen.width - blendBoxSize - 5, EditorGUIUtility.singleLineHeight);
						EditorGUI.LabelField(labelRect, property.displayName, EditorStyles.boldLabel);
						labelRect.y += 20;
						EditorGUI.LabelField(labelRect, b2D.XLabel);
						labelRect.y += EditorGUIUtility.singleLineHeight;
						v.x = EditorGUI.DelayedFloatField(labelRect, v.x);
						labelRect.y += EditorGUIUtility.singleLineHeight;
						EditorGUI.LabelField(labelRect, b2D.YLabel);
						labelRect.y += EditorGUIUtility.singleLineHeight;
						v.y = EditorGUI.DelayedFloatField(labelRect, v.y);
						if (cC.changed)
						{
							x.floatValue = Mathf.Clamp(v.x, b2D.Min.x, b2D.Max.x);
							y.floatValue = Mathf.Clamp(v.y, b2D.Min.y, b2D.Max.y);
						}
					}

					return;
				}

				v = property.vector2Value;
#else
				Vector2 v = property.vector2Value;
#endif

				using (EditorGUI.ChangeCheckScope cC = new EditorGUI.ChangeCheckScope())
				{
					Rect labelRect = new Rect(blendBoxSize + 5, blendBoxSize / 2f - 50, Screen.width - blendBoxSize - 5, EditorGUIUtility.singleLineHeight);
					EditorGUI.LabelField(labelRect, property.displayName, EditorStyles.boldLabel);
					labelRect.y += 20;
					EditorGUI.LabelField(labelRect, b2D.XLabel);
					labelRect.y += EditorGUIUtility.singleLineHeight;
					v.x = EditorGUI.DelayedFloatField(labelRect, v.x);
					labelRect.y += EditorGUIUtility.singleLineHeight;
					EditorGUI.LabelField(labelRect, b2D.YLabel);
					labelRect.y += EditorGUIUtility.singleLineHeight;
					v.y = EditorGUI.DelayedFloatField(labelRect, v.y);
					if (cC.changed)
						property.vector2Value = new Vector2(Mathf.Clamp(v.x, b2D.Min.x, b2D.Max.x), Mathf.Clamp(v.y, b2D.Min.y, b2D.Max.y));
				}
			}
			finally
			{
				GUI.EndGroup();
				Profiler.EndSample();
			}
		}

		public static void ShowContextMenu(SerializedProperty x, SerializedProperty y, Vector2 xLimit, Vector2 yLimit)
		{
			GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("Center"), false, () =>
			{
				x.floatValue = Mathf.Lerp(xLimit.x, xLimit.y, 0.5f);
				y.floatValue = Mathf.Lerp(yLimit.x, yLimit.y, 0.5f);
				x.serializedObject.ApplyModifiedProperties();
			});

			menu.ShowAsContext();
		}

		static Vector2 InverseLerp(Vector2 min, Vector2 max, Vector2 value) =>
			new Vector2(
				Mathf.InverseLerp(min.x, max.x, value.x),
				Mathf.InverseLerp(min.y, max.y, value.y));

		static Vector2 Lerp(Vector2 min, Vector2 max, Vector2 value) =>
			new Vector2(
				Mathf.Lerp(min.x, max.x, value.x),
				Mathf.Lerp(min.y, max.y, value.y));

		private static void DrawLineFast(Vector2 from, Vector2 to, Color color)
		{
			// Align perfectly along a pixel
			from.x = Mathf.Ceil(from.x) - 0.5f;
			from.y = Mathf.Ceil(from.y) - 0.5f;
			to.x = Mathf.Ceil(to.x) - 0.5f;
			to.y = Mathf.Ceil(to.y) - 0.5f;
			
			GL.Color(color);
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				Vector2 tangent = (to - from).normalized;
				Vector2 mult = new Vector2(tangent.y > tangent.x ? -1 : 1, tangent.y > tangent.x ? 1 : -1);
				tangent = new Vector2(mult.x * tangent.y, mult.y * tangent.x) * 0.25f;
				GL.Vertex(new Vector3(from.x + tangent.x, from.y + tangent.y, 0f));
				GL.Vertex(new Vector3(from.x - tangent.x, from.y - tangent.y, 0f));
				GL.Vertex(new Vector3(to.x - tangent.x, to.y - tangent.y, 0f));
				GL.Vertex(new Vector3(to.x + tangent.x, to.y + tangent.y, 0f));
			}
			else
			{
				GL.Vertex(new Vector3(from.x, from.y, 0f));
				GL.Vertex(new Vector3(to.x, to.y, 0f));
			}
		}

		private const int circleDivisions = 18;

		private static void DrawCircleFast(Vector2 position, float radius, float thickness, Color color)
		{
			GL.Color(color);
			for (int i = 1; i <= circleDivisions; i++)
			{
				float vC = Mathf.PI * 2 * (i / (float)circleDivisions - 1 / (float)circleDivisions);
				float vP = Mathf.PI * 2 * (i / (float)circleDivisions);
				Vector2 from = position + new Vector2(Mathf.Sin(vP) * radius, Mathf.Cos(vP) * radius);
				Vector2 to = position + new Vector2(Mathf.Sin(vC) * radius, Mathf.Cos(vC) * radius);
				if (Application.platform == RuntimePlatform.WindowsEditor)
				{
					Vector2 tangent = (to - from).normalized;
					Vector2 mult = new Vector2(tangent.y > tangent.x ? -1 : 1, tangent.y > tangent.x ? 1 : -1);
					tangent = new Vector2(mult.x * tangent.y, mult.y * tangent.x) * (0.5f * thickness);
					GL.Vertex(new Vector3(from.x + tangent.x, from.y + tangent.y, 0f));

					GL.Vertex(new Vector3(to.x + tangent.x, to.y + tangent.y, 0f));
					GL.Vertex(new Vector3(to.x - tangent.x, to.y - tangent.y, 0f));
					GL.Vertex(new Vector3(from.x - tangent.x, from.y - tangent.y, 0f));
				}
				else
				{
					GL.Vertex(new Vector3(from.x, from.y, 0f));
					GL.Vertex(new Vector3(to.x, to.y, 0f));
				}
			}
		}


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => blendBoxSize;

		private static Action<CompareFunction> s_ApplyWireMaterialDelegate;
		private static Action<CompareFunction> ApplyWireMaterialDelegate
			=> s_ApplyWireMaterialDelegate
			   ?? (s_ApplyWireMaterialDelegate = (Action<CompareFunction>)Delegate.CreateDelegate(
				   typeof(Action<CompareFunction>),
				   typeof(HandleUtility).GetMethod(
					   "ApplyWireMaterial",
					   BindingFlags.NonPublic | BindingFlags.Static,
					   null,
					   new[] { typeof(CompareFunction) },
					   null
				   )
			   ));
	}
}