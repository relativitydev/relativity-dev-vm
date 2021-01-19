# Changelog for Relativity-Dev-Vm

- This file is used to list changes made in the Relativity-Dev-Vm.

-------------------------

- 2021-01-19
	- REL-509154 - Removed unused scripts.  Changed DataGrid Instance Settings Create/Update to utilize our current C# PowerShell Modules

-------------------------

- 2021-01-13
	- REL-507462 - Replace ImagingHelper.cs RSAPI calls with REST implementation.

-------------------------

- 2021-01-12
	- REL-507447 - Replace WorkspaceHelper.cs RSAPI calls with REST implementation.
	- REL-507460 - Replace DisclaimerAcceptanceHelper.cs RSAPI calls with REST implementation.
	- REL-507459 - Replace ApplicationInstallHelper.cs RSAPI calls with REST implementation.

-------------------------

- 2020-11-18
	- REL-496550 - Replaced all uses of Object Manager API with REST implementation.

-------------------------

- 2020-11-10
	- REL-491399 - Added Relativity.Kepler.dll and Relativity.Services.DataContracts.dll as files to be added locally to the VM. Those dll's  are now in the Chef\Cookbooks\Relativity\files\default folder. Update those dll's to the correct version when building a new DevVm.