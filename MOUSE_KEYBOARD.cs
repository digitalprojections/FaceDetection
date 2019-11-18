using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    public class MOUSE_KEYBOARD
    {
        bool listen = false;
        //LL Muse and keyboard
        private readonly KeyboardListener keyboardListener = new KeyboardListener();
        private readonly MouseListener mouseListener = new MouseListener();
        private static readonly MouseListener mouseListenerClick = new MouseListener();

        public bool Listen { get => listen; set => listen = value; }

        public MOUSE_KEYBOARD()
        {
            START_CLICK_LISTENER();
            if (Properties.Settings.Default.capture_operator || Properties.Settings.Default.Recording_when_at_the_start_of_operation)
            {                
                Listen = true;
            }
            
        }
        public void START_CLICK_LISTENER()
        {
            Task.Run(() => {
                Thread.Sleep(7000);
                keyboardListener.KeyUpAll += KeyboardListener_KeyUpAll;
                mouseListener.MouseLeftDown += MouseListener_MouseLeftDown;
                mouseListenerClick.MouseMove += MouseListener_MouseMove;
            });
            }
        private void KeyboardListener_KeyUpAll(object sender, KeyEventArgs e)
        {
            if (MainForm.GetMainForm != null)
            {
                MainForm.GetMainForm.BackLight.Restart();
            }
            MouseKeyEventInit();
        }
        private void MouseListener_MouseLeftDown(object sender, MouseEventArgs e)
        {
            if (MainForm.GetMainForm != null)
            {
                MainForm.GetMainForm.BackLight.Restart();
            }
        }
        public void AddMouseAndKeyboardBack()
        {
            Listen = true;
        }
        private void MouseListener_MouseMove(object sender, MouseEventArgs e)
        {
            MouseKeyEventInit();
        }
        private void MouseKeyEventInit()
        {            
            if (Properties.Settings.Default.capture_operator && Properties.Settings.Default.Recording_when_at_the_start_of_operation && Listen && !MainForm.GetMainForm.crossbar.OPER_BAN)
            {
                Listen = false;
                //↓20191107 Nagayama added↓
                if (Properties.Settings.Default.capture_method <= 0)
                {
                    //↑20191107 Nagayama added↑
                    if (MainForm.GetMainForm.crossbar.PREEVENT_RECORDING && Properties.Settings.Default.seconds_after_event > 0)
                    {
                        TaskManager.EventAppeared(RECORD_PATH.EVENT, 1,
                            decimal.ToInt32(Properties.Settings.Default.seconds_before_event),
                            decimal.ToInt32(Properties.Settings.Default.seconds_after_event),
                            DateTime.Now);                                                
                        //↓20191107 Nagayama added↓
                    }
                    else
                    {
                        MainForm.GetMainForm.crossbar.Start(0, CAMERA_MODES.OPERATOR);                                                                        
                    }
                    MainForm.GetMainForm.crossbar.SetIconTimer(Properties.Settings.Default.seconds_after_event);
                    //↑20191107 Nagayama added↑
                }
                else
                {
                    SNAPSHOT_SAVER.TakeSnapShot(0, "event");
                    MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(0);
                }

                MainForm.GetMainForm.BackLight.Restart();
                MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.seconds_after_event));
            }
            else
            {
                Listen = false;
            }
            if (MainForm.GetMainForm != null)
            {
                MainForm.GetMainForm.BackLight.Restart();
            }
        }
    }
}
