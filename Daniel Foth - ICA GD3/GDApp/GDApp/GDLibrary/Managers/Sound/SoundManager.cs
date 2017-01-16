using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using GDApp;

namespace GDLibrary
{

    public class SoundManager : GameComponent
    {
        #region Fields
        private Main game;
        protected AudioEngine audioEngine;
        protected WaveBank waveBank;
        protected SoundBank soundBank;

        protected List<Cue3D> cueList3D;
        protected HashSet<string> playSet3D;

        protected AudioListener audioListener;
        protected List<string> categories;
        private float volume;
 
        #endregion

        #region Properties
        public float Volume 
        { 
            get; 
            private set; 
        }
        #endregion


        //See http://rbwhitaker.wikidot.com/audio-tutorials
        //See http://msdn.microsoft.com/en-us/library/ff827590.aspx
        //See http://msdn.microsoft.com/en-us/library/dd940200.aspx

        public SoundManager(Main game, string audioEngineStr, string waveBankStr, string soundBankStr)
            : base(game)
        {
            this.game = game;
            this.audioEngine = new AudioEngine(@"" + audioEngineStr);
            this.waveBank = new WaveBank(audioEngine, @"" + waveBankStr);
            this.soundBank = new SoundBank(audioEngine, @"" + soundBankStr);
            this.cueList3D = new List<Cue3D>();
            this.playSet3D = new HashSet<string>();
            this.audioListener = new AudioListener();

            #region Event Handling
            this.game.EventDispatcher.SoundChanged += EventDispatcher_SoundChanged;
            #endregion
        }

        #region Event Handling
        private void EventDispatcher_SoundChanged(EventData eventData)
        {
            SoundEventData soundEventData = eventData as SoundEventData;

            if(soundEventData != null)
            {
                //control 2D sounds through events
                if (soundEventData.EventType == EventActionType.OnPlay2DCue)
                    this.PlayCue(soundEventData.CueName);
                else if (soundEventData.EventType == EventActionType.OnPause2DCue)
                    this.PauseCue(soundEventData.CueName);
                else if (soundEventData.EventType == EventActionType.OnResume2DCue)
                    this.ResumeCue(soundEventData.CueName);
                else if (soundEventData.EventType == EventActionType.OnStop2DCue)
                    this.StopCue(soundEventData.CueName, soundEventData.AudioStopOptions);

                //control 3D sounds through events
                if (soundEventData.EventType == EventActionType.OnPlay3DCue)
                    this.Play3DCue(soundEventData.CueName, soundEventData.AudioEmitter);
                else if (soundEventData.EventType == EventActionType.OnPause3DCue)
                    this.Pause3DCue(soundEventData.CueName);
                else if (soundEventData.EventType == EventActionType.OnResume3DCue)
                    this.Resume3DCue(soundEventData.CueName);
                else if (soundEventData.EventType == EventActionType.OnStop3DCue)
                    this.Stop3DCue(soundEventData.CueName, AudioStopOptions.Immediate);
                else if (soundEventData.EventType == EventActionType.OnStopAll3DCues)
                    this.StopAll3DCues(soundEventData.AudioStopOptions);
            }
        }
        #endregion

        public override void Initialize()
        {
            base.Initialize();
        }

        /*************** Play, Pause, Resume, and Stop 2D sound cues ***************/

        // Plays a 2D cue e.g menu, game music etc
        public void PlayCue(string cueName)
        {
            if (!playSet3D.Contains(cueName)) //if we have not already been asked to play this in the current update loop then play it
            {
                Cue cue = this.soundBank.GetCue(cueName);
                cue.Play();
            }
        }
        //pauses a 2D cue
        public void PauseCue(string cueName)
        {
            Cue cue = this.soundBank.GetCue(cueName);
            if((cue != null) && (cue.IsPlaying))
                cue.Pause();
        }

        //resumes a paused 2D cue
        public void ResumeCue(string cueName)
        {
            Cue cue = this.soundBank.GetCue(cueName);
            if ((cue != null) && (cue.IsPaused))
                cue.Resume();
        }

        //stop a 2D cue - AudioStopOptions: AsAuthored and Immediate
        public void StopCue(string cueName, AudioStopOptions audioStopOptions)
        {
            Cue cue = this.soundBank.GetCue(cueName);
            if ((cue != null) && (cue.IsPlaying))
            {
                cue.Stop(audioStopOptions);
            }
        }

