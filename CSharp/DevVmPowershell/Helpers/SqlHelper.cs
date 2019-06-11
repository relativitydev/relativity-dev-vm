using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.API;

namespace Helpers
{
	public class SqlHelper : ISqlHelper
	{
		private IConnectionHelper ConnectionHelper { get; set; }
		private IDBContext DbContext { get; set; }

		public SqlHelper(IConnectionHelper connectionHelper)
		{
			ConnectionHelper = connectionHelper;
			DbContext = connectionHelper.GetDbContext(-1);
		}

		public void DeleteErrorsFromErrorsTab()
		{
			string sql = "DELETE FROM [eddsdbo].[Errors]";

			try
			{
				DbContext.ExecuteNonQuerySQLStatement(sql);
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to Delete all Errors from the Errors Tab");
			}
		}
	}
}
