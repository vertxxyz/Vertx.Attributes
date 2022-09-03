using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
#if UNITY_MATHEMATICS
using Unity.Mathematics;
#endif

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(Blend2DAttribute))]
	public class Blend2DAttributeDrawer : PropertyDrawer
	{
		public const string UssStyleName = "vertx-blend-2d";
		public const string BoxUssStyleName = UssStyleName + "__box";
		public const string RightUssStyleName = UssStyleName + "__right";
		public const string LabelUssStyleName = UssStyleName + "__label";
		
		private const float blendBoxSize = 150f;

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var root = new VisualElement();
			root.AddToClassList(UssStyleName);
			root.AddToClassList(BaseField<int>.ussClassName);
			
			var box = new VisualElement();
			box.AddToClassList(BoxUssStyleName);
			root.Add(box);

			var right = new VisualElement();
			right.AddToClassList(RightUssStyleName);
			root.Add(right);

			var label = new Label(property.displayName);
			label.AddToClassList(LabelUssStyleName);
			right.Add(label);
			
			return root;
		}

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
							GenericMenu menu = new GenericMenu();

							menu.AddItem(new GUIContent("Center"), false, () =>
							{
#if UNITY_MATHEMATICS
								if (IsFloat2())
								{
									SerializedProperty x = property.FindPropertyRelative("x");
									SerializedProperty y = property.FindPropertyRelative("y");
									Vector2 value = Vector2.Lerp(b2D.Min, b2D.Max, 0.5f);
									x.floatValue = value.x;
									y.floatValue = value.y;
									property.serializedObject.ApplyModifiedProperties();
									return;
								}
#endif
								property.vector2Value = Vector2.Lerp(b2D.Min, b2D.Max, 0.5f);
								property.serializedObject.ApplyModifiedProperties();
							});

							menu.ShowAsContext();
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
					const float quarter = 0.25f * blendBoxSize;
					const float half = quarter * 2;
					const float threeQuarters = half + quarter;
					float lowGreyVal = EditorGUIUtility.isProSkin ? 0.25f : 0.75f;
					Color lowGrey = new Color(lowGreyVal, lowGreyVal, lowGreyVal);
					DrawLineFast(new Vector2(blendRect.x + quarter, blendRect.y), new Vector2(blendRect.x + quarter, blendRect.y + blendBoxSize), lowGrey);
					DrawLineFast(new Vector2(blendRect.x + quarter, blendRect.y), new Vector2(blendRect.x + quarter, blendRect.y + blendBoxSize), lowGrey);
					DrawLineFast(new Vector2(blendRect.x + threeQuarters, blendRect.y), new Vector2(blendRect.x + threeQuarters, blendRect.y + blendBoxSize), lowGrey);
					DrawLineFast(new Vector2(blendRect.x, blendRect.y + quarter), new Vector2(blendRect.x + blendBoxSize, blendRect.y + quarter), lowGrey);
					DrawLineFast(new Vector2(blendRect.x, blendRect.y + threeQuarters), new Vector2(blendRect.x + blendBoxSize, blendRect.y + threeQuarters), lowGrey);
					DrawLineFast(new Vector2(blendRect.x + half, blendRect.y), new Vector2(blendRect.x + half, blendRect.y + blendBoxSize), Color.grey);
					DrawLineFast(new Vector2(blendRect.x, blendRect.y + half), new Vector2(blendRect.x + blendBoxSize, blendRect.y + half), Color.grey);
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
					DrawCircleFast(blendRect.position + circlePos, blendBoxSize * 0.04f, 2, new Color(1, 0.5f, 0));
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

		private static readonly Action<CompareFunction> ApplyWireMaterialDelegate =
			(Action<CompareFunction>)Delegate.CreateDelegate(
				typeof(Action<CompareFunction>),
				typeof(HandleUtility).GetMethod(
					"ApplyWireMaterial",
					BindingFlags.NonPublic | BindingFlags.Static,
					null,
					new[] { typeof(CompareFunction) },
					null
				)
			);
	}
}