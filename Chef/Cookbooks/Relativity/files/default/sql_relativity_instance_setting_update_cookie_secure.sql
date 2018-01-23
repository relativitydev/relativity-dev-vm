USE EDDS;

UPDATE 
  [EDDSDBO].[InstanceSetting]
SET
  [Value] = 'False'
WHERE
  [NAME] = 'CookieSecure'
  AND [Section] = 'Relativity.Authentication'

UPDATE 
  [EDDSDBO].[InstanceSetting]
SET
  [Value] = 'False'
WHERE
  [NAME] = 'EnforceHttps'
  AND [Section] = 'Relativity.Authentication'
  