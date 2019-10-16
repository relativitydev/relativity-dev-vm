namespace Helpers.Interfaces
{
	public interface ISqlHelper
	{
		bool DeleteAllErrors(string sqlDatabaseName);
		int GetFileShareResourceServerArtifactId(string sqlDatabaseName);
		void EnableDataGridOnExtractedText(string sqlDatabaseName, string workspaceName);
		bool CreateOrAlterShrinkDbProc(string sqlDatabaseName);
		bool RunShrinkDbProc(string sqlDatabaseName);
		int GetErrorsCount(string sqlDatabaseName);
		void InsertRsmfViewerOverride(string sqlDatabaseName);
	}
}
