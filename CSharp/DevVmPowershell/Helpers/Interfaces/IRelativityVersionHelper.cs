﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Interfaces
{
	public interface IRelativityVersionHelper
	{
		void ConfirmInstallerAndInstanceRelativityVersionAreEqual(string installerRelativityVersion);
	}
}