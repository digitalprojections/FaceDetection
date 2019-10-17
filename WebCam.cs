using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    class WebCam
    {
        
        private Panel cpanel;

        Guid CLSID_VideoCaptureSources = new Guid("{860BB310-5D01-11D0-BD3B-00A0C911CE86}"); //
        Guid CLSID_SampleGrabber = new Guid("{C1F400A0-3F08-11D3-9F0B-006008039E37}"); //qedit.dll
        Guid CLSID_VideoMixingRenderer9 = new Guid("{51B4ABF3-748F-4E3B-A276-C828330E926A}"); //quartz.dll

        DsDevice[] dsDevices;
        IBaseFilter pFHDCamera;

        private IMoniker GetDeviceMoniker(int index)
        {
            dsDevices = DsDevice.GetDevicesOfCat(DsGuid.FromGuid(CLSID_VideoCaptureSources));
            return dsDevices[index].Mon;
        }

        private void BuildGraph(IGraphBuilder pGraph, string dstFile1)
        {
            int hr = 0;
            
            //graph builder
            ICaptureGraphBuilder2 pBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = pBuilder.SetFiltergraph(pGraph);
            checkHR(hr, "Can't SetFiltergraph");

            dsDevices = DsDevice.GetDevicesOfCat(DsGuid.FromGuid(CLSID_VideoCaptureSources));
            Console.WriteLine(dsDevices[0].Name);
            //add YOUR Camera


            pFHDCamera = CreateFilterByName(dsDevices[0].Name, CLSID_VideoCaptureSources);            
            hr = pGraph.AddFilter(pFHDCamera, "FHD Camera");
            checkHR(hr, "Can't add FHD Camera to graph");

            //add AVI Mux
            IBaseFilter pAVIMux = (IBaseFilter)new AviDest();
            hr = pGraph.AddFilter(pAVIMux, "AVI Mux");
            checkHR(hr, "Can't add AVI Mux to graph");

            //add File writer
            IBaseFilter pFilewriter = (IBaseFilter)new FileWriter();
            hr = pGraph.AddFilter(pFilewriter, "File writer");
            checkHR(hr, "Can't add File writer to graph");
            //set destination filename
            IFileSinkFilter pFilewriter_sink = pFilewriter as IFileSinkFilter;
            if (pFilewriter_sink == null)
                checkHR(unchecked((int)0x80004002), "Can't get IFileSinkFilter");
            hr = pFilewriter_sink.SetFileName(dstFile1, null);
            checkHR(hr, "Can't set filename");

            //add Video Renderer
            IBaseFilter pVideoRenderer = (IBaseFilter)new VideoRenderer();
            hr = pGraph.AddFilter(pVideoRenderer, "Video Renderer");
            checkHR(hr, "Can't add Video Renderer to graph");

            //add Smart Tee
            IBaseFilter pSmartTee = (IBaseFilter)new SmartTee();
            hr = pGraph.AddFilter(pSmartTee, "Smart Tee");
            checkHR(hr, "Can't add Smart Tee to graph");

            //add SampleGrabber
            IBaseFilter pSampleGrabber = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_SampleGrabber));
            hr = pGraph.AddFilter(pSampleGrabber, "SampleGrabber");
            checkHR(hr, "Can't add SampleGrabber to graph");
            #region FORMAT
            /*
            AMMediaType pSampleGrabber_pmt = new AMMediaType();
            pSampleGrabber_pmt.majorType = MediaType.Video;
            pSampleGrabber_pmt.subType = MediaSubType.MJPG;
            pSampleGrabber_pmt.formatType = FormatType.VideoInfo;
            pSampleGrabber_pmt.fixedSizeSamples = true;
            pSampleGrabber_pmt.formatSize = 88;
            pSampleGrabber_pmt.sampleSize = 6220800;
            pSampleGrabber_pmt.temporalCompression = false;
            VideoInfoHeader pSampleGrabber_format = new VideoInfoHeader();
            pSampleGrabber_format.SrcRect = new DsRect();
            pSampleGrabber_format.TargetRect = new DsRect();
            pSampleGrabber_format.BitRate = 1244160000;
            pSampleGrabber_format.AvgTimePerFrame = 400000;
            pSampleGrabber_format.BmiHeader = new BitmapInfoHeader();
            pSampleGrabber_format.BmiHeader.Size = 40;
            pSampleGrabber_format.BmiHeader.Width = 1920;
            pSampleGrabber_format.BmiHeader.Height = 1080;
            pSampleGrabber_format.BmiHeader.Planes = 1;
            pSampleGrabber_format.BmiHeader.BitCount = 24;
            pSampleGrabber_format.BmiHeader.Compression = 1196444237;
            pSampleGrabber_format.BmiHeader.ImageSize = 6220800;
            pSampleGrabber_pmt.formatPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(pSampleGrabber_format));
            Marshal.StructureToPtr(pSampleGrabber_format, pSampleGrabber_pmt.formatPtr, false);
            hr = ((ISampleGrabber)pSampleGrabber).SetMediaType(pSampleGrabber_pmt);
            DsUtils.FreeAMMediaType(pSampleGrabber_pmt);
            checkHR(hr, "Can't set media type to sample grabber");
            */
            #endregion FORMAT
            //connect FHD Camera and SampleGrabber
            hr = pGraph.ConnectDirect(GetPin(pFHDCamera, "キャプチャ"), GetPin(pSampleGrabber, "Input"), null);
            checkHR(hr, "Can't connect FHD Camera and SampleGrabber");

            //connect SampleGrabber and Smart Tee
            hr = pGraph.ConnectDirect(GetPin(pSampleGrabber, "Output"), GetPin(pSmartTee, "Input"), null);
            checkHR(hr, "Can't connect SampleGrabber and Smart Tee");

            //connect Smart Tee and AVI Mux
            hr = pGraph.ConnectDirect(GetPin(pSmartTee, "Capture"), GetPin(pAVIMux, "Input 01"), null);
            checkHR(hr, "Can't connect Smart Tee and AVI Mux");

            //connect AVI Mux and File writer
            hr = pGraph.ConnectDirect(GetPin(pAVIMux, "AVI Out"), GetPin(pFilewriter, "in"), null);
            checkHR(hr, "Can't connect AVI Mux and File writer");

            ///////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////
            //add Video Mixing Renderer 9
            IBaseFilter pVideoMixingRenderer9 = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_VideoMixingRenderer9));
            hr = pGraph.AddFilter(pVideoMixingRenderer9, "Video Mixing Renderer 9");
            checkHR(hr, "Can't add Video Mixing Renderer 9 to graph");
            
            IVMRFilterConfig9 config9 = (IVMRFilterConfig9)pVideoMixingRenderer9;
            hr = config9.SetRenderingMode(VMR9Mode.Windowless);
            checkHR(hr, "Can't set windowless mode");

            IVMRWindowlessControl9 control9 = (IVMRWindowlessControl9)pVideoMixingRenderer9;
            hr = control9.SetVideoClippingWindow(cpanel.Handle);
            checkHR(hr, "Can't set video clipping window");

            hr = control9.SetVideoPosition(null, new DsRect(0, 0, 1980, 1080));
            checkHR(hr, "Can't set rectangles of the video position");
            //must 
            ///////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////

            //add MJPEG Decompressor
            IBaseFilter pMJPEGDecompressor = (IBaseFilter)new MjpegDec();
            hr = pGraph.AddFilter(pMJPEGDecompressor, "MJPEG Decompressor");
            checkHR(hr, "Can't add MJPEG Decompressor to graph");

            //connect Smart Tee and MJPEG Decompressor
            hr = pGraph.ConnectDirect(GetPin(pSmartTee, "Preview"), GetPin(pMJPEGDecompressor, "XForm In"), null);
            checkHR(hr, "Can't connect Smart Tee and MJPEG Decompressor");

            //add Color Space Converter
            IBaseFilter pColorSpaceConverter3 = (IBaseFilter)new Colour();
            hr = pGraph.AddFilter(pColorSpaceConverter3, "Color Space Converter");
            checkHR(hr, "Can't add Color Space Converter to graph");

            //connect MJPEG Decompressor and Color Space Converter
            hr = pGraph.ConnectDirect(GetPin(pMJPEGDecompressor, "XForm Out"), GetPin(pColorSpaceConverter3, "Input"), null);
            checkHR(hr, "Can't connect MJPEG Decompressor and Color Space Converter");

            //connect Color Space Converter and Video Mixing Renderer 9
            hr = pGraph.ConnectDirect(GetPin(pColorSpaceConverter3, "XForm Out"), GetPin(pVideoMixingRenderer9, "VMR Input0"), null);
            checkHR(hr, "Can't connect Color Space Converter and Video Mixing Renderer 9");


        }



        public static IBaseFilter CreateFilterByName(string filterName, Guid category)
        {
            int hr = 0;
            DsDevice[] devices = DsDevice.GetDevicesOfCat(category);
            foreach (DsDevice dev in devices)
                if (dev.Name == filterName)
                {
                    IBaseFilter filter = null;
                    IBindCtx bindCtx = null;
                    try
                    {
                        hr = CreateBindCtx(0, out bindCtx);
                        DsError.ThrowExceptionForHR(hr);
                        Guid guid = typeof(IBaseFilter).GUID;
                        object obj;
                        dev.Mon.BindToObject(bindCtx, null, ref guid, out obj);
                        filter = (IBaseFilter)obj;
                    }
                    finally
                    {
                        if (bindCtx != null) Marshal.ReleaseComObject(bindCtx);
                    }
                    return filter;
                }
            return null;
        }
        public WebCam(Panel panel, int w, int h)
        {

            cpanel = panel;
            try
            {
                IGraphBuilder graph = (IGraphBuilder)new FilterGraph();
                Console.WriteLine("Building graph...");
                BuildGraph(graph, @"test.avi");                

                IMediaControl mediaControl = (IMediaControl)graph;
                IMediaEvent mediaEvent = (IMediaEvent)graph;
                int hr = mediaControl.Run();
                checkHR(hr, "Can't run the graph");
                bool stop = false;
                while (!stop)
                {
                    System.Threading.Thread.Sleep(500);
                    Console.Write(".");
                    EventCode ev;
                    IntPtr p1, p2;
                    System.Windows.Forms.Application.DoEvents();
                    while (mediaEvent.GetEvent(out ev, out p1, out p2, 0) == 0)
                    {
                        if (ev == EventCode.Complete || ev == EventCode.UserAbort)
                        {
                            Console.WriteLine("Done!");
                            stop = true;
                        }
                        else
                        if (ev == EventCode.ErrorAbort)
                        {
                            Console.WriteLine("An error occured: HRESULT={0:X}", p1);
                            mediaControl.Stop();
                            stop = true;
                        }
                        mediaEvent.FreeEventParams(ev, p1, p2);
                    }
                }
            }
            catch (COMException ex)
            {
                Console.WriteLine("COM error: " + ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
        }
        static void checkHR(int hr, string msg)
        {
            if (hr < 0)
            {
                Console.WriteLine(msg);
                DsError.ThrowExceptionForHR(hr);
            }
        }
        static IPin GetPin(IBaseFilter filter, string pinname)
        {
            IEnumPins epins;
            int hr = filter.EnumPins(out epins);
            checkHR(hr, "Can't enumerate pins");
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
            checkHR(-1, "Pin not found");
            return null;
        }

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);
    }


    

}