        /*************** Play, Pause, Resume, and Stop 3D sound cues ***************/

            // Plays a cue to be heard from the perspective of a player or camera in the game i.e. in 3D
        public void Play3DCue(string cueName, AudioEmitter audioEmitter)
        {
            Cue3D sound = new Cue3D();

            sound.Cue = soundBank.GetCue(cueName);
            if (!this.playSet3D.Contains(cueName)) //if we have not already been asked to play this in the current update loop then play it
            {
                sound.Emitter = audioEmitter;
                sound.Cue.Apply3D(audioListener, audioEmitter);
                sound.Cue.Play();
                this.cueList3D.Add(sound);
                this.playSet3D.Add(cueName);
            }
        }
        //pause a 3D cue
        public void Pause3DCue(string cueName)
        {
            Cue3D cue3D = Get3DCue(cueName);
            if ((cue3D != null) && (cue3D.Cue.IsPlaying))
                cue3D.Cue.Pause();
        }
        //resumes a paused 3D cue
        public void Resume3DCue(string cueName)
        {
            Cue3D cue3D = Get3DCue(cueName);
            if ((cue3D != null) && (cue3D.Cue.IsPaused))
                cue3D.Cue.Resume();
        }

        //stop a 3D cue - AudioStopOptions: AsAuthored and Immediate
        public void Stop3DCue(string cueName, AudioStopOptions audioStopOptions)
        {
            Cue3D cue3D = Get3DCue(cueName);
            if (cue3D != null)
            {
                cue3D.Cue.Stop(audioStopOptions);
                this.playSet3D.Remove(cue3D.Cue.Name);
                this.cueList3D.Remove(cue3D);
            }
        }
        //stops all 3D cues - AudioStopOptions: AsAuthored and Immediate
        public void StopAll3DCues(AudioStopOptions audioStopOptions)
        {
            foreach (Cue3D cue3D in this.cueList3D)
            {
                cue3D.Cue.Stop(audioStopOptions);
                this.cueList3D.Remove(cue3D);
                this.playSet3D.Remove(cue3D.Cue.Name);
            }
        }
        //retrieves a 3D cue from the list of currently active cues
        public Cue3D Get3DCue(string name)
        {
            foreach (Cue3D cue3D in this.cueList3D)
            {
                if (cue3D.Cue.Name.Equals(name))
                    return cue3D;
            }
            return null;
        }

        //we can control the volume for each category in the sound bank (i.e. diegetic and non-diegetic)
        public void SetVolume(float volume, string soundCategory)
        {
            //if volume will be in appropriate range (0-1) then set it
            this.volume = ((volume >= 0) && (volume <= 1)) ? volume : 0.5f;
            //set by category
            this.audioEngine.GetCategory(soundCategory).SetVolume(volume);
        }
        public float GetVolume()
        {
            return volume;
        }
        public void ChangeVolume(float delta, string soundCategory)
        {
            //requested new volume
            float newVolume = this.volume + delta;
            //if requested volume will be in appropriate range (0-1) then set it
            this.volume = ((newVolume >= 0) && (newVolume <= 1)) ? newVolume : 0.5f;
            //set by category
            this.audioEngine.GetCategory(soundCategory).SetVolume(volume);
        }

     
        //Called by the listener to update relative positions (i.e. everytime the 1st Person camera moves it should call this method so that the 3D sounds heard reflect the new camera position)
        public void UpdateListenerPosition(Vector3 position)
        {
            this.audioListener.Position = position;
        }

        // Pause all playing sounds
        public void PauseAll()
        {
            foreach (Cue3D cue in cueList3D)
            {
                cue.Cue.Pause();
            }
        }

        public void ResumeAll()
        {
            foreach (Cue3D cue in cueList3D)
            {
                cue.Cue.Resume();
            }
        }

        public override void Update(GameTime gameTime)
        {
            this.audioEngine.Update();
            for (int i = 0; i < cueList3D.Count; i++)
            {
                if (this.cueList3D[i].Cue.IsPlaying)
                    this.cueList3D[i].Cue.Apply3D(audioListener, this.cueList3D[i].Emitter);
                else if (this.cueList3D[i].Cue.IsStopped)
                {
                    this.playSet3D.Remove(this.cueList3D[i].Cue.Name);
                    this.cueList3D.RemoveAt(i--);
                }
            }
            base.Update(gameTime);
        }

        //to do - dispose
    }
}
