# B-S-Finger-Tracking-Spells
B&S Spell Selection with Finger Tracking

With this mod I intend to make use of Blade and Sorcery's finger tracking.
(That being said, this works best with the Index)

Poses are stored in the Settings.JSON, inside the handPoses array.

In order to bind a specific spell to a pose, you will need to get the desired spell's spellID.
Take the spellID and enter it into the JSON under the spellAtPose array AT THE CORRESPONDING INDEX.
(MEANING, if you wanted to assign spell x into pose y: pose y might be at handPose[1], so you must put spell x into spellAtPose[1] as well.)

This will work with any custom spells, as long as you have enough hand poses.
