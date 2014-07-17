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

    public class km_Animator : PartModule
    {

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Animation")]
        public string animationName = ""; 

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Animation")]
        public string endEventGUIName = "Close"; 

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Animation")]
        public string startEventGUIName = "Open"; 

        [KSPField(isPersistant = true)]
        private bool isDeployed = false;

        [KSPField(isPersistant = true)]
        public string triggerNode = "";
        public bool locked = false;
      

        private void setText(){
            this.Events["toggle"].guiName = (isDeployed?endEventGUIName:startEventGUIName);
        }


        [KSPAction("Toggle")]
        public void toggletAG (KSPActionParam param){
            toggle ();
        }
        [KSPEvent(guiName = "Toggle", guiActive = true, guiActiveEditor = true)]
        public void toggle(){
            if (!locked) {
                isDeployed = !isDeployed;
                animate (true);
                setText ();
            } else {
                print ("Part locked. Animation blocked by attached part.");
            }
        }

        public void animate(bool play)
        {

            Utility.playAnimation (this.part, animationName, isDeployed, play, 1.0f); 

            
        }

        public override void OnStart(StartState state)
        {

            base.OnStart (state);

            if (state != StartState.None) {
                animate (false);
                setText ();

            }
            if (state != StartState.Editor) {
                if (locked) {
                    this.Events ["toggle"].guiActive = false;
                }
            }

           
           

        }

    }



}

