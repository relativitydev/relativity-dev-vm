# Changelog for Relativity-Dev-Vm

- This file is used to list changes made in the relativity-dev-mm repo.
- **Place Newer updates on the top**

-------------------------

## 2021-02-18

- [REL-523461](https://jira.kcura.com/browse/REL-523461) - HTTP API requests in the Lanceleaf branch.
    - This code works only in Lanceleaf release.  Updated Application Install Helper to work specifically for Lanceleaf.

-------------------------

## 2021-02-15
- [REL-522356](https://jira.kcura.com/browse/REL-522356) - Updated the ImagingHelper.cs class to use REST APIs instead of C# APIs.
- [REL-522362](https://jira.kcura.com/browse/REL-522362) - Updated the ProcessingHelper.cs class to use REST APIs instead of C# APIs.

-------------------------

## 2021-02-12
- [REL-522364](https://jira.kcura.com/browse/REL-522364) - Updated the RelativityVersionHelper.cs class to use REST APIs instead of C# APIs.
[REL-522353](https://jira.kcura.com/browse/REL-522353) - Updated the AgentServerHelper.cs class to use REST APIs instead of C# APIs.

-------------------------

## 2021-02-11

- [REL-522350](https://jira.kcura.com/browse/REL-522350) 
	- Updated the AgentHelper.cs class to use REST APIs instead of C# APIs.
	- Updated Constructors to take ConnectionHelper instead of InstanceName, AdminUsername, AdminPassword

-------------------------

## 2021-02-02

- [REL-508653](https://jira.kcura.com/browse/REL-508653) - Updated Processing Source Location code to use Kepler services REST endpoints.
- This code works only in Ninebark 0 release and above.

-------------------------

## 2021-02-01

- [REL-507466](https://jira.kcura.com/browse/REL-507466) - Made code changes to make sure all the Integration tests in  the DevVmPsModules solution are passing.

-------------------------

## 2021-01-19

- [REL-509154](https://jira.kcura.com/browse/REL-509154) - Removed unused scripts.  Changed DataGrid Instance Settings Create/Update to utilize our current C# PowerShell Modules

-------------------------

## 2021-01-15

- [REL-507461](https://jira.kcura.com/browse/REL-507461) - Replace SmokeTestHelper.cs RSAPI calls with REST implementation.

-------------------------

## 2021-01-13

- [REL-507462](https://jira.kcura.com/browse/REL-507462) - Replace ImagingHelper.cs RSAPI calls with REST implementation.

-------------------------

## 2021-01-12

- [REL-507447](https://jira.kcura.com/browse/REL-507447) - Replace WorkspaceHelper.cs RSAPI calls with REST implementation.
- [REL-507460](https://jira.kcura.com/browse/REL-507460) - Replace DisclaimerAcceptanceHelper.cs RSAPI calls with REST implementation.
- [REL-507459](https://jira.kcura.com/browse/REL-507459) - Replace ApplicationInstallHelper.cs RSAPI calls with REST implementation.

-------------------------

## 2020-11-18

- [REL-496550](https://jira.kcura.com/browse/REL-496550) - Replaced all uses of Object Manager API with REST implementation.

-------------------------

## 2020-11-10

- [REL-491399](https://jira.kcura.com/browse/REL-491399) - Added Relativity.Kepler.dll and Relativity.Services.DataContracts.dll as files to be added locally to the VM. Those dll's  are now in the Chef\Cookbooks\Relativity\files\default folder. Update those dll's to the correct version when building a new DevVm.
