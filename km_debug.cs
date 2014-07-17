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

namespace KM_Lib
{

    public class km_Debug : PartModule
    {

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "ID")]
        public string myPartID;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Name")]
        public string myPartName;

        [KSPEvent(guiName = "Print", guiActive = true, guiActiveEditor = true)]
        public void print(){
            foreach (PartModule module in part.Modules) {
                print (this.name + ": " + module.ClassName + " " + module.GetInstanceID () + " " + module.part.GetInstanceID ());

            }
        }

        [KSPEvent(guiName = "Nodes", guiActive = true, guiActiveEditor = true)]
        public void nodes(){
            foreach (AttachNode node in part.attachNodes) {
                print (this.name + ": " + node.id + " " + node.attachedPartId + " " + node.attachedPart.name);

            }
        }



        [KSPEvent(guiName = "isactive", guiActive = true, guiActiveEditor = true)]
        public void printActive(){
            string pState = "Unknown";
            switch (this.part.State) {
            case PartStates.ACTIVE:
                pState = "ACTIVE";
                break;
            case PartStates.DEACTIVATED:
                pState = "DEACTIVATED";
                break;
            case PartStates.DEAD:
                pState = "DEAD";
                break;
            case PartStates.IDLE:
                pState = "IDLE";
                break;
            default:
                pState = "JuckaJucka:" + this.part.State;
                break;
            }


            print ("Isactive: " + pState);


        }
        public static bool hasTech(string techid) {
            try{
                string persistentfile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/persistent.sfs";
                ConfigNode config = ConfigNode.Load (persistentfile);
                ConfigNode gameconf = config.GetNode ("GAME");
                ConfigNode[] scenarios = gameconf.GetNodes ("SCENARIO");
                foreach (ConfigNode scenario in scenarios) {
                    if (scenario.GetValue ("name") == "ResearchAndDevelopment") {
                        ConfigNode[] techs = scenario.GetNodes ("Tech");
                        foreach (ConfigNode technode in techs) {
                            if (technode.GetValue ("id") == techid) {
                                return true;
                            }
                        }
                    }
                }
                return false;
            } catch (Exception ex) {
                return false;
            }
        }

        [KSPEvent(guiName = "tech", guiActive = true, guiActiveEditor = true)]
        public void printTech(){

            hasTech ("specializedElectrics");


        }
        /// <summary>
        /// Determine the signed angle between two vectors, with normal 'n'
        /// as the rotation axis.
        /// Code by Tinus: http://forum.unity3d.com/threads/51092-need-Vector3-Angle-to-return-a-negtive-or-relative-value
        /// </summary>
        public static double AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2));
        }

        [KSPEvent(guiName = "Gimbalinfo", guiActive = true, guiActiveEditor = true)]
        public void printGimbal(){



            print ("X:"+vessel.ctrlState.X+" Y:"+vessel.ctrlState.Y+" Z:"+vessel.ctrlState.Z);
            print ("isNeutral" + vessel.ctrlState.isNeutral);
            print ("FwdV" + vessel.GetFwdVector());
            print ("Vup"+vessel.upAxis);
          
            /*

            */
            print ("VTU1:" + vessel.vesselTransform.up +"VTU:" + vessel.transform.up + " VRTU:" + vessel.ReferenceTransform.up);
            print ("VTR1:" + vessel.vesselTransform.right +"VTR:" + vessel.transform.right + " VRTR:" + vessel.ReferenceTransform.right);
            print ("VTF1:" + vessel.vesselTransform.forward +"VTF:" + vessel.transform.forward + " VRTF:" + vessel.ReferenceTransform.forward);

        }

        public override void OnActive ()
        {

            base.OnActive ();
            print ("Part " + part.name + " " + part.GetInstanceID () + " became active.");
        }



        public override void OnStart(StartState state)
        {
            myPartID = this.part.GetInstanceID().ToString();
            myPartName = this.part.name;


            base.OnStart (state);
        }



     
    }
}

