using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_EAC_AcadPlugin.Utils
{
    public static class Extension
    {
        public static Matrix3d EyeToWorld(this AbstractViewTableRecord view)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            return
                Matrix3d.Rotation(-view.ViewTwist, view.ViewDirection, view.Target) *
                Matrix3d.Displacement(view.Target.GetAsVector()) *
                Matrix3d.PlaneToWorld(view.ViewDirection);
        }

        public static Matrix3d WorldToEye(this AbstractViewTableRecord view)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            return
                Matrix3d.WorldToPlane(view.ViewDirection) *
                Matrix3d.Displacement(view.Target.GetAsVector().Negate()) *
                Matrix3d.Rotation(view.ViewTwist, view.ViewDirection, view.Target);
        }

        public static Matrix3d DCS2WCS(this Editor ed)
        {
            if (ed == null)
                throw new ArgumentNullException("ed");
            using (ViewTableRecord view = ed.GetCurrentView())
            {
                return view.EyeToWorld();
            }
        }
        public static Matrix3d WCS2DCS(this Editor ed)
        {
            if (ed == null)
                throw new ArgumentNullException("ed");
            using (ViewTableRecord view = ed.GetCurrentView())
            {
                return view.WorldToEye();
            }
        }
    }
}
