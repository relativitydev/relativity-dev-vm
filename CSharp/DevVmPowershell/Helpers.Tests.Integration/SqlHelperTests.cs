using NUnit.Framework;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class SqlHelperTests
	{
		private ISqlHelper Sut { get; set; }

		[SetUp]
		public void Setup()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				relativityInstanceName: TestConstants.RELATIVITY_INSTANCE_NAME,
				relativityAdminUserName: TestConstants.RELATIVITY_ADMIN_USER_NAME,
				relativityAdminPassword: TestConstants.RELATIVITY_ADMIN_PASSWORD,
				sqlAdminUserName: TestConstants.SQL_USER_NAME,
				sqlAdminPassword: TestConstants.SQL_PASSWORD);
			ISqlRunner sqlRunner = new SqlRunner(connectionHelper);
			Sut = new SqlHelper(sqlRunner);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void DeleteAllErrorsTest()
		{
			// Act
			bool result = Sut.DeleteAllErrors(Constants.Connection.Sql.EDDS_DATABASE);
			int errorsCount = Sut.GetErrorsCount(Constants.Connection.Sql.EDDS_DATABASE);

			// Assert
			Assert.True(errorsCount == 0);
		}

		[Test]
		public void GetFileShareResourceServerArtifactIdTest()
		{
			// Act
			int fileShareResourceServerArtifactId = Sut.GetFileShareResourceServerArtifactId(Constants.Connection.Sql.EDDS_DATABASE);

			// Assert
			Assert.That(fileShareResourceServerArtifactId, Is.GreaterThan(0));
		}

		[Test]
		public void EnableDataGridOnExtractedTextTest()
		{
			// Arrange
			string workspaceName = ""; // To Test set workspace name of DataGrid Enabled workspace

			// Act
			// Assert
			Assert.DoesNotThrow(() => Sut.EnableDataGridOnExtractedText(Constants.Connection.Sql.EDDS_DATABASE, workspaceName));
		}

		[Test]
		public void CreateOrAlterShrinkDbProcTest()
		{
			// Arrange

			// Act

			// Assert
			Assert.That(Sut.CreateOrAlterShrinkDbProc(Constants.Connection.Sql.EDDS_DATABASE), Is.True);
		}

		[Test]
		public void RunShrinkDbProcTest()
		{
			// Arrange

			// Act

			// Assert
			Assert.That(Sut.RunShrinkDbProc(Constants.Connection.Sql.EDDS_DATABASE), Is.True);
		}

		[Test]
		public void InsertRsmfViewerOverrideTest()
		{
			// Arrange

			// Act

			// Assert
			Assert.DoesNotThrow(() => Sut.InsertRsmfViewerOverride(Constants.Connection.Sql.EDDS_DATABASE));
		}
	}
}
