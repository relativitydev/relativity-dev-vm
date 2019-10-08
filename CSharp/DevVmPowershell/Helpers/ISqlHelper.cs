namespace Helpers
{
	public interface ISqlHelper
	{
		bool DeleteAllErrors();
		int GetFileShareResourceServerArtifactId();
		void EnableDataGridOnExtractedText(string workspaceName);
		bool CreateOrAlterShrinkDbProc();
		bool RunShrinkDbProc();
		int GetErrorsCount();
		void InsertRsmfViewerOverride();
	}
}
