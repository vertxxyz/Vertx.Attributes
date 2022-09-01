using UnityEngine;

namespace Vertx.Attributes
{
	/// <summary>
	/// Provide a text field that operates like a button which opens a directory dialog
	/// A help box is shown when the resulting string is invalid
	/// </summary>
	public class DirectoryAttribute : PropertyAttribute
	{
		public readonly bool DirectoryIsLocalToProject;

		/// <summary>
		/// Provide a text field that operates like a button which opens a directory dialog
		/// A help box is shown when the resulting string is invalid
		/// </summary>
		/// <param name="directoryIsLocalToProject">Restricts selection to the Assets directory.</param>
		public DirectoryAttribute(bool directoryIsLocalToProject) => DirectoryIsLocalToProject = directoryIsLocalToProject;
	}
}