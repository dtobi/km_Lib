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
using km_Lib;

namespace KM_Lib
{

    

    public class km_ModuleAttAniObserver : PartModule
    {
        [KSPField (isPersistant = false)]
        public string initAnimation = "Open";
    
        [KSPField (isPersistant = false)]
        public string runAnimation = "Run";

        [KSPField (isPersistant = false)]
        public string nodeName = "top";

        private km_Animator ani = null;
        private AttachNode checkNode = null;


        private bool state = false;

        [KSPField (isPersistant = true)]
        bool triggered = false;


        public override void OnStart(StartState state)
        {
            ani = this.part.GetComponentInChildren <km_Animator>();
            checkNode = this.part.findAttachNode(nodeName);
            if (ani != null && checkNode != null) {


                print ("Observer: Attaching to "+ani.moduleName+" Parent:"+ani.part.name +"ID:"+part.GetInstanceID().ToString());
                print ("Observer: Attaching to node"+checkNode.id);

                if (state != StartState.Editor && !triggered)
                    ani.locked = true;
            }

            base.OnStart (state);
        }

        public override void OnUpdate ()
        {
            if (checkNode==null || ani==null) {
                print ("KM ModuleAnimatorObserver: Node or Ani are null");
                return;
            }

            if (!triggered && checkNode.attachedPart == null) {
                print ("Triggering ani:" + ani.part.name + " " + this.part.name + " " + ani.part.GetInstanceID () + " " + this.part.GetInstanceID ());
                ani.locked = false;
                ani.toggle ();
                triggered = true;
            
            }
        }
    }
}

