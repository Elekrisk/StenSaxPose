using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace StenSaxPose
{
    class Music
    {

        SoundPlayer Sound;
        private bool isPlaying;

        public Music(string FileName)
        {
            Sound = new SoundPlayer(Directory.GetCurrentDirectory() + "\\..\\..\\Resources\\" + FileName);
            isPlaying = false;
        }

        public void Play()
        {
            try
            {
                Sound.Play();
                isPlaying = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception source: {0}", e.Source);
                isPlaying = false;
                throw;
            }

        }

        public void Stop()
        {
            Sound.Stop();
            isPlaying = false;
        }

        public bool Playing()
        {
            return isPlaying;
        }

    }
}
