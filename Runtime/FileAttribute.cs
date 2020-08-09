using UnityEngine;

namespace Vertx.Attributes
{
	public class FileAttribute : PropertyAttribute
	{
		public readonly bool FileIsLocalToProject;

		public FileAttribute(bool fileIsLocalToProject) => FileIsLocalToProject = fileIsLocalToProject;
	}
}