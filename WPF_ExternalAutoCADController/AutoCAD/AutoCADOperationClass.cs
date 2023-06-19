using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPF_ExternalAutoCADController.AppHandling;

namespace WPF_ExternalAutoCADController.AutoCAD
{
        class AutoCADOperationClass
        {
            private AcadApplication _acadApp { get; set; }
            bool isConnected { get; set; } = false;
            long ProcessHandle { get; set; } = 0000;
            public EventWaitHandle waitHandle;

            // Version specific strings for handling application via Interop.
            const string AcadVersion = "AutoCAD.Application.24";
            const string AutoCADWorkingDirectory_envVariable = "AcadPath2021";


            public AutoCADOperationClass()
            {

            }

            [CommandMethod("PerformSingleDrawingPurge")]
            public bool PerformSingleDrawingPurge(string fileName)
            {
                bool WasSuccess = false;
                try
                {
                    AcadDocument doc = GetAcad().ActiveDocument;
                    WaitForAcadCommand(doc, "SAE_ETC_ManualPurge " + fileName + "\n");
                    WasSuccess = true;
                }
                catch (System.Exception ex)
                {

                }
                finally
                {

                }
                return WasSuccess;
            }


        
      
        #region CADConnectionFunctionality
        private void ConnectAcad(bool visible = true)
            {
                if (_acadApp == null)
                {
                    _acadApp = FindOrCreate();
                    WaitForCAD();
                }
            }
        [CommandMethod("ConnectToAcad")]
        public void ConnectAppToAcad()
        {
            try
            {
                AcadDocument doc = null;
                bool HasActiveDocument = false;
                try
                {
                    var tmp = _acadApp.Documents.Count;
                    if (tmp > 0)
                        HasActiveDocument = true;
                    else
                    {
                        doc = _acadApp.Documents.Add();
                    }
                }
                catch (COMException ex)
                {
                    doc = _acadApp.Documents.Add();
                    HasActiveDocument = true;
                }
                if (HasActiveDocument)
                {
                    doc = _acadApp.ActiveDocument;
                }

                string location = Directory.GetCurrentDirectory();
                location = location.Replace("\\", "/");

                doc.SendCommand("(command " + (char)34 + "NETLOAD" + (char)34 + " " + (char)34 + location + "/WPF_EAC_AcadPlugin.dll" + (char)34 + ") ");

                isConnected = true;
                ProcessHandle = _acadApp.Application.HWND;
            }
            catch (System.Exception ex)
            {

            }

        }
        private AcadApplication FindOrCreate()
            {
                AcadApplication application = null;
                try
                {
                    application = (Marshal2.GetActiveObject(AcadVersion) as AcadApplication);
                }
                catch (System.Exception)
                {
                    string environmentVar = Environment.GetEnvironmentVariable(AutoCADWorkingDirectory_envVariable);
                    if (string.IsNullOrEmpty(environmentVar) == false)
                    {
                        string fileName = environmentVar + "\\acad.exe";
                        using (Process.Start(fileName))
                        {
                            int i = 0;
                            while (i < 50)
                            {
                                i++;
                                Thread.Sleep(1000);
                                try
                                {
                                    object activeObject = Marshal2.GetActiveObject(AcadVersion);
                                    application = (activeObject as AcadApplication);
                                    break;
                                }
                                catch (System.Exception)
                                { application = null; }
                            }
                        }
                    }
                }
                return application;
            }
        public void WaitForCAD()
            {
                if (_acadApp != null)
                {
                    bool StillWaiting = true;
                    int start = 0;
                    int end = 100;
                    while (StillWaiting)
                    {
                        try
                        {
                            int count = _acadApp.Documents.Count;
                            StillWaiting = false;
                        }
                        catch (COMException ex)
                        {
                            int errorCode = ex.ErrorCode;
                            if (errorCode != -2147417846)
                            {

                            }
                            else if (start > end)
                            {
                                start++;
                                Thread.Sleep(1000);
                            }
                        }
                        catch (System.Exception)
                        { }
                        finally
                        {
                            start++;
                            if (start > end)
                            { StillWaiting = false; }
                        }
                    }
                }
            }
        internal void CloseCAD()
        {
            try
            {
                _acadApp.Documents.Close();
            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                _acadApp = null;
            }
        }
        public void WaitForAcadCommand(AcadDocument doc, string CommandName)
            {
                bool flag = true;
                int num = 0;
                int num2 = 100;
                while (flag)
                {
                    try
                    {
                        doc.SendCommand(CommandName);
                        flag = false;
                    }
                    catch (COMException ex)
                    {
                        int errorCode = ex.ErrorCode;
                        bool flag2 = errorCode != -2147417846;
                        if (flag2)
                        {
                            throw ex;
                        }
                        bool flag3 = num > num2;
                        if (flag3)
                        {

                        }
                        Thread.Sleep(500);
                    }
                    catch (System.Exception ex2)
                    {

                    }
                    finally
                    {
                        num++;
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
            }

        // in my opinion methods below are not reliable for handling bulk AutoCAD operations. It's better to use command chains from within the plugin
        #region OptionalHelpMethods 
        private void ClearGarbage()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        private void AutoCAD_AwaitCommand()
        {
            _DAcadApplicationEvents_EndCommandEventHandler handler = new
                _DAcadApplicationEvents_EndCommandEventHandler(CommandEnded);
            _acadApp.EndCommand += handler;
            waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            waitHandle.WaitOne(5000);
            _acadApp.EndCommand -= handler;
        }
        public void CommandEnded(string globalCommandName)
        {
            waitHandle.Set();
        }
        public IAcadApplication GetAcad()
        {
            if (_acadApp != null)
            {
                try
                {
                    var tmp = _acadApp.Documents.Count;
                }
                catch (COMException ex)
                {
                    _acadApp = null;
                    ConnectAcad();
                    if (isConnected != true || ProcessHandle != _acadApp.Application.HWND)
                    {
                        isConnected = false;
                        ConnectAppToAcad();
                    }
                    _acadApp.Visible = true;
                    Thread.Sleep(1000);

                }
                return _acadApp;
            }
            else
            {
                ConnectAcad();
                ConnectAppToAcad();
                try
                {
                    _acadApp.Visible = true;

                }
                catch (Autodesk.AutoCAD.Runtime.Exception)
                { }
                Thread.Sleep(1000);
            }
            return _acadApp;
        }
        #endregion
        #endregion
    }

}
