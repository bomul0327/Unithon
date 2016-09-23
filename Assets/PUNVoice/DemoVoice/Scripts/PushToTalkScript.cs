// ----------------------------------------------------------------------------
// <copyright file="PushToTalkScript.cs" company="Exit Games GmbH">
// Photon Voice Demo for PUN- Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
// Script that implements Push-To-Talk feature of Photon Voice for PUN
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

namespace ExitGames.Demos.DemoPunVoice
{
    using Client.Photon.LoadBalancing;
    using UnityEngine;
    using UnityEngine.UI;

    public class PushToTalkScript : MonoBehaviour
    {
        [SerializeField]
        private Button pushToTalkButton;

        /// <summary>The button's text, so we can change it.</summary>
        private Text pushToTalkText;

        /// <summary>This client's voice recorder.</summary>
        private PhotonVoiceRecorder rec;


        public void OnEnable()
        {
            CharacterInstantiation.CharacterInstantiated += CharacterInstantiation_CharacterInstantiated;
        }

        public void OnDisable()
        {
            CharacterInstantiation.CharacterInstantiated -= CharacterInstantiation_CharacterInstantiated;
        }


        /// <summary>Initializes the component.</summary>
        public void Awake()
        {
            if (pushToTalkButton != null)
            {
                pushToTalkText = pushToTalkButton.GetComponentInChildren<Text>();
                if (pushToTalkText != null)
                {
                    pushToTalkText.text = "Push-To-Talk";
                }
                else
                {
                    Debug.LogWarning("Could not find the button's text component.");
                }
                pushToTalkButton.gameObject.SetActive(false);   // if it's inactive, GetComponentInChildren won't find anything.
            }
            PhotonVoiceNetwork.Client.OnStateChangeAction = OnVoiceClientStateChanged;
            PhotonVoiceSettings.Instance.VoiceDetection = true;
            PhotonVoiceSettings.Instance.AutoTransmit = false;
            PhotonVoiceSettings.Instance.AutoDisconnect = true;
            PhotonVoiceSettings.Instance.AutoConnect = true;
        }


        /// <summary>
        /// A callback for CharacterInstantiation. We need the PhotonVoiceRecorder from our new character.
        /// </summary>
        /// <param name="character">This client's character.</param>
        private void CharacterInstantiation_CharacterInstantiated(GameObject character)
        {
            rec = character.GetComponent<PhotonVoiceRecorder>();
            if (rec != null)
            {
                rec.enabled = true;
            }
        }

        /// <summary>Callback by the Voice Chat Client.</summary>
        /// <remarks>
        /// Unlike callbacks from PUN, this only updates you on the state of Voice.
        /// Voice will by default automatically enter a Voice room, when PUN joined one. That's why Joined state will happen.
        /// </remarks>
        /// <param name="state">The new state.</param>
        private void OnVoiceClientStateChanged(ClientState state)
        {
            //Debug.LogFormat("VoiceClientState={0}", state);
            if (pushToTalkButton != null)
            {
                switch (state)
                {
                    case ClientState.Joined:
                        pushToTalkButton.gameObject.SetActive(true);
                        break;
                    default:
                        pushToTalkButton.gameObject.SetActive(false);
                        break;
                }
            }
        }

        /// <summary>Activates Push-To-Talk feature</summary>
        public void PushToTalkOn()
        {
            PushToTalk(true);
            if (pushToTalkText != null) { pushToTalkText.text = "Talk now!"; }
        }

        /// <summary>Deactivates Push-To-Talk feature</summary>
        public void PushToTalkOff()
        {
            PushToTalk(false);
            if (pushToTalkText != null) { pushToTalkText.text = "Push-To-Talk"; }
        }

        /// <summary>
        /// Toggles Push-To-Talk feature
        /// </summary>
        /// <param name="toggle">value to set PTT on/off</param>
        public void PushToTalk(bool toggle)
        {
            if (rec != null)
            {
                rec.Transmit = toggle;
            }
        }

        /// <summary>This is a callback by PUN. Makes sure recording is off when leaving the room.</summary>
        /// <remarks>Unlike callbacks from the Voice Client, this only updates you on the state of PUN. Use to control voice according to changes in PUN.</remarks>
        public void OnLeftRoom()
        {
            if (rec != null)
            {
                rec.enabled = false;
                rec = null;
            }
        }

        /// <summary>Called per frame. Used to check key "v", which is used for Voice push to talk.</summary>
        public void Update()
        {
            if (Input.GetKeyDown("v"))
            {
                PushToTalkOn();
            }
            else if (Input.GetKeyUp("v"))
            {
                PushToTalkOff();
            }
        }
    }
}
