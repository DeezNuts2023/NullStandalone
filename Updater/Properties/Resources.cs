using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Updater.Properties
{
	// Token: 0x0200000C RID: 12
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x0600005A RID: 90 RVA: 0x000020C2 File Offset: 0x000002C2
		internal Resources()
		{
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00002414 File Offset: 0x00000614
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("Updater.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00002440 File Offset: 0x00000640
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00002447 File Offset: 0x00000647
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x0400002B RID: 43
		private static ResourceManager resourceMan;

		// Token: 0x0400002C RID: 44
		private static CultureInfo resourceCulture;
	}
}
