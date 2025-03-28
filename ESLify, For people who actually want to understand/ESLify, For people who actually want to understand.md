ESLify, For people who actually want to understand

# What does ESLify mean?

It is to take a mod with a plugin less then 2048 new records that loads into regular esp space using one of your 254 mods. Then, making it load in your ESL space, which you can thoreticly load 4095 extra plugins.
Are total theoretical limit is now 254 + 4095 plugins, totaling 4,350 theoretical mods.
4,351 if you count Skyrim.esm, we need to subtract Update.esm, Dawnguard.esm, Hearthfire.esm, and Dragonborn.esm now at 4346 total theoretical mods added to the game.

Back in the origonal skyrim, skyrim LE if you will, we did not have this amazing ESL system. So our total mod limit was 200 MAX new mods, so merging plugins was vary common. This had and still has the downside of it wouldn't always work well. It would make things increasingly unstable. You could not re-merge them mid-save, or add more mods to the merge.

But now in Skyrim SE, Bethesda added ESLs and the ESL flag to the engine. This allows us to compact our esp's and change the extention to .esl, but that has the issue of making plugins not recognize it as the same mod.
Now here is the solution ESL flag.
The ESL flag allows you to flag a mod with compacted forms instead of changing the extention. This will make it load in ESL space allowing you to reach that sweet sweet theoretical 4346 mods.

A new problem, Modders do not always think of this before hand, so they start on the path to a mod and find that they made 30 or 40 NPCs or new Voice lines, or thay now have a custom made sword and then they think wouldn't this look sweat with there own custom made animations, and thats great new characters or better looking animations paired with a genuinly unique sword.
Now recently I have noticed more of them think of it first and start with their plugins flagged as esl or esl file. But the ones who didn't either can't eslify it, because its to much work or users have already been using it in there save. Most don't read the patch notes that say it would need a new save, which is also perfectly alright. But for users who know how to make a stable 2000+ plugin load order, 
that 250 new plugin limit is stood in the way of using that really good looking sword or to the degenerates, [spoiler]that dummy thick mommy milker follower sex mod[/spoiler], can not be used or it will make me have to give up one of my large gameplay mod.

but now with this fixing all those hard coded formIDs in data files its simple and worry free to fix. but it seems people don't really understand what to eslify and what to not or can't be eslified.

# Criteria

This is the criteria regular mod users should be using before the creation of ESLify Everything should be using when eslifing a mod:

## What can not be ESLified?
- Mods that add new Cells or Interiors
- Mods that add new World Spaces
- Mods with more then 2048 new records
- Mods that use higher Framework mods, like SKSE, SPID(and its duplicate functions), DAR, .Net Script Framework, 

## What should not be ESLified?
- Master plugins, .esm plugins
- Mods that are required by many other mods or need to be patched with many other mods
- Mods with more then 2000 new records
- Mods that are being developed, with scripts that contain hard coded FormIDs inside of scripts
- New NPC mods, with NPCs formIDs outside the range of 00000800 and 00000FFF, or voice files
- Weapon mods, if they were hooked into Animated Armory, because they usually get hooked into DAR as a new weapon type as well

## What should be ESLified?
- Small mods that are not required by other mods
- Mods that are no longer being developed
- Armor 
- Weapon mods, if they weren't hooked into Animated Armory, because they usually get hooked into DAR as a new weapon type as well
- Creature mods, that don't use FaceGen

This is the criteria for regular mod users after the creation of ESLify Everything should be using when eslifing a mod. and what I will be talking about furthur down

## What can not be ESLified?
- Mods that add new Cells or Interiors, that player can enter
- Mods that add new World Spaces

## What should not be ESLified?
- Master plugins, .esm plugins
- Mods that are required by manyother mods that dont use something like SPID to distribute amor or need to be patched with many other mods
- Mods that are being developed, with frequint updates

## What should be ESLified? as long as the mod has less then 2048 new records.
- Small mods that are not required by other mods
- Mods that are no longer in developed or supported
- Armor 
- All Weapon mods
- Creature mods
- New NPC mods
- Mods that use higher Framework mods, like SKSE, SPID(and its duplicate functions), DAR, .Net Script Framework

## How to tell if a mod CAN NOT be eslifed.

New Cells and World space
Reason) New Cells and World space are the only two that still can not be eslified. Due to an engine bug making mods that edit that new cell loaded from a plugin in ESL space not get loaded.

How to tell) Open xEdit, and apply the script "Find ESP plugins which could be turned into ESL", You usually will only see three defferent messeges on them.
(Insert 3messeges.png)
the messeges that will tell you this might not be a plugin you can eslify is
   
"Warning: Plugin has new CELL(s) which won't work when turned into ESL and overridden by other mods due to the game bug"
(Insert SOMexample.png)

SOM.esp is from the mod Simple Outfit Manager, it contains a cell that it uses to store containers. This flags as a mod that might not work when compacted.
But the Cell is unreachable except by using the a console command and no other mod is going to add anything to it, or at least should add anything to it. So its relitivly safe to eslify.

