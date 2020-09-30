using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.Rendering;

namespace Vertx.Attributes.Editor {
	[CustomPropertyDrawer(typeof(Blend2DAttribute))]
	public class Blend2DAttributeDrawer : PropertyDrawer
	{
		private const float blendBoxSize = 150f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Blend2DAttribute b2D = (Blend2DAttribute) attribute;
			using(new EditorGUI.PropertyScope(position, GUIContent.none, property))
				Do2DBlend(position, property, b2D);
		}

		private int hotControl = -1;
		private readonly int hash = "Blend2D".GetHashCode();

		private void Do2DBlend(Rect r, SerializedProperty property, Blend2DAttribute b2D)
		{
			using (new GUI.GroupScope(r))
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
					switch (e.button) {
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
						property.vector2Value =
							Lerp(
								b2D.Min,
								b2D.Max,
								new Vector2(e.mousePosition.x / blendBoxSize, (blendBoxSize - e.mousePosition.y) / blendBoxSize));
						e.Use();
					}
				}
			
				using (new GUI.GroupScope(blendRect, EditorStyles.helpBox))
				{
					if (e.type == EventType.Repaint)
					{
						GL.Begin(Application.platform == RuntimePlatform.WindowsEditor ? GL.QUADS : GL.LINES);
						ApplyWireMaterial.Invoke(null, new object[] {CompareFunction.Always});
						const float quarter = 0.25f * blendBoxSize;
						const float half = quarter * 2;
						const float threeQuarters = half + quarter;
						float lowGreyVal = EditorGUIUtility.isProSkin ? 0.25f : 0.75f;
						Color lowGrey = new Color(lowGreyVal, lowGreyVal, lowGreyVal);
						DrawLineFast(new Vector2(quarter, 0), new Vector2(quarter, blendBoxSize), lowGrey);
						DrawLineFast(new Vector2(quarter, 0), new Vector2(quarter, blendBoxSize), lowGrey);
						DrawLineFast(new Vector2(threeQuarters, 0), new Vector2(threeQuarters, blendBoxSize), lowGrey);
						DrawLineFast(new Vector2(0, quarter), new Vector2(blendBoxSize, quarter), lowGrey);
						DrawLineFast(new Vector2(0, threeQuarters), new Vector2(blendBoxSize, threeQuarters), lowGrey);
						DrawLineFast(new Vector2(half, 0), new Vector2(half, blendBoxSize), Color.grey);
						DrawLineFast(new Vector2(0, half), new Vector2(blendBoxSize, half), Color.grey);
						GL.End();

						GUI.Label(new Rect(0, blendBoxSize - 15, blendBoxSize, 15), b2D.XLabel, EditorStyles.centeredGreyMiniLabel);
						Matrix4x4 matrixP = GUI.matrix;
						GUIUtility.RotateAroundPivot(-90, new Vector2(0, blendBoxSize));
						GUI.Label(new Rect(0, blendBoxSize - 15, blendBoxSize, 15), b2D.YLabel, EditorStyles.centeredGreyMiniLabel);
						GUI.matrix = matrixP;

						GL.Begin(Application.platform == RuntimePlatform.WindowsEditor ? GL.QUADS : GL.LINES);
						ApplyWireMaterial.Invoke(null, new object[] {CompareFunction.Always});
						Vector2 circlePos = InverseLerp(b2D.Min, b2D.Max, property.vector2Value);
						circlePos.y = 1 - circlePos.y;
						circlePos *= blendBoxSize;
						DrawCircleFast(circlePos, blendBoxSize * 0.04f, 2, new Color(1, 0.5f, 0));
						GL.End();
					}
				}

				Vector2 v = property.vector2Value;
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
				tangent = new Vector2(mult.x * tangent.y, mult.y * tangent.x) * 0.5f;
				GL.Vertex(new Vector3(from.x + tangent.x, from.y + tangent.y, 0f));
				GL.Vertex(new Vector3(from.x - tangent.x, from.y - tangent.y, 0f));
				GL.Vertex(new Vector3(to.x + tangent.x, to.y + tangent.y, 0f));
				GL.Vertex(new Vector3(to.x - tangent.x, to.y - tangent.y, 0f));
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
				float vC = Mathf.PI * 2 * (i / (float) circleDivisions - 1 / (float) circleDivisions);
				float vP = Mathf.PI * 2 * (i / (float) circleDivisions);
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

		private MethodInfo applyWireMaterial;

		private MethodInfo ApplyWireMaterial =>
			applyWireMaterial ?? (applyWireMaterial =
				typeof(HandleUtility).GetMethod("ApplyWireMaterial", BindingFlags.NonPublic | BindingFlags.Static, null, new[] {typeof(CompareFunction)}, null));
	}
}