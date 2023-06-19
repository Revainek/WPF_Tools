using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Exception = Autodesk.AutoCAD.Runtime.Exception;
using System.Reflection;
using System.IO;
using WPF_EAC_AcadPlugin.Utils;
using Autodesk.AutoCAD.Interop;
using ACADCommon = Autodesk.AutoCAD.Interop.Common;
using AcadDocument = Autodesk.AutoCAD.Interop.AcadDocument;

using Autodesk.AutoCAD.BoundaryRepresentation;
using Face = Autodesk.AutoCAD.DatabaseServices.Face;

using System.Security.Cryptography;




namespace WPF_EAC_AcadPlugin
{
    [ProgId("WPF_EAC_AcadPlugin")]
    public class WPF_EAC_AcadPluginClass : IExtensionApplication
    {
        #region Initialization

        [CommandMethod("EAC_ConnectionTestCommand")]
        public void MyFirstTestCommand()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
           
            var ed = doc.Editor;
                ed.WriteMessage("Connection Success. My First test Command is running correctly.");
        }
        void IExtensionApplication.Initialize()
        {

        }

        void IExtensionApplication.Terminate()
        {

        }
        #endregion
    }
}