// /*
//  * Author: dtobi
//  * This work is shared under CC BY-NC-ND 3.0 license.
//  * Non commercial, no derivatives, attribution if shared unmodified.
//  * You may distribute this code and the compiled .dll as is.
//  * 
//  * Exception from the no-deriviates clause in case of KSP updates:
//  * In case of an update of KSP that breaks the code, you may change
//  * this code to make it work again and redistribute it under a different
//  * class name until the author publishes an updated version. After a
//  * release by the author, the right to redistribute the changed code
//  * vanishes.
//  * 
//  * You must keep this boilerplate in the file and give credit to the author
//  * in the download file as well as on the webiste that links to the file.
//  * 
//  * Should you wish to change things in the code, contact me via the KSP forum.
//  * Patches are welcome.
//  *
//  */
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace km_Lib
{
    public class KM_PreLaunchEffect : PartModule
    {

        [KSPField (isPersistant = false)]
        public string effectName = "PreLaunchEffect";


        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Launch Effects"),
            UI_Toggle(disabledText = "Disabled", enabledText = "Enabled")]
        public bool isActive = true;

        public override void OnStart(StartState state)
        {
            if (state != StartState.Editor && vessel.situation == Vessel.Situations.PRELAUNCH) {
                setEffect (true);
            }
        }

        public override void OnUpdate()
        {
            if (isActive && vessel.situation != Vessel.Situations.PRELAUNCH) {
                setEffect (false);
            }
        }


        //[KSPEvent(guiName = "Reset", guiActive = true)]
        public void setEffect (bool state) {   
            isActive = state;
            var launchEffects = this.part.GetComponentsInChildren <KSPParticleEmitter> ();
            foreach (KSPParticleEmitter launchEffect in launchEffects) {
                if (launchEffect != null){
                    print ("Found Effect: " + launchEffect.name);
                    if (launchEffect.name == effectName) {
                        launchEffect.emit = state;
                    }
                }
            }
        }
    }
}

