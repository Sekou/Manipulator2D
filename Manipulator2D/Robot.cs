//S. Diane 2015 - 2017
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Manipulator2D
{
    public class Link //Звено
    {
        public float L; //length 
        public float alpha; //local angle

        public float X, Y, Angle, X1, Y1; //глобальные координаты
        public void Calc()
        {
            X1 = X + L * (float)Math.Cos(Angle);
            Y1 = Y + L * (float)Math.Sin(Angle);
        }
        Pen pen = new Pen(System.Drawing.Color.Black, 2 / Physics.scale);
        public void Draw(Graphics g)
        {
            g.DrawLine(pen, X, Y, X1, Y1);
            DrawPt(g, X, Y);
            DrawPt(g, X1, Y1);
        }

        public static void DrawPt(Graphics g, float x, float y)
        {
            var r = 3/Physics.scale;
            g.FillEllipse(Brushes.Blue, x - r, y - r, r * 2, r * 2);
        }
    }
    public class Robot //создание робота с несколькими звеньями
    {
        public PointF end;
        public float AngleEnd;
        public PointF goal;

       public List<Link> links = new List<Link>();
        public Robot(PointF p0, float L, int N)
        {
            for (int i = 0; i < N; i++)
            {
                links.Add(new Link { L = L / (0.5f*i + 1), alpha = 0.3f });//?
            }
            links[0].X = p0.X;
            links[0].Y = p0.Y;
        }
        public void Draw(Graphics g)
        {
            for (int i = 0; i < links.Count; i++)
            {
                links[i].Draw(g);
            }

            Link.DrawPt(g, goal.X, goal.Y);//DrawPt вызывается из двух мест?
        }

        //расчет
        public void Calc()
        {
            for (int i = 0; i < links.Count; i++)
            {
                links[i].Angle = 0;
            }
            for (int i = 0; i < links.Count; i++) //расчет углов в глобальной системе координат
            {
                
                links[i].Angle = links[i].alpha;
                if (i > 0)
                {
                    links[i].Angle += links[i - 1].Angle;

                    links[i].X = links[i - 1].X1;
                    links[i].Y = links[i - 1].Y1;
                }
                
                links[i].Calc();
            }
            var l = links[links.Count - 1];
            end.X= l.X1; end.Y = l.Y1;
            AngleEnd = l.Angle;
        }

        public PointF SetAngles(float[] angles)
        {
            for (int i = 0; i < links.Count; i++)
            {
                links[i].alpha = angles[i];
            }
            Calc();
            var end = links[links.Count - 1];
            return new PointF(end.X1, end.Y1);
        }

        public PointF IncrementAngles(float[] vals)
        {
            for (int i = 0; i < links.Count; i++)
                links[i].alpha += vals[i];
            Calc();
            var end = links[links.Count - 1];
            return new PointF(end.X1, end.Y1);
        }

        static float Dist2(PointF a, PointF b)
        {
            float dx=a.X-b.X, dy=a.Y-b.Y;
            return dx*dx+dy*dy;
        }

        //покоординатный спуск вдоль одной координаты
        public void Descent(PointF goal, int coord_id, float step, int iters)
        {
            var L = links[coord_id];
            Calc();

            var dmin = Dist2(end, goal);
            bool stop = false;

            for (int i = 0, sign = 1; !stop && i < iters; i++)
            {
                if (!stop) L.alpha += sign * step;
                Calc();
                var d = Dist2(end, goal);
                if (d < dmin) dmin = d;
                else
                {//двигаться обратно
                    if (i > 0) stop = true;
                    else iters++;
                    sign = -sign;
                    L.alpha += sign * step;//вернуться на шаг
                }
            }

            Calc();
        }

        //покоординатный спуск по всем координатам
        public float[] DescentAll(PointF goal, float[] speeds, int iters)
        {
            var res = new float[links.Count];
            for (int i = 0; i < links.Count; i++)
                res[i] = links[i].alpha;

            for(int i=0;i<links.Count;i++)
                Descent(goal, i, speeds[i], iters);

            for (int i = 0; i < links.Count; i++)
            {
                var t = links[i].alpha;
                links[i].alpha = res[i];
                res[i] = t;
            }
            
            return res;
        }
       
     }
}
