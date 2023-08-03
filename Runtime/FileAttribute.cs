using UnityEngine;

namespace Vertx.Attributes
{
	/// <summary>
	/// Provide a text field that operates like a button which opens a file dialog
	/// A help box is shown when the resulting string is invalid
	/// </summary>
	public sealed class FileAttribute : PropertyAttribute
	{
		public readonly bool FileIsLocalToProject;

		/// <summary>
		/// Provide a text field that operates like a button which opens a file dialog
		/// A help box is shown when the resulting string is invalid
		/// </summary>
		/// <param name="fileIsLocalToProject">Restricts selection to the Assets directory.</param>
		public FileAttribute(bool fileIsLocalToProject) => FileIsLocalToProject = fileIsLocalToProject;
	}
}