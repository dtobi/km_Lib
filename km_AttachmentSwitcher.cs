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

    public class km_AttachmentSwitcher : PartModule
    {

        public override void OnStart(StartState state)
        {
            print ("Km attachment tweaker is running!!!");
            setText ();
            base.OnStart (state);
        }

        private void setText(){
            this.Events["toggleRadAttach"].guiName = (this.part.attachRules.allowSrfAttach?"Disable Srf Attach":"Enable Sef Attach");
        }

        [KSPEvent(guiActive = false,  guiActiveEditor = true, guiName = "Toggle Rad Attachment")]
        public void toggleRadAttach()
        {
            print ("attmodessrf : "+this.part.attachRules.allowSrfAttach);
            this.part.attachRules.allowSrfAttach = !this.part.attachRules.allowSrfAttach;
            setText ();

        }
    }
}

