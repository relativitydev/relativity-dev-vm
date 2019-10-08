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
			Sut = new SqlHelper(TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.SQL_USER_NAME, TestConstants.SQL_PASSWORD);
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
			bool result = Sut.DeleteAllErrors();
			int errorsCount = Sut.GetErrorsCount();

			// Assert
			Assert.True(errorsCount == 0);
		}

		[Test]
		public void GetFileShareResourceServerArtifactIdTest()
		{
			// Act
			int fileShareResourceServerArtifactId = Sut.GetFileShareResourceServerArtifactId();

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
			Assert.DoesNotThrow(() => Sut.EnableDataGridOnExtractedText(workspaceName));
		}

		[Test]
		public void CreateOrAlterShrinkDbProcTest()
		{
			// Arrange

			// Act

			// Assert
			Assert.That(Sut.CreateOrAlterShrinkDbProc(), Is.True);
		}

		[Test]
		public void RunShrinkDbProcTest()
		{
			// Arrange

			// Act

			// Assert
			Assert.That(Sut.RunShrinkDbProc(), Is.True);
		}

		[Test]
		public void InsertRsmfViewerOverrideTest()
		{
			// Arrange

			// Act

			// Assert
			Assert.DoesNotThrow(() => Sut.InsertRsmfViewerOverride());
		}
	}
}
