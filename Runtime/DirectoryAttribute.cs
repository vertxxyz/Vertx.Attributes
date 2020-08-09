using UnityEngine;

namespace Vertx.Attributes
{
	public class DirectoryAttribute : PropertyAttribute
	{
		public readonly bool DirectoryIsLocalToProject;

		public DirectoryAttribute(bool directoryIsLocalToProject) => DirectoryIsLocalToProject = directoryIsLocalToProject;
	}
}