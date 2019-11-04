using DirectShowLib;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace FaceDetectionY
{
    class CameraManager
    {
        string sourcePath = @"D:\TEMP";
        string targetPath = String.Empty;

        //public MainForm mainForm;        

        public enum PlayState : int
        {
            Stopped,
            Paused,
            Running,
            Init
        }


        IBaseFilter pVideoMixingRenderer9 = null;
        private IGraphBuilder pGraph;
        //private Guid CLSID_SampleGrabber = new Guid("{C1F400A0-3F08-11D3-9F0B-006008039E37}"); //qedit.dll
        private PlayState CurrentState = PlayState.Stopped;
        private int WM_GRAPHNOTIFY = Convert.ToInt32("0X8000", 16) + 1;
        //public IVideoWindow videoWindow = null;
        private IMediaControl mediaControl = null;        
        private IMediaEventEx mediaEventEx = null;
        private IGraphBuilder graph = null;
        private ICaptureGraphBuilder2 pGraphBuilder = null;
        private IBaseFilter pUSB = null;
        private IBaseFilter renderFilter = null;
        private NullRenderer nullRender = null;
        private IAMStreamConfig streamConfig = null;
        private ISampleGrabber pSampleGrabber = null;        
        private VideoInfoHeader format = null;
        private AMMediaType pmt = null;

        IBaseFilter pSmartTee = null;
        IBaseFilter pAVIMux = null;

        public void GetInterfaces()
        {
            graph = (IGraphBuilder)(new FilterGraph());
            pGraphBuilder = (ICaptureGraphBuilder2)(new CaptureGraphBuilder2());
            mediaControl = (IMediaControl)graph;
            //videoWindow = (IVideoWindow)graph;
            mediaEventEx = (IMediaEventEx)graph;
            renderFilter = (IBaseFilter)new VideoMixingRenderer9();
            nullRender = new NullRenderer();
            pSampleGrabber = new SampleGrabber() as ISampleGrabber;
            //send notification messages to the control window
            int hr = mediaEventEx.SetNotifyWindow(IntPtr.Zero, WM_GRAPHNOTIFY, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);
        }
        

        void SetFormat()
        {
            int hr = 0;
            pmt = new AMMediaType();
            pmt.majorType = MediaType.Video;
            pmt.subType = MediaSubType.MJPG;
            pmt.formatType = FormatType.VideoInfo;
            pmt.fixedSizeSamples = false; //true for 640x480
            pmt.formatSize = 88;
            pmt.sampleSize = 2764800; //2764800 614400
            pmt.temporalCompression = false;
            //////////////////////////////////
            format = new VideoInfoHeader();
            format.SrcRect = new DsRect();
            format.TargetRect = new DsRect();
            format.BitRate = 5000000;
            format.AvgTimePerFrame = 666666;
            //////////////////////////////////            
            format.BmiHeader = new BitmapInfoHeader();
            format.BmiHeader.Size = 40;
            format.BmiHeader.Width = 1280;
            format.BmiHeader.Height = 720;
            format.BmiHeader.Planes = 1;
            format.BmiHeader.BitCount = 24;
            format.BmiHeader.Compression = 1196444237; //1196444237 //844715353
            format.BmiHeader.ImageSize = 2764800; //2764800 614400
            pmt.formatPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(format));
            Marshal.StructureToPtr(format, pmt.formatPtr, false);
            Debug.WriteLine(getCatName(pUSB) + " at line 130");
            streamConfig = (IAMStreamConfig)GetPin(pUSB, getCatName(pUSB));
            hr = streamConfig.SetFormat(pmt);
            DsUtils.FreeAMMediaType(pmt);
            if (hr < 0)
            {
                
                DsError.ThrowExceptionForHR(hr);
            }
        }

        //The following is called for building the PREVIEW graph
        #region PREVIEW ONLY
        public void CaptureVideo(IntPtr pbx)
        {            

            Debug.WriteLine("VIDEO FOR PREVIEW");
            pGraph = graph;
            int hr = 0;

            hr = pGraphBuilder.SetFiltergraph(pGraph);
            DsError.ThrowExceptionForHR(hr);

            pUSB = FindCaptureDevice();

            hr = pGraph.AddFilter(pUSB, "WebCamControl Video");
            DsError.ThrowExceptionForHR(hr);

            //add smartTee
            //pSmartTee = (IBaseFilter)new SmartTee();
            hr = pGraph.AddFilter(pSmartTee, "Smart Tee");
            DsError.ThrowExceptionForHR(hr);
                        
            //connect smart tee to camera 
            hr = pGraphBuilder.RenderStream(null, MediaType.Video, pUSB, null, pSmartTee);
            DsError.ThrowExceptionForHR(hr);

            hr = pSampleGrabber.SetBufferSamples(true);
            checkHR(hr, "Can't set buffer sample to true for the SampleGrabber");

            

            hr = pGraph.AddFilter(pSampleGrabber as IBaseFilter, "SampleGrabber");
            checkHR(hr, "Can't add SampleGrabber to graph");

            //connect Smart Tee and SampleGrabber
            hr = pGraph.ConnectDirect(GetPin(pSmartTee, "Preview"), GetPin(pSampleGrabber as IBaseFilter, "Input"), null);
            //hr = pGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, pSmartTee, null, pSampleGrabber);
            checkHR(hr, "Can't connect Smart Tee and SampleGrabber");
            
            //フォーマットの設定
            //暫くは一時的な値を使用してます

            SetFormat();
                        

            IVMRAspectRatioControl9 ratioControl9 = (IVMRAspectRatioControl9)renderFilter;
            hr = ratioControl9.SetAspectRatioMode(VMRAspectRatioMode.LetterBox);
            DsError.ThrowExceptionForHR(hr);

            IVMRFilterConfig9 config9 = (IVMRFilterConfig9)renderFilter;
            hr = config9.SetRenderingMode(VMR9Mode.Windowless);
            checkHR(hr, "Can't set windowless mode");

            IVMRWindowlessControl9 control9 = (IVMRWindowlessControl9)renderFilter;
            hr = control9.SetVideoClippingWindow(pbx);
            checkHR(hr, "Can't set video clipping window");
            /*
            if (MainForm.CURRENT_MODE != MainForm.CAMERA_MODES.HIDDEN)
            {
                hr = control9.SetVideoPosition(null, new DsRect(0, 0, MainForm.GetMainForm.Width, MainForm.GetMainForm.Height));
                checkHR(hr, "Can't set rectangles of the video position");
                CustomMessage.ShowMessage("NOT HIDDEN MODE " + MainForm.CURRENT_MODE + ", Active path: " + MainForm.ACTIVE_RECPATH);
            }
            */
            hr = pGraph.AddFilter(renderFilter, "My Render Filter");
            DsError.ThrowExceptionForHR(hr);

            hr = pGraphBuilder.RenderStream(null, MediaType.Video, pSampleGrabber, null, renderFilter);
            DsError.ThrowExceptionForHR(hr);
            Debug.WriteLine(DsError.GetErrorText(hr) + " is error in rendering");

            IEnumFilters enumFilters = null;
            IBaseFilter[] baseFilters = { null };
            IntPtr fetched = IntPtr.Zero;
            hr = pGraph.EnumFilters(out enumFilters);
            int r = 0;
            while (r == 0)
            {
                try
                {
                    r = enumFilters.Next(baseFilters.Length, baseFilters, fetched);
                    DsError.ThrowExceptionForHR(hr);
                    baseFilters[0].QueryFilterInfo(out FilterInfo filterInfo);
                    Debug.WriteLine(filterInfo.achName + " -filtername");
                }
                catch
                {
                    r = 1;
                    continue;
                }

            }

            

            SafeReleaseComObject(pUSB);
            SafeReleaseComObject(pAVIMux);
            SafeReleaseComObject(pGraph);
            SafeReleaseComObject(pGraphBuilder);
            SafeReleaseComObject(pSampleGrabber);
            SafeReleaseComObject(pSmartTee);
            SafeReleaseComObject(renderFilter);
            SafeReleaseComObject(ratioControl9);
            SafeReleaseComObject(control9);
            SafeReleaseComObject(config9);
            

            SetupVideoWindow();
        }
        #endregion
        public void SetWindowPosition(Size size)
        {
            int hr = 0;
            IVMRWindowlessControl9 control9 = (IVMRWindowlessControl9)renderFilter;
            hr = control9.SetVideoPosition(null, new DsRect(0, 0, FaceDetection.MainForm.GetMainForm.Width, FaceDetection.MainForm.GetMainForm.Height));
            checkHR(hr, "Can't set rectangles of the video position");
            SafeReleaseComObject(control9);
        }
        //This one is for recording
        public void CaptureVideo(IntPtr pbx, string dstFile)
        {
            pGraph = graph;
            int hr = 0;

            hr = pGraphBuilder.SetFiltergraph(pGraph);
            DsError.ThrowExceptionForHR(hr);

            pUSB = FindCaptureDevice();

            hr = pGraph.AddFilter(pUSB, "WebCamControl Video");
            DsError.ThrowExceptionForHR(hr);

            //add smartTee
            //pSmartTee = (IBaseFilter)new SmartTee();
            hr = pGraph.AddFilter(pSmartTee, "Smart Tee");
            DsError.ThrowExceptionForHR(hr);

            //add File writer
            IBaseFilter pFilewriter = (IBaseFilter)new FileWriter();
            hr = pGraph.AddFilter(pFilewriter, "File writer");
            checkHR(hr, "Can't add File writer to graph");

            //set destination filename
            IFileSinkFilter pFilewriter_sink = pFilewriter as IFileSinkFilter;
            if (pFilewriter_sink == null)
                checkHR(unchecked((int)0x80004002), "Can't get IFileSinkFilter");
            hr = pFilewriter_sink.SetFileName(dstFile, null);
            checkHR(hr, "Can't set filename");

            //add AVI Mux
            pAVIMux = (IBaseFilter)new AviDest();
            hr = pGraph.AddFilter(pAVIMux, "AVI Mux");
            checkHR(hr, "Can't add AVI Mux to graph");

            //connect AVI Mux and File writer
            hr = pGraph.ConnectDirect(GetPin(pAVIMux, "AVI Out"), GetPin(pFilewriter, "in"), null);
            checkHR(hr, "Can't connect AVI Mux and File writer");


            //connect smart tee to camera 
            hr = pGraphBuilder.RenderStream(null, MediaType.Video, pUSB, null, pSmartTee);
            DsError.ThrowExceptionForHR(hr);

            //add SampleGrabber
            //_pSampleGrabberHelper = new SampleGrabberHelper(pSampleGrabber, false);
            //_pSampleGrabberHelper.ConfigureMode();

            hr = pGraph.AddFilter(pSampleGrabber as IBaseFilter, "SampleGrabber");
            checkHR(hr, "Can't add SampleGrabber to graph");

            //connect Smart Tee and SampleGrabber
            hr = pGraph.ConnectDirect(GetPin(pSmartTee, "Preview"), GetPin(pSampleGrabber as IBaseFilter, "Input"), null);
            //hr = pGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, pSmartTee, null, pSampleGrabber);
            checkHR(hr, "Can't connect Smart Tee and SampleGrabber");


            //フォーマットの設定
            //暫くは一時的な値を使用してます

            SetFormat();

            //connect Smart Tee and AVI Mux
            hr = pGraphBuilder.RenderStream(null, MediaType.Video, pSmartTee, null, pAVIMux);
            checkHR(hr, "Can't connect Smart Tee and AVI Mux");

            IVMRAspectRatioControl9 ratioControl9 = (IVMRAspectRatioControl9)renderFilter;
            hr = ratioControl9.SetAspectRatioMode(VMRAspectRatioMode.LetterBox);
            DsError.ThrowExceptionForHR(hr);

            IVMRFilterConfig9 config9 = (IVMRFilterConfig9)renderFilter;
            hr = config9.SetRenderingMode(VMR9Mode.Windowless);
            checkHR(hr, "Can't set windowless mode");

            IVMRWindowlessControl9 control9 = (IVMRWindowlessControl9)renderFilter;
            hr = control9.SetVideoClippingWindow(pbx);
            checkHR(hr, "Can't set video clipping window");
            /*
            if (MainForm.CURRENT_MODE != MainForm.CAMERA_MODES.HIDDEN)
            {
                hr = control9.SetVideoPosition(null, new DsRect(0, 0, MainForm.GetMainForm.Width, MainForm.GetMainForm.Height));
                checkHR(hr, "Can't set rectangles of the video position");
                CustomMessage.ShowMessage("NOT HIDDEN MODE " + MainForm.CURRENT_MODE + ", Active path: " + MainForm.ACTIVE_RECPATH);
            }
            */
            hr = pGraph.AddFilter(renderFilter, "My Render Filter");
            DsError.ThrowExceptionForHR(hr);

            hr = pGraphBuilder.RenderStream(null, MediaType.Video, pSampleGrabber, null, renderFilter);
            DsError.ThrowExceptionForHR(hr);
            Debug.WriteLine(DsError.GetErrorText(hr) + " is error in rendering");

            IEnumFilters enumFilters = null;
            IBaseFilter[] baseFilters = { null };
            IntPtr fetched = IntPtr.Zero;
            hr = pGraph.EnumFilters(out enumFilters);
            int r = 0;
            while (r == 0)
            {
                try
                {
                    r = enumFilters.Next(baseFilters.Length, baseFilters, fetched);
                    DsError.ThrowExceptionForHR(hr);
                    baseFilters[0].QueryFilterInfo(out FilterInfo filterInfo);
                    Debug.WriteLine(filterInfo.achName + " -filtername");
                }
                catch
                {
                    r = 1;
                    continue;
                }

            }



            SafeReleaseComObject(pUSB);
            SafeReleaseComObject(pAVIMux);
            SafeReleaseComObject(pFilewriter);
            SafeReleaseComObject(pFilewriter_sink);
            SafeReleaseComObject(pGraph);
            SafeReleaseComObject(pGraphBuilder);
            SafeReleaseComObject(pSampleGrabber);
            SafeReleaseComObject(pSmartTee);
            SafeReleaseComObject(renderFilter);
            SafeReleaseComObject(ratioControl9);
            SafeReleaseComObject(control9);
            SafeReleaseComObject(config9);

            //Debug.WriteLine(pSampleGrabber.);
            //_pSampleGrabberHelper.SaveMode();
            SetupVideoWindow();
        }
                
        

        
        static void checkHR(int hr, string msg)
        {
            if (hr < 0)
            {
                //CustomMessage.ShowMessage(msg);
                DsError.ThrowExceptionForHR(hr);
            }
        }
        private void SetupVideoWindow()
        {
            int hr = 0;

            hr = mediaControl.Run();

            DsError.ThrowExceptionForHR(hr);
            HandleGraphEvent();

            CurrentState = PlayState.Running;
            
            //mainForm.ResumeSensor();
            //mainForm.WebCamControl_Resize(this, null);
            //videoControl.GetMaxAvailableFrameRate(pUSB.FindPin(""));
        }
        public void ReleaseInterfaces()
        {

            if (mediaControl != null)
                mediaControl.StopWhenReady();

            CurrentState = PlayState.Stopped;

            // stop notifications of events
            if (mediaEventEx != null)
                mediaEventEx.SetNotifyWindow(IntPtr.Zero, WM_GRAPHNOTIFY, IntPtr.Zero);

            //// below we relinquish ownership (IMPORTANT!) of the video window.
            //// Failing to call put_Owner can lead to assert failures within
            //// the video renderer, as it still assumes that it has a valid
            //// parent window.
            /*if (videoWindow != null)
            {
                videoWindow.put_Visible(OABool.False);
                videoWindow.put_Owner(IntPtr.Zero);
                videoWindow = null;
            }
            */
            // Release DirectShow interfaces
            if (mediaControl != null)
            {
                Marshal.ReleaseComObject(mediaControl);
                mediaControl = null;
            }


            if (mediaEventEx != null)
            {
                Marshal.ReleaseComObject(mediaEventEx);
                mediaEventEx = null;
            }
            /*
            if (videoWindow != null)
            {
                Marshal.ReleaseComObject(videoWindow);                
            }
            */
            if (graph != null)
            {
                Marshal.ReleaseComObject(graph);
                graph = null;
            }

            if (pGraphBuilder != null)
            {
                Marshal.ReleaseComObject(pGraphBuilder);
                pGraphBuilder = null;
            }
            GC.Collect();

        }
        private static void SafeReleaseComObject(object obj)
        {
            if (obj != null)
            {
                Marshal.ReleaseComObject(obj);
            }
        }

        //カメラデバイスにアクセス
        private IBaseFilter FindCaptureDevice()
        {
            ArrayList devs = new ArrayList();
            IEnumMoniker classEnum = null;
            IMoniker[] moniker = { null };

            object source = null;
            ICreateDevEnum devEnum = (ICreateDevEnum)(new CreateDevEnum());

            int hr = devEnum.CreateClassEnumerator(FilterCategory.VideoInputDevice, out classEnum, CDef.None);
            DsError.ThrowExceptionForHR(hr);

            if (classEnum == null)
            {
                throw new ApplicationException("No video capture device was detected.\\r\\n\\r\\n" + "This sample requires a video capture device, such as a USB WebCam,\\r\\nto be installed and working properly.  The sample will now close.");
            }

            IntPtr none = IntPtr.Zero;
            while (classEnum.Next(1, moniker, none) == 0)
            {

                Guid iid = typeof(IBaseFilter).GUID;
                moniker[0].BindToObject(null, null, ref iid, out source);
                Marshal.ReleaseComObject(moniker[0]);
                devs.Add(source);
            }

            Debug.WriteLine(devs.Count + " ==== cameras found");

            Marshal.ReleaseComObject(devEnum);
            Marshal.ReleaseComObject(classEnum);

            //we can now select cameras
            return (IBaseFilter)devs[0];
        }
        //カメラのPINにアクセス
        static IPin GetPin(IBaseFilter filter, string pinname)
        {
            IEnumPins epins;
            int hr = filter.EnumPins(out epins);
            if (hr < 0)
            {
                
                DsError.ThrowExceptionForHR(hr);
            }

            IntPtr fetched = Marshal.AllocCoTaskMem(4);
            IPin[] pins = new IPin[1];
            while (epins.Next(1, pins, fetched) == 0)
            {
                PinInfo pinfo;
                pins[0].QueryPinInfo(out pinfo);
                bool found = (pinfo.name == pinname);
                
                DsUtils.FreePinInfo(pinfo);
                if (found)
                    return pins[0];
            }

            //CapTest.CustomMessage.ShowMessage("Pin not found");
            DsError.ThrowExceptionForHR(hr);

            return null;
        }

        string getCatName(IBaseFilter filter)
        {
            string retval = "";
            IEnumPins epins;
            int hr = filter.EnumPins(out epins);
            if (hr < 0)
            {
                //CapTest.CustomMessage.ShowMessage("Can't enumerate pins");
                DsError.ThrowExceptionForHR(hr);
            }

            IntPtr fetched = Marshal.AllocCoTaskMem(4);
            IPin[] pins = new IPin[1];
            while (epins.Next(1, pins, fetched) == 0)
            {
                PinInfo pinfo;
                pins[0].QueryPinInfo(out pinfo);
                bool found = (pinfo.name == "Capture" || pinfo.name == "キャプチャ");
                //CapTest.CustomMessage.ShowMessage(pinfo.name + " is pinname on getCatName");
                DsUtils.FreePinInfo(pinfo);
                if (found)
                    retval = pinfo.name;
            }
            //CapTest.CustomMessage.ShowMessage("Pin found " + retval);
            return retval;
        }


        //デバッグ用イベント情報
        private void HandleGraphEvent()
        {
            int hr = 0;
            EventCode evCode = 0;
            IntPtr evParam1 = IntPtr.Zero;
            IntPtr evParam2 = IntPtr.Zero;

            while (mediaEventEx != null && mediaEventEx.GetEvent(out evCode, out evParam1, out evParam2, 0) == 0)
            {
                // Free event parameters to prevent memory leaks associated with
                // event parameter data.  While this application is not interested
                // in the received events, applications should always process them.
                hr = mediaEventEx.FreeEventParams(evCode, evParam1, evParam2);
                DsError.ThrowExceptionForHR(hr);

                //CustomMessage.ShowMessage(evCode + " -evCode " + evParam1 + " " + evParam2);
                // Insert event processing code here, if desired (see http://msdn2.microsoft.com/en-us/library/ms783649.aspx)
            }
        }

    }
}
