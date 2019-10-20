using DirectShowLib;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace FaceDetection
{
    class CameraManager
    {
        string sourcePath = @"D:\TEMP";
        string targetPath = String.Empty;

        public enum PlayState : int
        {
            Stopped,
            Paused,
            Running,
            Init
        }
        public Action Stop { get; private set; }
        public Action Release { get; private set; }
        public Func<Bitmap> GetBitmap { get; private set; }

        private IGraphBuilder pGraph;
        //private Guid CLSID_SampleGrabber = new Guid("{C1F400A0-3F08-11D3-9F0B-006008039E37}"); //qedit.dll
        private PlayState CurrentState = PlayState.Stopped;
        private int WM_GRAPHNOTIFY = Convert.ToInt32("0X8000", 16) + 1;
        public IVideoWindow videoWindow = null;
        private IMediaControl mediaControl = null;
        private IMediaEventEx mediaEventEx = null;
        private IGraphBuilder graph = null;
        private ICaptureGraphBuilder2 pGraphBuilder = null;
        private IBaseFilter pUSB = null;
        private IBaseFilter renderFilter = null;
        //private NullRenderer nullRender = null;
        private IAMStreamConfig streamConfig = null;
       private SampleGrabber pSampleGrabber = null;
        
        private VideoInfoHeader format = null;
        private AMMediaType pmt = null;
        private ISampleGrabber i_grabber = null;

        IBaseFilter pSmartTee = null;
        IBaseFilter pAVIMux = null;

        public void GetInterfaces()
        {
            graph = (IGraphBuilder)(new FilterGraph());
            pGraphBuilder = (ICaptureGraphBuilder2)(new CaptureGraphBuilder2());
            mediaControl = (IMediaControl)graph;
            videoWindow = (IVideoWindow)graph;
            mediaEventEx = (IMediaEventEx)graph;
            renderFilter = (IBaseFilter)new VideoMixingRenderer9();
            pSampleGrabber = new SampleGrabber();            
            i_grabber = pSampleGrabber as ISampleGrabber;
            var mt = new AMMediaType();
            mt.majorType = MediaType.Video;
            mt.subType = MediaSubType.RGB24;
            i_grabber.SetMediaType(mt);
            //send notification messages to the control window
            int hr = mediaEventEx.SetNotifyWindow(IntPtr.Zero, WM_GRAPHNOTIFY, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);
            DsUtils.FreeAMMediaType(mt);
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
                CustomMessage.ShowMessage("Can`t set format");
                DsError.ThrowExceptionForHR(hr);
            }
        }

        //The following is called for building the PREVIEW graph
       
        //This one is for recording
        public CameraManager(int cameraIndex, Size size, double fps, IntPtr pbx, string dstFileName)
        {
            GetInterfaces();

            string str = Path.Combine(FaceDetection.Properties.Settings.Default.video_file_location, (cameraIndex + 1).ToString());
            Console.WriteLine(str);
            Directory.CreateDirectory(str);
            targetPath = Path.Combine(str, dstFileName);
            Console.WriteLine(targetPath);

            //var camera_list = FindDevices();
            //if (cameraIndex >= camera_list) throw new ArgumentException("USB camera is not available.", "index");

            //Init(cameraIndex, size, fps, pbx, targetPath);

            graph = (IGraphBuilder)(new FilterGraph());
            pGraph = graph;
            int hr = 0;

            hr = pGraphBuilder.SetFiltergraph(pGraph);
            DsError.ThrowExceptionForHR(hr);

            pUSB = FindCaptureDevice();

            hr = pGraph.AddFilter(pUSB, "WebCamControl Video");
            DsError.ThrowExceptionForHR(hr);

            //add smartTee
            pSmartTee = (IBaseFilter)new SmartTee();
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
            
            hr = pFilewriter_sink.SetFileName(targetPath, null);
            checkHR(hr, "Can't set filename");

            //add AVI Mux
            pAVIMux = (IBaseFilter)new AviDest();
            hr = pGraph.AddFilter(pAVIMux, "AVI Mux");
            checkHR(hr, "Can't add AVI Mux to graph");

            //connect AVI Mux and File writer
            hr = pGraph.ConnectDirect(GetPin(pAVIMux, "AVI Out"), GetPin(pFilewriter, "in"), null);
            checkHR(hr, "Can't connect AVI Mux and File writer");

            
            
            hr = pGraph.AddFilter(pSampleGrabber as IBaseFilter, "SampleGrabber");
            checkHR(hr, "Can't add SampleGrabber to graph");
            //i_grabber = (ISampleGrabber)pSampleGrabber;
            i_grabber.SetBufferSamples(true); //サンプルグラバでのサンプリングを開始

            //connect smart tee to camera 
            hr = pGraphBuilder.RenderStream(null, MediaType.Video, pUSB, null,(IBaseFilter)pSampleGrabber);
            DsError.ThrowExceptionForHR(hr);

            //connect AVI Mux and File writer
            //hr = pGraph.ConnectDirect(GetPin((IBaseFilter)pSampleGrabber, "Output"), GetPin(pSmartTee, "Input"), null);
            //checkHR(hr, "Can't connect SampleGrabber and Smartee");

            

            //フォーマットの設定
            //暫くは一時的な値を使用してます

            //SetFormat();

            //connect Smart Tee and AVI Mux
            hr = pGraphBuilder.RenderStream(null, MediaType.Video, pSmartTee, null, pAVIMux);
            checkHR(hr, "Can't connect Smart Tee and AVI Mux");

            IVMRAspectRatioControl9 ratioControl9 = (IVMRAspectRatioControl9)renderFilter;
            hr = ratioControl9.SetAspectRatioMode(VMRAspectRatioMode.LetterBox);
            DsError.ThrowExceptionForHR(hr);

            hr = pGraph.AddFilter(renderFilter, "My Render Filter");
            DsError.ThrowExceptionForHR(hr);



            IVMRFilterConfig9 config9 = (IVMRFilterConfig9)renderFilter;
            hr = config9.SetRenderingMode(VMR9Mode.Windowless);
            checkHR(hr, "Can't set windowless mode");

            IVMRWindowlessControl9 control9 = (IVMRWindowlessControl9)renderFilter;
            hr = control9.SetVideoClippingWindow(pbx);
            checkHR(hr, "Can't set video clipping window");

            hr = control9.SetVideoPosition(null, new DsRect(0, 0, size.Width, size.Height));
            checkHR(hr, "Can't set rectangles of the video position");

            //connect Smart Tee and SampleGrabber
            hr = pGraphBuilder.RenderStream(null, MediaType.Video, pSmartTee, null, renderFilter);
            checkHR(hr, "Can't connect Smart Tee and render");

            //hr = pGraphBuilder.RenderStream(null, MediaType.Video, null, null, renderFilter);
            DsError.ThrowExceptionForHR(hr);
            Debug.WriteLine(DsError.GetErrorText(hr) + " is error in rendering");

            {
                var mt = new AMMediaType();
                i_grabber.GetConnectedMediaType(mt);
                var header = (VideoInfoHeader)Marshal.PtrToStructure(mt.formatPtr, typeof(VideoInfoHeader));
                var width = header.BmiHeader.Width;
                var height = header.BmiHeader.Height;
                var stride = width * (header.BmiHeader.BitCount / 8);
                DsUtils.FreeAMMediaType(mt);
                
                GetBitmap = () => GetBitmapMain(i_grabber, width, height, stride);
            }
            Stop = () => ReleaseInterfaces();
            Release = () =>
            {
                Stop();

                SafeReleaseComObject(pSampleGrabber);
                SafeReleaseComObject(control9);
                SafeReleaseComObject(config9);
                SafeReleaseComObject(ratioControl9);
                SafeReleaseComObject(pGraphBuilder);
                SafeReleaseComObject(renderFilter);
                SafeReleaseComObject(i_grabber);
                SafeReleaseComObject(pGraphBuilder);
                SafeReleaseComObject(graph);
                SafeReleaseComObject(pAVIMux);
                SafeReleaseComObject(pSmartTee);
                SafeReleaseComObject(pFilewriter);
                SafeReleaseComObject(pFilewriter_sink);
            };
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
            mediaControl = (IMediaControl)graph;            
            hr = mediaControl.Run();
            checkHR(hr, "Can't run the graph");
        }

        private Bitmap GetBitmapMain(ISampleGrabber i_grabber, int width, int height, int stride)
        {
            try
            {
                return GetBitmapMainMain(i_grabber, width, height, stride);
            }
            catch (COMException ex)
            {
                const uint VFW_E_WRONG_STATE = 0x80040227;
                if ((uint)ex.ErrorCode == VFW_E_WRONG_STATE)
                {
                    // image data is not ready yet. return empty bitmap.
                    return new Bitmap(width, height);
                }
                throw;
            }
        }
        private Bitmap GetBitmapMainMain(ISampleGrabber i_grabber, int width, int height, int stride)
        {
            int sz = 0;
            i_grabber.GetCurrentBuffer(ref sz, IntPtr.Zero); // IntPtr.Zeroで呼び出してバッファサイズ取得
            if (sz == 0) return null;
            // メモリ確保し画像データ取得
            var ptr = Marshal.AllocCoTaskMem(sz);
            i_grabber.GetCurrentBuffer(ref sz, ptr);
            // 画像データをbyte配列に入れなおす
            var data = new byte[sz];
            Marshal.Copy(ptr, data, 0, sz);
            Bitmap result = null;
            try
            {
                // 画像を作成
                result = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var bmp_data = result.LockBits(new Rectangle(Point.Empty, result.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                //Debug.WriteLine(i_grabber + " igrabber " + ptr + " pointer, " + data.Length + " data  length " + sz);

                // 上下反転させながら1行ごとコピー
                for (int y = 0; y < height; y++)
                {
                    var src_idx = sz - (stride * (y + 1)); // 最終行から
                    var dst = new IntPtr(bmp_data.Scan0.ToInt32() + (stride * y));
                    Marshal.Copy(data, src_idx, dst, stride);
                }
                result.UnlockBits(bmp_data);
                Marshal.FreeCoTaskMem(ptr);
            }
            catch (ArgumentException ax)
            {
                //SnapShot.Form1.CleanBuffer();
                //System.Windows.Forms.MessageBox.Show(ax.Message);
                Console.WriteLine(ax.Message);
            }
            return result;
        }
        public void SetWindowPosition(Size size)
        {
            int hr = 0;
            IVMRWindowlessControl9 control9 = (IVMRWindowlessControl9)renderFilter;
            hr = control9.SetVideoPosition(null, new DsRect(0, 0, size.Width, size.Height));
            checkHR(hr, "Can't set rectangles of the video position");
        }
        
        private volatile ManualResetEvent m_PictureReady = null;
        private IntPtr m_ipBuffer = IntPtr.Zero;
        private int m_videoWidth = 1280;
        private int m_videoHeight = 720;
        private int m_videoBitCount = 24;
        private volatile bool m_bWantOneFrame = false;
        

        public IntPtr GetNextFrame()
        {
            Debug.WriteLine("VALUES " + m_videoBitCount + " " + m_videoWidth);
            // get ready to wait for new image

            m_ipBuffer = Marshal.AllocCoTaskMem(Math.Abs(m_videoBitCount / 8 * m_videoWidth) * m_videoHeight);


            int dwStartTime = System.Environment.TickCount;

            while (true)

            {

                if (System.Environment.TickCount - dwStartTime > 1000) break; //1000 milliseconds 

            }

            // Got one
            return m_ipBuffer;
        }
        void checkHR(int hr, string msg)
        {
            if (hr < 0)
            {
                Console.WriteLine(msg);
                ReleaseInterfaces();
                DsError.ThrowExceptionForHR(hr);
            }
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
            if (videoWindow != null)
            {
                videoWindow.put_Visible(OABool.False);
                videoWindow.put_Owner(IntPtr.Zero);
            }

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

            if (videoWindow != null)
            {
                Marshal.ReleaseComObject(videoWindow);
                videoWindow = null;
            }

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
                CustomMessage.ShowMessage("Can't enumerate pins");
                DsError.ThrowExceptionForHR(hr);
            }

            IntPtr fetched = Marshal.AllocCoTaskMem(4);
            IPin[] pins = new IPin[1];
            while (epins.Next(1, pins, fetched) == 0)
            {
                PinInfo pinfo;
                pins[0].QueryPinInfo(out pinfo);
                bool found = (pinfo.name == pinname);
                CustomMessage.ShowMessage(pinfo.name + " is PIN for ");
                DsUtils.FreePinInfo(pinfo);
                if (found)
                    return pins[0];
            }

            CustomMessage.ShowMessage("Pin not found");
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
                CustomMessage.ShowMessage("Can't enumerate pins");
                DsError.ThrowExceptionForHR(hr);
            }

            IntPtr fetched = Marshal.AllocCoTaskMem(4);
            IPin[] pins = new IPin[1];
            while (epins.Next(1, pins, fetched) == 0)
            {
                PinInfo pinfo;
                pins[0].QueryPinInfo(out pinfo);
                bool found = (pinfo.name == "Capture" || pinfo.name == "キャプチャ");
                CustomMessage.ShowMessage(pinfo.name + " is pinname on getCatName");
                DsUtils.FreePinInfo(pinfo);
                if (found)
                    retval = pinfo.name;
            }
            CustomMessage.ShowMessage("Pin found " + retval);
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

                Console.WriteLine(evCode + " -evCode " + evParam1 + " " + evParam2);
                // Insert event processing code here, if desired (see http://msdn2.microsoft.com/en-us/library/ms783649.aspx)
            }
        }
    }
}
