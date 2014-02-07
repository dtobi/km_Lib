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
    public class KM_LaunchEffect : PartModule
    {

        [KSPField (isPersistant = false)]
        public string effectName = "PreLaunchEffect";

        [KSPField (isPersistant = false)]
        public string effectLightName = "PreLaunchEffectLight";

        [KSPField (isPersistant = false)]
        public string transformName = "";

        [KSPField (isPersistant = false)]
        public bool checkBottomNode = true;

        [KSPField (isPersistant = false)]
        public bool debug = false;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Show"),
            UI_Toggle(disabledText = "Off", enabledText = "Editor")]
        public bool editorPlacementOptionsActive = false;

        [KSPField (isPersistant = true)]
        public bool isRunning = false;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Timer"),
            UI_FloatRange(minValue =0f, maxValue = 20f, stepIncrement = 1f)]
        public float runningTime = 0.0f;

        private double startTime = 0f;
        private bool timerRunning = false;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "With"),
            UI_FloatRange(minValue =0f, maxValue = 2f, stepIncrement = 0.1f)]
        public float width = 0f;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Height"),
            UI_FloatRange(minValue =0f, maxValue = 20f, stepIncrement = 1f)]
        public float height = 0;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "X"),
            UI_FloatRange(minValue =-4f, maxValue = 4f, stepIncrement = 0.1f)]
        public float xOffset = 0;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Y"),
            UI_FloatRange(minValue =-4f, maxValue = 4f, stepIncrement = 0.1f)]
        public float yOffset = 0;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Z"),
            UI_FloatRange(minValue =-4f, maxValue = 4f, stepIncrement = 0.1f)]
        public float zOffset = 0;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Num P"),
            UI_FloatRange(minValue =10, maxValue = 200f, stepIncrement = 20f)]
        public float numP = 0;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "speedX"),
            UI_FloatRange(minValue =-5f, maxValue = 5f, stepIncrement = 0.25f)]
        public float speedX = 0;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "speedY"),
            UI_FloatRange(minValue =-5f, maxValue = 5f, stepIncrement = 0.25f)]
        public float speedY =0;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "speedZ"),
            UI_FloatRange(minValue =-5f, maxValue = 5f, stepIncrement = 0.25f)]
        public float speedZ = 0;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Size"),
            UI_FloatRange(minValue =-0f, maxValue = 1.5f, stepIncrement = 0.05f)]
        public float size =0f;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Random"),
            UI_FloatRange(minValue =-0f, maxValue = 2f, stepIncrement = 0.1f)]
        public float rndVelocity = 0f;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "SoftOff"),
            UI_Toggle(disabledText = "Disabled", enabledText = "Enabled")]
        public bool softDecrease = false;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Launch Effects"),
            UI_Toggle(disabledText = "Disabled", enabledText = "Enabled")]
        public bool isActive = true;

        protected static System.Random rnd = new System.Random();
       

        private int performanceLimiter =  rnd.Next(); 
        private int performaceThreshold = 10;

        private Dictionary<int, KSPParticleEmitter> effectsList = new Dictionary<int, KSPParticleEmitter> ();
        private Dictionary<int, Vector3> locationList = new Dictionary<int, Vector3> ();
        private Dictionary<int, Light> lightList = new Dictionary<int, Light> ();


        [KSPEvent(guiName = "Toggle", guiActive = true)]
        public void toggle (){
            setEffect (!isActive);
        }
      

        Vessel.Situations lastSituation;


        [KSPAction("Toggle")]
        public void toggleAG (KSPActionParam param){
            setEffect (!isActive);
        }

        public override void OnStart(StartState state)
        {
            if (checkBottomNode) {
                foreach (var node in this.part.attachNodes) {
                    print ("Att:" + node.id);
                    if (node.id == "bottom" && node.attachedPart != null) {
                        print ("Attached!");
                        isActive = false;
                    }
                }
            }

            registerEffects ();

            if (state != StartState.Editor && Vessel.Situations.PRELAUNCH == vessel.situation) {
                OnPreLaunch ();
            }

            if (!debug) {

                this.Fields["runningTime"].guiActiveEditor  = false;
                this.Fields["width"].guiActiveEditor        = false;
                this.Fields["height"].guiActiveEditor       = false; 
                this.Fields["xOffset"].guiActiveEditor      = false; 
                this.Fields["yOffset"].guiActiveEditor      = false;
                this.Fields["zOffset"].guiActiveEditor      = false;
                this.Fields["numP"].guiActiveEditor         = false;
                this.Fields["speedX"].guiActiveEditor       = false;
                this.Fields["speedY"].guiActiveEditor       = false;
                this.Fields["speedZ"].guiActiveEditor       = false;
                this.Fields["size"].guiActiveEditor         = false;
                this.Fields["rndVelocity"].guiActiveEditor  = false;
                this.Fields["softDecrease"].guiActiveEditor  = false;

            }
            print ("DBG: OnEditorAttach");
            if (state == StartState.Editor)
            {
                print ("Placement is " + editorPlacementOptionsActive);
                this.part.OnEditorAttach += OnEditorAttach;
                this.part.OnEditorDetach += OnEditorDetach;
                this.part.OnEditorDestroy += OnEditorDestroy;
                OnEditorAttach();
            }

        }

        void registerEffects(){
            var launchEffects = this.part.GetComponentsInChildren <KSPParticleEmitter> ();
            foreach (KSPParticleEmitter launchEffect in launchEffects) {
                if (launchEffect != null) {
                    print ("DBG Found Effect: " + launchEffect.name);
                    if (launchEffect.name == effectName) {
                        effectsList.Add (launchEffect.GetInstanceID(), launchEffect);
                        if (transformName == "" || transformName == effectName) {
                            print ("DBG: LP");
                            locationList.Add (launchEffect.GetInstanceID(), launchEffect.transform.localPosition);
                        } else {
                            print ("DBG: MB");
                            var partTransforms = this.part.GetComponentsInChildren <MonoBehaviour> ();
                            foreach (MonoBehaviour partTransform in partTransforms) {
                                if (transformName == partTransform.name) {
                                    locationList.Add (launchEffect.GetInstanceID(), partTransform.transform.localPosition);
                                }
                            }
                        }
                    }
                }
            }
            print ("DBG: BeforeLights");
            var launchEffectsLights = this.part.GetComponentsInChildren <Light> ();
            foreach (Light launchEffectLight in launchEffectsLights) {
                if (launchEffectLight != null) {
                    print ("Found Light: " + launchEffectLight.name);
                    if (launchEffectLight.name == effectLightName) {
                        print ("Added light");
                        lightList.Add (launchEffectLight.GetInstanceID(), launchEffectLight);
                    }
                }
            }
            print ("DBG: AfterLights");
        }


        public virtual void OnPreLaunch(){}

        public virtual void OnLaunch(){}
    
        public virtual void OnTimer(){}

        protected void startTimer(){

            timerRunning = true;
            startTime = Time.fixedTime;     
        }

        protected float remainingTime(){
            return (float) (!timerRunning ? 0 : startTime + runningTime - Time.fixedTime);
        }

        protected void checkTimer(){
            if(remainingTime() < 0){
                startTime = 0;
                timerRunning = false;
                OnTimer ();
            }
        }

        public override void OnUpdate()
        {
            if (HighLogic.LoadedSceneIsEditor)
                return;

            // we launched
            if (lastSituation == Vessel.Situations.PRELAUNCH && vessel.situation != Vessel.Situations.PRELAUNCH) {
                OnLaunch ();
            }
            checkTimer ();

            lastSituation = vessel.situation;

        }

        public override string GetInfo ()
        {
            return "KM Launch Effect by dtobi";
        }                    

        private void updateEditor(){
            if(performanceLimiter++ % performaceThreshold == 0)
                setEffect(editorPlacementOptionsActive);
        }

        private void OnEditorAttach()
        {
            RenderingManager.AddToPostDrawQueue(99, updateEditor);
        }

        private void OnEditorDetach()
        {

            RenderingManager.RemoveFromPostDrawQueue(99, updateEditor);
            print("OnEditorDetach");
        }

        private void OnEditorDestroy()
        {
            RenderingManager.RemoveFromPostDrawQueue(99, updateEditor);
            print("OnEditorDestroy");

        }

        public void setEffect (bool state)
        {   
            foreach (KeyValuePair<int, KSPParticleEmitter> pair in effectsList) {
                KSPParticleEmitter launchEffect = pair.Value;
                int key = pair.Key;
                //print ("Found Effect: " + launchEffect.name+" state"+state);
                launchEffect.emit = state;

                if (state) {

                    launchEffect.shape2D = new Vector2 (width, width);

                    if (height != 0)
                        launchEffect.maxEnergy = height;
                    if (height != 0)
                        launchEffect.minEnergy = height / 2;                       
                    launchEffect.transform.localPosition = locationList [key];
                    if (xOffset != 0 || yOffset != 0 || zOffset != 0)
                        launchEffect.transform.Translate (new Vector3 (xOffset, yOffset, zOffset), Space.Self);

                    if (numP != 0)
                        launchEffect.maxEmission = (int)(numP);
                    if (numP != 0)
                        launchEffect.minEmission = (int)(numP / 2);
                    if (size != 0)
                        launchEffect.minSize = size;
                    if (size != 0)
                        launchEffect.minSize = size / 2;
                    if (speedX != 0 || speedY != 0 || speedZ != 0)
                        launchEffect.localVelocity = new Vector3 (speedX, speedY, speedZ);

                    if (rndVelocity != 0)
                        launchEffect.rndVelocity = new Vector3 (rndVelocity, rndVelocity, rndVelocity);

                }
            }
            foreach (Light launchEffectLight in lightList.Values) {
                launchEffectLight.intensity = (state ? 1 : 0);
            }

        }
    }      
        


    public class KM_PreLaunchEffect : KM_LaunchEffect
    {


        public override void OnLaunch ()
        {
            if (runningTime > 0) {
                startTimer ();
            } else {
                setEffect (false);
            }
        }

        public override void OnPreLaunch ()
        {
            if (isActive) {
                setEffect (true);
            } else {
                print ("Effect " + effectName + " not active. Not starting.");
            }

        }

        public override void OnTimer ()
        {
            setEffect (false);
            //print("Shutting off effects:" + effectName);
        }
    }

    public class KM_PostLaunchEffect : KM_LaunchEffect
    {
        private int performanceLimiterUpdate = 0;
        private int performanceThreshold = 15; 

        private int originalNumP = 0;

        public override void OnPreLaunch ()
        {

            if(isActive) setEffect (false);
        }

        public override void OnUpdate ()
        {
            if (HighLogic.LoadedSceneIsEditor)
                return;

            if (softDecrease && performanceLimiterUpdate++ % performanceThreshold == 0) {
                // we launched
                if (vessel.situation != Vessel.Situations.PRELAUNCH) {
                    var remT = remainingTime ();
                    if (remT <= 0) {
                        numP = 0;
                        setEffect (false);
                        softDecrease = false;   
                    } else {
                        numP = originalNumP * remT / runningTime;
                        setEffect (true);
                    }
                   
                }
            }


            checkTimer ();

            base.OnUpdate ();
        }


        public override void OnLaunch ()
        {
            performanceLimiterUpdate = rnd.Next ();
            originalNumP = (int) numP;
            if(isActive) setEffect (true);
            startTimer ();
        }

        public override void OnTimer ()
        {
            setEffect (false);
            softDecrease = false;   
            print("Shutting off effects:" + effectName);
        }
    }
}   




