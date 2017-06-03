using Box2DX.Dynamics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Manipulator2D
{
    public class Entity
    {
      //  float2 p;  //center
        float2 size; //w,h
     public   Body body; //физическое тело

     public Entity(Physics ph, float2 p0, float2 size0, float angle_deg, bool isDynamic)
     {
         // p = p0;
         size = size0;

         body = ph.CreateBox(new Pose { xc = p0.X, yc = p0.Y, angle_rad = angle_deg/180*(float)System.Math.PI },
             size, new BodyBehaviour { isDynamic = isDynamic, mass = 0.1f }, null);
     }

     public void Draw(Graphics g)
     {
         var T = g.Transform;
         var dx = size.X / 2;
         var dy = size.Y / 2;
         var p = body.GetPosition();
         g.TranslateTransform(p.X, p.Y);
         g.RotateTransform(180 / (float)System.Math.PI * body.GetAngle());
         var pen = new Pen(System.Drawing.Color.Black, 2 / Physics.scale);
         g.DrawRectangle(pen, -dx, -dy, size.X, size.Y);
         g.Transform = T;

     }
    }
}
