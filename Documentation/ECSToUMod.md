# ECSToUMod

# Prerequisites
## Unity 2020.1
## UMod 2.0
## Entities 0.7
This package was developed with the latest versions of Unity, UMod, and Entities, but will probably work with much earlier versions as well.

# Quick start
##Install UMod 
Do this if you haven't already.

## Import the ECSToUMod package
Open the Window -> Package Manager window, and in the top left corner click the plus sign icon.
From the dropdown, select "Add Packge From Git URL..."
Paste in https://github.com/vestigialdev/ECSToUMod.git
Hit enter and let the package import.

## Run the Setup function
A few files need to be copied from the package cache into the project. 
Open the Tools menu and select ECSToUMod -> Install
This will create a folder at Project/Assets/ECSToUMod 

## Include the ECSToUModFiles/ForExportWithModTools folder in the Mod Tools package
In the build tools wizard, in the Content section, be sure to select the ECSToUModFiles/ForExportWithModTools directory (and all files in it)

## Populate the ECSToUMod.ModHosts array
When in play mode, after loading your mods, populate the ECSToUMod.ModHosts static field

## Call ECSToUMod.CreateSystems()
This scans the loaded mods for subtypes of SystemBase to instantiate and update.

# Documentation
In the __Package Manager__ window, with __DOTS Editor__ selected, you have access to a __View documentation__ link that opens the package documentation for the current release.

This page contains the user manual, the Editor API reference, and the Runtime API reference.
