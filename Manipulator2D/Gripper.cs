using Box2DX.Dynamics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Manipulator2D
{
    public class Gripper//захват
    {
        public void Grasp(bool grasp)
        {
            d = grasp ? 0.2f : 0.5f;
        }
        float d = 0;

        Entity e1, e2;
        public Gripper(Physics ph)
        {
            e1 = new Entity(ph, new float2(0, 0), new float2(0.1f, 0.6f), 0, false);//губка схвата 1
            e2 = new Entity(ph, new float2(0, 0), new float2(0.1f, 0.6f), 0, false);//губка схвата 2
            Grasp(false);
        }

        public void UpdatePose(Robot r)
        {
            //var a = 0;// r.AngleEnd + (float)Math.PI / 2;
            var a = 0;// (float)Math.PI / 2;
            var s = (float)System.Math.Sin(a);
            var c = (float)System.Math.Cos(a);
            var v = new float2(c, s)*d;
            e1.body.SetXForm((float2)r.end+v, a); //установка положения схвата по концевой точке манипулятора
            e2.body.SetXForm((float2)r.end-v, a); //установка положения схвата по концевой точке манипулятора
        }

        public void Draw(Graphics g)
        {
            e1.Draw(g);
            e2.Draw(g);
        }
    }
}
