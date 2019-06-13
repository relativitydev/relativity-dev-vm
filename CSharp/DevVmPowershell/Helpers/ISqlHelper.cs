﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
	public interface ISqlHelper
	{
		bool DeleteAllErrors();
		int GetFileShareResourceServerArtifactId();
		void EnableDataGridOnExtractedText(string workspaceName);
	}
}
