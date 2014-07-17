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
using KSP.IO;

namespace KM_Lib
{

    public class km_PartScaler : PartModule
    {

        [KSPField(isPersistant = true)]
        public string childPartName = ""; 

        [KSPField(isPersistant = true)]
        private bool isHiddenInEditor = false;

        public override void OnStart(StartState state)
        {
            base.OnStart (state);
            if (state == StartState.Editor) {
                if (isHiddenInEditor) {
                    setRenderer (childPartName, !isHiddenInEditor);

                }
            }
        }
        [KSPEvent(guiName = "Toggle editor visibility", guiActive = false, guiActiveEditor = true)]
        private void toggleVisibility ()
        {
            isHiddenInEditor = !isHiddenInEditor;
            setRenderer (childPartName, !isHiddenInEditor);
        }


        private void setRenderer(string childPartName, bool state){
            var childComponent = part.FindModelComponent<Component> (childPartName);
            if (childComponent != null) {
                childComponent.renderer.enabled = state;
                foreach (Collider col in childComponent.GetComponentsInChildren<Collider>()) {
                    col.enabled = false;
                    print ("Disabling collider:" + col.name);
                }
            }
        }
    }



}

