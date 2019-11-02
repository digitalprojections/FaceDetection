using System;

namespace FaceDetection
{
    class Sequencer
    {
        //static int duration;
        //static int wait;        
        //static Action Run;
        //static Action Destroy;
        //static MainForm.CAMERA_MODES prevmode;
        //static System.Timers.Timer the_timer = new System.Timers.Timer();
        //public static void Start(int dur, int waitTime)
        //{            
        //    duration = dur;
        //    wait = waitTime;
        //    Run = () =>
        //    {
        //        Console.WriteLine("Running camera in " + " mode");
        //        the_timer.Enabled = true;
        //    };
        //    Destroy = () =>
        //    {
        //        Console.WriteLine("Destroyed");
        //    };
        //    if (the_timer.Enabled)
        //    {                
        //    }
        //    else
        //    {
        //        //we got the same task as before, while running the camera
        //        Destroy();
        //    }            
        //    the_timer.AutoReset = false; //prevent from running - true
        //    the_timer.Elapsed += The_timer_Elapsed;
        //    the_timer.Interval = duration;
        //    Run();
        //}
        ///// <summary>
        ///// If Enabled and AutoReset are both set to false, 
        ///// and the timer has previously been enabled, 
        ///// setting the Interval property causes the Elapsed event to be raised once, 
        ///// as if the Enabled property had been set to true. 
        ///// To set the interval without raising the event, 
        ///// you can temporarily set the Enabled property to true, 
        ///// set the Interval property to the desired time interval, 
        ///// and then immediately set the Enabled property back to false.
        ///// </summary>
        //private static void The_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    Console.WriteLine(duration + " " + wait + " the_timer.Enabled: " + the_timer.Enabled);
        //    if (wait > 0)
        //    {
        //        the_timer.Enabled = true;
        //        the_timer.Interval =(int)wait;
        //        wait = 0;
        //        //the_timer.Enabled = false;
        //        //
        //    }
        //    else
        //    {
        //        the_timer.Enabled = false;
        //        the_timer.AutoReset = false;
        //        //wait also ran or was not set
        //        //task complete                
        //        Destroy();
        //    }            
        //}
    }
}
