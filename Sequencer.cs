using System;

namespace FaceDetection
{
    class Sequencer
    {
        static int duration;
        static int? wait;
        
        static Action Run;
        static Action Destroy;

        static System.Timers.Timer the_timer;

        public static void Start(int dur, int? waitTime)
        {
            the_timer = new System.Timers.Timer();
            duration = dur;
            wait = waitTime;
            
            the_timer.AutoReset = true;
            the_timer.Elapsed += The_timer_Elapsed;
            the_timer.Interval = duration;
            the_timer.AutoReset = false;
            

            Run = () =>
            {
                Console.WriteLine("testing Run");
                the_timer.Start();
            };
            Destroy = () =>
            {
                Console.WriteLine("Destroyed");
            };

            Run();
        }

        private static void The_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine(duration + " " + wait + " " + the_timer.Interval);
            if (wait!=null && wait > 0)
            {
                the_timer.Interval =(int)wait;
                wait = 0;
            }
            else
            {
                //wait also ran or was not set
                //task complete                
                Destroy();
            }
            
        }
    }
}
