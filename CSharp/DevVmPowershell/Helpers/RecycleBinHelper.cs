using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
	public class RecycleBinHelper : IRecycleBinHelper
	{
		[DllImport("Shell32.dll")]
		static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlag dwFlags);
		enum RecycleFlag : int
		{
			SHERB_NOCONFIRMATION = 0x00000001, // No confirmation, when emptying
			SHERB_NOPROGRESSUI = 0x00000001, // No progress tracking window during the emptying of the recycle bin
			SHERB_NOSOUND = 0x00000004 // No sound when the emptying of the recycle bin is complete
		}

		public void EmptyRecycleBin()
		{
			try
			{
				SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOSOUND | RecycleFlag.SHERB_NOCONFIRMATION);
				Console.WriteLine("Recycling Bin Emptied");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred Emptying the Recycle Bin", ex);
			}
		}
	}
}
