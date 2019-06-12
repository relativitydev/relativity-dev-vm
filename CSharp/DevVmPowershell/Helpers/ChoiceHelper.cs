using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
	public class ChoiceHelper
	{
		public bool Query_Choices(IRSAPIClient rsapiClient, string fieldName = "Data Grid File Repository")
		{
			//Query for the Field ChoiceTypeId
			const string queryFieldChoiceTypeIdErrorMessage = "An error occured when querying for Field ChoiceTypeId";
			Query<kCura.Relativity.Client.DTOs.Field> fieldQuery = new Query<kCura.Relativity.Client.DTOs.Field>
			{
				Fields = new List<FieldValue>
				{
					new FieldValue(FieldFieldNames.Name),
					new FieldValue(FieldFieldNames.ChoiceTypeID)
				},
				Condition = new TextCondition("Name", TextConditionEnum.EqualTo, fieldName)
			};

			int choiceTypeId;
			try
			{
				QueryResultSet<kCura.Relativity.Client.DTOs.Field> fieldQueryResultSet = rsapiClient.Repositories.Field.Query(fieldQuery);

				if (fieldQueryResultSet.Success && fieldQueryResultSet.Results.Any())
				{
					kCura.Relativity.Client.DTOs.Field fieldArtifact = fieldQueryResultSet.Results.First().Artifact;
					if (fieldArtifact.ChoiceTypeID == null)
					{
						throw new Exception();
					}
					choiceTypeId = fieldArtifact.ChoiceTypeID.Value;
				}
				else
				{
					throw new Exception($"{queryFieldChoiceTypeIdErrorMessage}. ErrorMessage: {fieldQueryResultSet.Message}");
				}
			}
			catch (Exception ex)
			{
				throw new Exception(queryFieldChoiceTypeIdErrorMessage, ex);
			}

			//Query for the Choices
			Query<kCura.Relativity.Client.DTOs.Choice> choiceQuery = new Query<kCura.Relativity.Client.DTOs.Choice>
			{
				Condition = new WholeNumberCondition(ChoiceFieldNames.ChoiceTypeID, NumericConditionEnum.EqualTo, choiceTypeId),
				Fields = FieldValue.AllFields
			};

			// STEP 3: Create QueryResultSet to collect Query Results.
			QueryResultSet<kCura.Relativity.Client.DTOs.Choice> choiceQueryResultSet;

			// STEP 4: Perform the Query.
			try
			{
				choiceQueryResultSet = rsapiClient.Repositories.Choice.Query(choiceQuery, 0);
			}
			catch (Exception ex)
			{
				Console.WriteLine("An error occurred: {0}", ex.Message);
				return false;
			}

			// Check for success
			if (!choiceQueryResultSet.Success)
			{
				Console.WriteLine("The Query operation was not successful.{0}{1}", Environment.NewLine, choiceQueryResultSet.Message);
				return false;
			}

			Console.WriteLine("Number of Choices returned: {0}", choiceQueryResultSet.Results.Count);

			return true;
		}
	}
}
