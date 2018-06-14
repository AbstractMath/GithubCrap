using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using System;

namespace GameEngineProjectRevive.Audio {

    public enum SystemSound {
        OK,
        CANCEL,
        SIZE
    }

    public class AudioManager {

        SoundEffect [] SystemSoundEffects;
        AudioListener listener;

        const string EffectPrefix = "Audio/Effects/";
        const string SongPrefix = "Audio/Music/";

        List<Tuple<AudioEmitter, SoundEffectInstance>> spatialSounds = new List<Tuple<AudioEmitter, SoundEffectInstance>> ();

        string [] SystemSoundNames = new string [] 
        {
            "menu_ok",
            "menu_cancel"
        };

        public void LoadContent (ContentManager content)
        {
            listener = new AudioListener();
            listener.Position = Vector3.Zero;
            listener.Up = Vector3.Up;
            listener.Forward = new Vector3(1f, 0f, 0f);

            Song song = content.Load<Song>(SongPrefix + "Wander_01");
            //MediaPlayer.Play(song);

            SystemSoundEffects = LoadSystemSounds(content);
            SoundEffect.DistanceScale = 200f;
        }

        public SoundEffect LoadSoundEffect (string name, ContentManager content) {
            return content.Load<SoundEffect> (EffectPrefix + name);
        }

        private SoundEffect [] LoadSystemSounds (ContentManager content)
        {
            SoundEffect [] systemSounds = new SoundEffect [(int)SystemSound.SIZE];

            if (SystemSoundNames.Length != systemSounds.Length)
            {
                Debug.WriteLine("ERROR: system sound list mismatch");
            }

            for (int i = 0; i < systemSounds.Length; i++) 
            {
                systemSounds [i] = content.Load<SoundEffect>(EffectPrefix + SystemSoundNames[i]);
            }

            return systemSounds;

        }

        public void SetListenerPosition (Vector2 position) {
            listener.Position = new Vector3(position.X, position.Y, 0.0f);
            
            // updates spatial sounds
            foreach (var emitterToEffect in spatialSounds) {
                var effect = emitterToEffect.Item2;
                effect.Apply3D(listener, emitterToEffect.Item1);
            }
        }

        public Vector3 GetListenerPosition () {
            return listener.Position;
        }

        public SoundEffectInstance MakeEmitter (SoundEffect effect, Vector3 position) {
            AudioEmitter emitter = new AudioEmitter();
            emitter.Position = position;
            SoundEffectInstance instance = effect.CreateInstance();
            instance.Apply3D(this.listener, emitter);
            spatialSounds.Add(new Tuple<AudioEmitter, SoundEffectInstance> (emitter, instance));
            return instance;
        }

        public void ClearEmitters () {
            spatialSounds.Clear();
        }

        public void PlaySystemSound (SystemSound sound) {

            SoundEffect effect = SystemSoundEffects[(int)sound];
            effect.Play();
        }

        public void ChangeSong () {
            // do some sweet fadeout and stuff
        }

    }
}
