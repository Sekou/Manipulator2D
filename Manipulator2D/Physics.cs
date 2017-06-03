﻿using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;


namespace Manipulator2D
{
    //структура, описывающая положение 2d-тела
    public struct Pose
    {
        public float xc, yc, angle_rad;
    }
    //структура, описывающая физические параметры тела
    public struct BodyBehaviour
    {
        public bool isDynamic;
        public bool HasNoBody;
        public float mass;
    }
    //класс, упрощающий работу с физикой
    public class Physics
    {
        public static float scale = 41;
        //переменная для управления объектами моделируемого мира
        public World world;

        public float2 p0, p1;

        public Physics(float2 p0, float2 p1)
        {
            this.p0 = p0; this.p1 = p1;
            var worldAABB = new AABB();
            float d = 0;// 0.3f * (p1 - p0).Length();
            worldAABB.LowerBound.Set(p0.X - d, p0.Y - d);
            worldAABB.UpperBound.Set(p1.X + d, p1.Y + d);
            bool doSleep = true;
            world = new World(worldAABB, new Vec2(0, 0), doSleep);
        }

        //шаг рассчета физической модели мира
        public void Step(float dt)
        {
            world.Step(dt, 100, 100); //warning - mn
            //world.Validate();
        }

        //создание в указанном месте коробки заданного размера и массы.
        public Body CreateBox(Pose p, Vec2 sz, BodyBehaviour b, object UserData)
        {
            var bodyDef = new BodyDef();

            bodyDef.LinearDamping = 0.3f;
            bodyDef.AngularDamping = 0.3f;

            bodyDef.Position.Set(p.xc, p.yc);
            bodyDef.Angle = p.angle_rad;

            var body = world.CreateBody(bodyDef);

            // описание формы физического объекта
            var shapeDef = new PolygonDef();

            if (b.HasNoBody) shapeDef.SetAsBox(0.00f, 0.00f);
            else shapeDef.SetAsBox(sz.X / 2, sz.Y / 2);

            if (b.isDynamic)
            {
                // вычисление плотности коробки по ее массе
                shapeDef.Density = b.mass / (sz.X * sz.Y);
                shapeDef.Friction = 1000;
            }
            if (b.HasNoBody)
            {
                //не сталкивается
                shapeDef.IsSensor = true;
            }


            // в общем случае физическое тело может состоять из нескольких геом. объектов.
            // здесь он только один - shapeDef
            body.CreateFixture(shapeDef);

            if (b.isDynamic) body.SetMassFromShapes();

            // устанавливаем замедление скорости тела со временем (для иммитации трения о поверхность)
            //body.SetLinearDamping(0.9f);
            //body.SetAngularDamping(0.9f);

            body.SetUserData(UserData);

            return body;
        }

        //создание в указанном месте коробки заданного размера и массы.
        public Body CreateCircle(Pose p, float R, BodyBehaviour b, object UserData)
        {
            var bodyDef = new BodyDef();

            bodyDef.LinearDamping = 0.3f;
            bodyDef.AngularDamping = 0.3f;

            bodyDef.Position.Set(p.xc, p.yc);
            bodyDef.Angle = p.angle_rad;

            var body = world.CreateBody(bodyDef);

            // описание формы физического объекта
            var shapeDef = new CircleDef();
            shapeDef.Radius = R;

            if (b.isDynamic)
            {
                // вычисление плотности коробки по ее массе
                shapeDef.Density = b.mass / ((float)System.Math.PI * R * R);
                shapeDef.Friction = 1000;

            }

            // в общем случае физическое тело может состоять из нескольких геом. объектов.
            // здесь он только один - shapeDef
            body.CreateFixture(shapeDef);

            if (b.isDynamic) body.SetMassFromShapes();

            // устанавливаем замедление скорости тела со временем (для иммитации трения о поверхность)
            //body.SetLinearDamping(0.3f);
            //body.SetAngularDamping(0.3f);

            body.SetUserData(UserData);

            return body;
        }


        List<Joint> joints = new List<Joint>();
        // Соединение двух тел при помощи вращательного соединения
        // http://www.iforce2d.net/b2dtut/joints-revolute
        public Joint ConnectBodies(Body b1, float x1, float y1, Body b2, float x2, float y2)
        {
            var revoluteJointDef = new RevoluteJointDef();
            revoluteJointDef.Body1 = b1;
            revoluteJointDef.Body2 = b2;
            revoluteJointDef.CollideConnected = false;
            revoluteJointDef.LocalAnchor1.Set(x1, y1);
            revoluteJointDef.LocalAnchor2.Set(x2, y2);
            var j = world.CreateJoint(revoluteJointDef);

            joints.Add(j);

            return j;
        }

        public void DisconnectBodies(Body b1, Body b2)
        {
            for (int i = joints.Count - 1; i >= 0; i--)
            {
                var j = joints[i];
                if (j.GetBody1() == b1 && j.GetBody2() == b2 || j.GetBody2() == b1 && j.GetBody1() == b2)
                {
                    joints.RemoveAt(i);
                    world.DestroyJoint(j);
                }
            }
        }

    }

    public class float2
    {
        public float X, Y;
        public float2(float x_, float y_)
        { X = x_; Y = y_; }
        public void init(float x_, float y_)
        { X = x_; Y = y_; }
        public float2() : this(0, 0) { }

        public static float2 operator +(float2 v1, float2 v2)//сумма векторов
        { return new float2(v1.X + v2.X, v1.Y + v2.Y); }
        public static float2 operator -(float2 v1, float2 v2)//разность векторов
        { return new float2(v1.X - v2.X, v1.Y - v2.Y); }
        public static float2 operator *(float2 v1, float k)//умножение вектора на число
        { return new float2(v1.X * k, v1.Y * k); }
        public static float2 operator *(float k, float2 v1)//умножение числа на вектор
        { return v1 * k; }
        public static float2 operator /(float2 v1, float k)//деление вектора на число
        { return new float2(v1.X / k, v1.Y / k); }

        public bool Coinsides(float2 other)
        {
            return (X == other.X && Y == other.Y);
        }

        public static implicit operator PointF(float2 p)
        {
            return new PointF(p.X, p.Y);
        }

        public static implicit operator float2(PointF p)
        {
            return new float2(p.X, p.Y);
        }

        public static implicit operator Point(float2 p)
        {
            return new Point((int)System.Math.Round(p.X), (int)System.Math.Round(p.Y));
        }

        public static implicit operator float2(Point p)
        {
            return new float2(p.X, p.Y);
        }
        public static implicit operator Vec2(float2 p)
        {
            return new Vec2(p.X, p.Y);
        }

        public static implicit operator float2(Vec2 p)
        {
            return new float2(p.X, p.Y);
        }
        public float Length()
        {
            return (float)System.Math.Sqrt(X * X + Y * Y);
        }
        public float Length2()
        {
            return X * X + Y * Y;
        }
        public string ToString()
        {
            return string.Format("{0}, {1}",
            X.ToString(CultureInfo.InvariantCulture),
            Y.ToString(CultureInfo.InvariantCulture));
        }
        public float Angle()
        {
            return (float)System.Math.Atan2(Y, X);
        }
        public float2 Copy()
        {
            return new float2(X, Y);
        }
    }

}
