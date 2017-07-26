USE EDDS;

UPDATE 
  [EDDSDBO].[InstanceSetting]
SET
  [Value] = 'True'
WHERE
  [NAME] = 'DeveloperMode'
  AND [Section] = 'Relativity.Core'
  