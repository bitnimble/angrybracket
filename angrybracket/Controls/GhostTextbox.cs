using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AngryBracket
{
	public class GhostTextbox : TextBox
	{
		private string ghostText;
		public string GhostText
		{
			get { return ghostText; }
			set { ghostText = value; updateGhostText(); }
		}

		public GhostTextbox() { }

		public GhostTextbox(string ghostText)
		{
			GhostText = ghostText;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			updateGhostText();
		}

		private void updateGhostText()
		{
			if (!this.IsHandleCreated || string.IsNullOrWhiteSpace(ghostText))
				return;

			IntPtr mem = Marshal.StringToHGlobalUni(ghostText);
			NativeMethods.SendMessage(this.Handle, 0x1501, (IntPtr)1, mem);
			Marshal.FreeHGlobal(mem);
		}
	}
}
