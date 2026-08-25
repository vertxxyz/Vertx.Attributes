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
		public readonly string Extension;

		/// <summary>
		/// Provide a text field that operates like a button which opens a file dialog
		/// A help box is shown when the resulting string is invalid
		/// </summary>
		/// <param name="fileIsLocalToProject">Restricts selection to the Assets directory.</param>
		/// <param name="extension">An optional default extension filter. Do not precede file extension names with a period. Enter an empty string to include all file types. Separate multiple file extensions with a comma.</param>
		public FileAttribute(bool fileIsLocalToProject, string extension = "")
		{
			FileIsLocalToProject = fileIsLocalToProject;
			Extension = extension;
		}
	}
}