The same warning will pop up from if it detects a world space because it also adds cells to said worldspace. Its also nessessary to check if the plugin has a worldspace header
(insert worldspace.png)
Usually if its a mod that adds a new worldspace it likely will have more then that 2048 form budget.

SKSE mods with hardcoded formIDs inside the DLL
Reason) There is no reliable way to decompile a C++ DLL. So from a scripting standpoint, I have no way to decompile and recompile the .dll file. And a user standpoint there is no point because it isn't a quick thing to do as a regular user. You must ask the mod author to do it and upload it.

How to tell) The only way to tell is finding the source code. Many mod authors usually have a link to there source code on GitHub or have it posted on nexus, If they don't don't compact the plugin.

If they do, go one by one through the .cpp files using ctrl+F searching for the plugin name, I am using source code from [Loki's Poise mod](https://github.com/LokiWasTaken/POISE-Stagger-Overhaul-SKSE/blob/main/src/POISE/PoiseMod.cpp)﻿ for this example

(insert hardcodedlokiform.png)

The underline in Green﻿ is ok because it wont be changed when compacted, but what is underlined in red is not because it will be compacted because it is outside the Hex range of 800 and FFF.

If it has any hardcoded forms for other mods, it is not safe to compact those mods. However most modders will use a .ini file to cover other mods or some other way to add support for other mods if needed. To check for other mods you can use ctrl+F searching for .esp or .esm files, .esl files don't need to be compacted anyway so you don't need to worry about them. and Skyrim.esm, Update.esm, Dawnguard.esm, Hearthfire.esm and Dragonborn.esm can be ignored because they are Skyrim's base plugins.

## How to tell if a mod SHOULD NOT be ESLified.
#### this is more a personal choice sellection

Master plugins
Reason) It likly is either too large or required by alot of other mods,

How to tell) If it is a .esm file extention or if it has a ESM flag
(insert masterflag.png)

Does not mean they can't many modders that make patches for base skyrim make them a .esm plugin or flag them as a esm, in order to enforce the mod not to overwrite anything else. That is why the "Unofficial Skyrim Modders Patch.esp" should always be at the top right under Skyrim and its DLCs
(insert usmp.png)

Mods that are required by manyother mods that dont use something like SPID to distribute amor or need to be patched with many other mods
Reason) you will need to reinstall the file and re-compact and eslify the mod to update the forms inside the plugin

How to tell) Really more of a what mods you are downloading and how often do I add a mod requiring that mod, I can't really anwer this one

Mods that are being developed, with frequint updates
Reason) Same as previous

How to tell) Check its files and archive and see how frequintly they update the mod, of if it is new chances are its going to be updated for any bugs that were not found before release

## How to tell if a mod should be ESLified now that ESLify Everything is released. All have the same steps
- Small mods that are not required by other mods
- Mods that are no longer being developed
- Armor 
- All Weapon mods
- Creature mods
- New NPC mods
- Mods that use higher Framework mods, like SKSE, SPID(and its duplicate functions), DAR, .Net Script Framework

How to tell) Check using the "New Cells and World space" method, run "Find ESP plugins which could be turned into ESL", and if it comes up with:

(insert skyhavenlog.png)
"Can be turned into ESL by adding ESL flag in TES4 header" then all it needs is you to set the ESL flag in the header file.
(insert seteslskyhaven.png)

or

(insert WardsFunctionalitiesExtendedlog.png)
"Can be turned into ESL by compacting FormIDs first, then adding ESL flag in TES4 header" then you only need to compact forms
(insert WardsFunctionalitiesExtendedcompactform.png)
and then set the esl flag
(insert WardsFunctionalitiesExtendedsetesl.png)

then run ESLify Everything to fix any hard coded formIDs

# How to tell if ESLify Everything will output any files

1. First by checking if you downloaded and installed any mods that require the mod, like a mod that requires something like SPID to distribute the items

Our base mod is the amazing mod "Guards Armor Replacer SSE"

But then a mod comes along to add the armor to Housecarls like "Housecarls - Guards Armor Replacer (SPID)" GAR version, but without it actually adding duplicate outfits to add
it would use the formIDs of the armor to add it to the Housecarls, This would output a corrected formID to a new file to the output folder.

2. FaceGen and Voice files

For this we are using the mod "Caesia Follower - Borne of Magic - Revamped" the plugin file is named "CaesiaFollower.esp"
So open the mod arcive or folder in MO2

and check for a folder in side of the arcive at "meshes\actors\facegendata\facegeom", inside we will find a folder called "CaesiaFollower.esp" and inside of that it will have "00000D62.nif", this is Caesia's formID.
Behold it is a formID between 800 and FFF so this file does not need to be changed when compacting forms for ESL.

But for Voices you would check "sound\voice" and you will find another folder named "CaesiaFollower.esp" inside of that you will find alot of voice files but I am only going to name 2 for this example:
"CaesiaFoll_CaesiaAlertIdle_0000762A_1.fuz" is a voice line with the form "0000762A" this will be changed and outputed with a form between 800 and FFF to the output folder
but there are also voice files with the formIDs between 800 and FFF, "CaesiaFoll_CaesiaCombatTau_00000D9B_1.fuz". This would not be copacted so it would not have a new file outputed by ESLify Everything.




































