using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Manipulator2D
{
    public partial class Form1 : Form
    {
        Robot r;//многозвенный робот
        Graphics g;
        Physics ph;

        Entity box, box2, boxFloor;
        Gripper gripper;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            r = new Robot(new float2(3, 1), 2.5f, 3);
            pb.Image = new Bitmap(pb.Width, pb.Width);
            g = Graphics.FromImage(pb.Image);
            r.Calc();

            ph = new Physics(new float2(), new float2(pb.Width, pb.Height));
            box = new Entity(ph, new float2(3, 3), new float2(0.6f, 1.2f), 0, true);//создали коробку в 100 100 на экране.
            box2 = new Entity(ph, new float2(3.7f, 0.3f), new float2(0.6f, 1.2f), 90, true);//создали коробку в 100 100 на экране.
            boxFloor = new Entity(ph, new float2(4.3f, 6), new float2(9, 0.6f), 0, false);//создали пол

            gripper = new Gripper(ph);

            DrawAll();

            g.ScaleTransform(Physics.scale, Physics.scale);
        }

        private void DrawAll()
        {
            g.Clear(Color.White);
            r.Draw(g);
            box.Draw(g);
            box2.Draw(g);
            boxFloor.Draw(g);
            gripper.Draw(g);

            pb.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            box.body.ApplyForce(new float2(0, 1), box.body.GetPosition());//сила притяжения коробки
            box2.body.ApplyForce(new float2(0, 1), box2.body.GetPosition());//сила притяжения коробки

            var dt = timer1.Interval / 1000f;

            //for (int i = 0, N=10; i < N; i++)//расчет физики схвата
            //{
            //   // var p0 = boxGripper.body.GetPosition();
            //   // var F = (float2)r.end - (float2)p0;
            //   // var V = (float2)boxGripper.body.GetLinearVelocity();
            //   // var F_ = new float2(System.Math.Sign(F.X), System.Math.Sign(F.Y));
            //   //boxGripper.body.SetLinearVelocity(F_*100);
                
            //    //boxGripper.body.SetXForm(boxGripper.body.GetPosition(), 0);
            //    ph.Step(dt/N);
            //}

            gripper.UpdatePose(r);

            ph.Step(dt);


            if (!r.goal.IsEmpty)
            {
                var A = r.DescentAll(r.goal, new float[] { 0.01f, 0.01f, 0.01f, 0.01f, 0.01f }, 10);
                for (int i = 0; i < A.Length; i++)
                {
                    A[i] -= r.links[i].alpha;
                    A[i] = 10f *limit(A[i], 1) * (float)System.Math.PI / 180;
                }
                r.IncrementAngles(A);
                
            }
            DrawAll();
        }

        float limit(float x, float A)
        {
            if (x > A) return A;
            if (x < -A) return -A;
            return x;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            r.goal = 1/Physics.scale*(float2)e.Location;
        }

        private void cb_grasp_CheckedChanged(object sender, EventArgs e)
        {
            gripper.Grasp(cb_grasp.Checked);
        }
    }
}
