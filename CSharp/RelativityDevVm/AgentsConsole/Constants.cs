namespace AgentsConsole
{
	public class Constants
	{
		//Application GUID's
		//Imaging = C9E4322E-6BD8-4A37-AE9E-C3C9BE31776B
		//DocumentViewer = 5725CAB5-EE63-4155-B227-C74CC9E26A76
		//Production = 51B19AB2-3D45-406C-A85E-F98C01B033EC
		//Processing = ED0E23F9-DA60-4298-AF9A-AE6A9B6A9319
		//SmokeTest = 0125C8D4-8354-4D8F-B031-01E73C866C7C

		public class ErrorMessages
		{
			public static readonly string SetupPropertiesError = "An error occured when parsing args array and setting properties";
			public static readonly string RestStringDeserializationError = "An error occured when deserializing REST result string";

			public class Rest
			{
				public static readonly string RestPostCallError = "An error occured when performing the REST POST call";
				public static readonly string RestPostCallNullResultError = "REST Result is NULL.";
			}
		}
	}
}
