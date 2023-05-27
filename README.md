# Mismagius

Offline [PvPoke](https://github.com/pvpoke/pvpoke) custom format development utility.

Instruction guide:

== INSTALLING OFFLINE PVPOKE
* 1.) Install XAMPP - https://www.apachefriends.org/
* 2.) Download the [PvPoke Github repo](https://github.com/pvpoke/pvpoke) (Code -> Download ZIP) 
* 3.) Extract the zipped file into `(XAMPP installation folder)/htdocs`, by default on Windows it'll be`C:/xampp/htdocs`, and rename the `pvpoke-master` folder to just `pvpoke`. If you're using Linux then you don't need me to give you instructions for this
* 4.) Run XAMPP Control Panel, it'll show up as an installed app on Windows, or alternatively run xampp-control.exe in the XAMPP installation folder
* 5.) Start the Apache Module
* 6.) Navigate to `localhost/pvpoke/src` in your browser
* 7.) You should see a familiar PvPoke page 

== INSTALLING MISMAGIUS
* 1.) Download `Mismagius.exe` (Windows) / `Mismagius` (MacOS) from the latest release from the [Releases page](https://github.com/RedSpah/Mismagius/releases).
* 2.) Move the downloaded executable to `(XAMPP installation folder)/htdocs/pvpoke`

== ADDING A CUSTOM FORMAT WITH MISMAGIUS
* 1.) Copy the exported format settings from the Custom Ranking page
* 2.) Make a new file in the same folder as Mismagius, name it `XXX.json` where XXX is the lowercase name of the cup that you have in mind
* 3.) Paste the exported format settings into the newly created file
* 4.) Run Mismagius
* 5.) In Mismagius, run `add XXX`
* 6.) If on Windows, run `restart`. If on Mac/Linux, you'll have to manually restart the Apache server with XAMPP control panel
* 7.) Refresh the local PvPoke page and your format should be selectable and rank-able, by default it'll have the open format rankings as placeholder
* 8.) If you want to update a format, update the contents of `XXX.json`, then run `add XXX` in Mismagius again to update it without discarding the previous rankings / overrides
