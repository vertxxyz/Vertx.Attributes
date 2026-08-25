using System;
using JetBrains.Annotations;

namespace Vertx.Attributes
{
	/// <summary>
	/// Can be used as an anchor for attributes.<br/>
	/// For example, a Button might have no relation to a field, but you still want to display it. You can append it to this type for no runtime cost.
	/// </summary>
	[Serializable, UsedImplicitly]
	public struct Empty { }
}